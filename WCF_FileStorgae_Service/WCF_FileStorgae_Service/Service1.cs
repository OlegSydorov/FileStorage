using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Windows;

namespace WCF_FileStorgae_Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class Service1 : IService1
    {
        FileStorageEntities context = new FileStorageEntities();
        string storageRootPath = "E:\\FILE_STORAGE";
        string storageUserPath = null;
        string login = null;
        string uploadFileName = null;
        string fileName = null;
        IMyCallBack callback;

        public Code LogIn(string login, string password)
        {
            try
            {
                if (context.Users.Select(t => t).Where(t => t.Login == login).Count() > 0)
                {
                    if (context.Users.Select(t => t).Where(t => t.Login == login && t.Password == password).Count() > 0)
                    {
                        User selectedUser = (from t in context.Users
                                             where t.Login == login
                                             select t).First();

                        selectedUser.Status = "online";
                        context.SaveChanges();
                        this.login = login;
                        storageUserPath = System.IO.Path.Combine(storageRootPath, login);

                        return Code.login;
                    }
                    else
                    {
                        return Code.passwordError;
                    }
                }
                else
                {
                    try
                    {
                        User user = new User
                        {
                            Login = login,
                            Password = password,
                            Registration = DateTime.Now,
                            Status = "online",
                            Files = 0,
                            Bytes = 0,
                        };

                        context.Users.Add(user);

                        context.SaveChanges();
                        this.login = login;
                        storageUserPath = System.IO.Path.Combine(storageRootPath, login);
                        System.IO.Directory.CreateDirectory(storageUserPath);

                        return Code.registration;
                    }
                    catch (Exception ex)
                    {
                        throw new FaultException(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }
        public Code LogOut(string login)
        {
            try
            {
                User selectedUser = (from t in context.Users
                                     where t.Login == login
                                     select t).First();

                selectedUser.Status = "offline";
                context.SaveChanges();
                login = null;
                return Code.exit;
            }

            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }

        public List<MyFile> GetFileList(string path)
        {
            List<MyFile> fileList = new List<MyFile>();
            string auxPath = (path == "\\") ? storageUserPath : storageUserPath + path;

            DirectoryInfo d1 = new DirectoryInfo($"{auxPath}");
            if (d1.Exists == true)
            {
                try
                {
                    FileInfo[] files = d1.GetFiles();
                    foreach (FileInfo current in files)
                    {
                        //Icon ic = Icon.ExtractAssociatedIcon(current.FullName);
                        fileList.Add(new MyFile { Name = current.Name,
                            Extension = current.Extension,
                            LastChanged = current.CreationTime,
                            Size = current.Length,
                            Type = "file",
                        });
                    }

                }
                catch (Exception ex)
                {
                    throw new FaultException(ex.Message);
                }
                try
                {
                    DirectoryInfo[] dirs = d1.GetDirectories();
                    foreach (DirectoryInfo current in dirs)
                    {
                        fileList.Add(new MyFile { Name = current.Name,
                            Extension = current.Extension,
                            LastChanged = current.CreationTime,
                            Type = "folder",
                        });
                    }
                }
                catch (Exception ex)
                {
                    throw new FaultException(ex.Message);
                }
            }
            else Console.WriteLine("Incorrect path entered!");

            return fileList;
        }

        public void FileDelete(string path, string name)
        {
            string auxPath = (path == "\\") ? storageUserPath : storageUserPath + path;

            DirectoryInfo d1 = new DirectoryInfo($"{auxPath}");
            if (d1.Exists == true)
            {
                try
                {
                    FileInfo[] files = d1.GetFiles();
                    foreach (FileInfo current in files)
                    {
                        if (current.Name == name)
                        {
                            User selectedUser = (from t in context.Users
                                                 where t.Login == login
                                                 select t).First();

                            selectedUser.Files--;
                            selectedUser.Bytes -= current.Length;
                            context.SaveChanges();
                            File.Delete(current.FullName);
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new FaultException(ex.Message);
                }
                try
                {
                    DirectoryInfo[] dirs = d1.GetDirectories();
                    foreach (DirectoryInfo current in dirs)
                    {
                        if (current.Name == name)
                        {
                            DeleteAllFiles(current.FullName);
                            current.Delete();

                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new FaultException(ex.Message);
                }
            }
            else Console.WriteLine("Incorrect path entered!");
        }

        private void DeleteAllFiles(string path)
        {
            DirectoryInfo d1 = new DirectoryInfo($"{path}");
            if (d1.Exists == true)
            {
                try
                {
                    FileInfo[] files = d1.GetFiles();
                    foreach (FileInfo current in files)
                    {
                        User selectedUser = (from t in context.Users
                                             where t.Login == login
                                             select t).First();

                        selectedUser.Files--;
                        selectedUser.Bytes -= current.Length;
                        context.SaveChanges();
                        File.Delete(current.FullName);
                    }
                }
                catch (Exception ex)
                {
                    throw new FaultException(ex.Message);
                }
                try
                {
                    DirectoryInfo[] dirs = d1.GetDirectories();
                    foreach (DirectoryInfo current in dirs)
                    {
                        DeleteAllFiles(current.FullName);
                        current.Delete();
                    }
                }
                catch (Exception ex)
                {
                    throw new FaultException(ex.Message);
                }
            }
            else Console.WriteLine("Incorrect path entered!");
        }

        public void FileMove(string folderIni, string name, string folderFin)
        {
            try
            {
                string filePathIni = (folderIni != "\\") ? storageUserPath + folderIni + "\\" + name : storageUserPath + "\\" + name;
                string filePathFin = (folderFin != "\\") ? storageUserPath + folderFin + "\\" + name : storageUserPath + "\\" + name;

                //check if file with the same name already exists in the destination folder
                string auxPath = (folderFin != "\\") ? storageUserPath + "\\" + folderFin : storageUserPath;
                List<string> fileList = new List<string>();
                DirectoryInfo d1 = new DirectoryInfo($"{auxPath}");
                if (d1.Exists == true)
                {
                    try
                    {
                        FileInfo[] files = d1.GetFiles();
                        foreach (FileInfo current in files)
                        {
                            fileList.Add(current.FullName);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new FaultException(ex.Message);
                    }
                    try
                    {
                        DirectoryInfo[] dirs = d1.GetDirectories();
                        foreach (DirectoryInfo current in dirs)
                        {
                            fileList.Add(current.FullName);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new FaultException(ex.Message);
                    }
                }


                //if it does its name is altered - e.g. text.txt->text_Copy_0.txt
                int a = 0;
                string newFileName = filePathFin;
                if (fileList.Contains(newFileName))
                {
                    newFileName = filePathFin.Insert(filePathFin.LastIndexOf('.'), "_Copy_" + a);

                    while (fileList.Contains(newFileName))
                    {
                        a++;
                        newFileName = filePathFin.Insert(filePathFin.LastIndexOf('.'), "_Copy_" + a);
                    }
                    filePathFin = newFileName;
                }

                //file is copied from source folder, saved to destination folder and then deleted in the source folder
                FileStream infile = new FileStream(filePathIni, FileMode.Open, FileAccess.Read, FileShare.Read);
                BinaryReader br = new BinaryReader(infile, Encoding.Default);
                byte[] buffer = null;
                buffer = br.ReadBytes(Convert.ToInt32(infile.Length));
                infile.Close();

                FileStream destFile = new FileStream(filePathFin, FileMode.Create, FileAccess.Write);
                BinaryWriter bw = new BinaryWriter(destFile, Encoding.Default);
                bw.Write(buffer);
                destFile.Close();

                File.Delete(filePathIni);
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }

        public byte[] FileDownload(string folderIni, string name)
        {
            try
            {
                string filePathIni = (folderIni != "\\") ? storageUserPath + folderIni + "\\" + name : storageUserPath + "\\" + name;

                FileStream infile = new FileStream(filePathIni, FileMode.Open, FileAccess.Read, FileShare.Read);

                MemoryStream mem = new MemoryStream();

                GZipStream zipStream = new GZipStream(infile, CompressionMode.Decompress);
                int b = zipStream.ReadByte();
                while (b != -1)
                {
                    mem.WriteByte((byte)b);
                    b = zipStream.ReadByte();
                }


                BinaryReader br = new BinaryReader(mem, Encoding.Default);
                byte[] buffer = null;
                mem.Position = 0;
                buffer = br.ReadBytes(Convert.ToInt32(mem.Length));

                mem.Close();
                zipStream.Close();
                infile.Close();

                return buffer;
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }

        public Stream BigFileDownload(string folderIni, string name)
        {
            try
            {
                string filePathIni = (folderIni != "\\") ? storageUserPath + folderIni + "\\" + name : storageUserPath + "\\" + name;

                if (name.Contains(".packed"))
                {
                    FileStream infile = new FileStream(filePathIni, FileMode.Open, FileAccess.Read, FileShare.Read);

                    MemoryStream mem = new MemoryStream();

                    GZipStream zipStream = new GZipStream(infile, CompressionMode.Decompress);
                    int b = zipStream.ReadByte();
                    while (b != -1)
                    {
                        mem.WriteByte((byte)b);
                        b = zipStream.ReadByte();
                    }
                    mem.Position = 0;

                    zipStream.Close();
                    infile.Close();
                    return mem;
                }
                else
                {
                    return new FileStream(filePathIni, FileMode.Open, FileAccess.Read);
                }
            }
            catch (Exception ex)
            {
                throw new FaultException<string>(ex.InnerException?.ToString());
            }
        }

        public void FileNameUpload(string fileName)
        {
            this.fileName = fileName;
            uploadFileName = storageUserPath + "\\" + fileName + ".packed";

            //check if files with the same names already exist in the root folder
            List<string> fileList = new List<string>();
            DirectoryInfo d1 = new DirectoryInfo(storageUserPath);
            if (d1.Exists == true)
            {
                try
                {
                    FileInfo[] files = d1.GetFiles();
                    foreach (FileInfo current in files)
                    {
                        fileList.Add(current.FullName);
                    }
                }
                catch (Exception ex)
                {
                    throw new FaultException(ex.Message);
                }
                try
                {
                    DirectoryInfo[] dirs = d1.GetDirectories();
                    foreach (DirectoryInfo current in dirs)
                    {
                        fileList.Add(current.FullName);
                    }
                }
                catch (Exception ex)
                {
                    throw new FaultException(ex.Message);
                }
            }
            //if it does its name is altered - e.g. text.txt->text_Copy_0.txt
            int a = 0;
            string newFileName = uploadFileName;
            if (fileList.Contains(newFileName))
            {
                newFileName = uploadFileName.Insert(uploadFileName.LastIndexOf('.'), "_Copy_" + a);

                while (fileList.Contains(newFileName))
                {
                    a++;
                    newFileName = uploadFileName.Insert(uploadFileName.LastIndexOf('.'), "_Copy_" + a);
                }
                uploadFileName = newFileName;
            }
        }
        public void FileUpload(byte[] buffer)
        {
            try
            {
                FileStream destFile = File.Create(uploadFileName);
                GZipStream compressedzipStream = new GZipStream(destFile, CompressionMode.Compress, true);
                compressedzipStream.Write(buffer, 0, buffer.Length);
                compressedzipStream.Close();
                destFile.Close();

                //update info in the database
                User selectedUser = (from t in context.Users
                                     where t.Login == login
                                     select t).First();

                selectedUser.Files++;
                selectedUser.Bytes += new System.IO.FileInfo(uploadFileName).Length;
                context.SaveChanges();

                //reflect changes in the storage view
                //MyFile file = new MyFile()
                //{
                //    Name = fileName,
                //    Size = new System.IO.FileInfo(uploadFileName).Length,
                //    LastChanged = DateTime.Now,
                //    Type = "file"
                //};

                //callback = OperationContext.Current.GetCallbackChannel<IMyCallBack>();
                //GetUpdateCallback d = new GetUpdateCallback(SendUpdate);
                //d.BeginInvoke(file, new AsyncCallback(DComplete), null);
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }

        delegate void GetUpdateCallback(MyFile file);
        public void DComplete(IAsyncResult res)
        {
        }
        public void SendUpdate(MyFile file)
        {
            try
            {
                if (callback != null)
                {
                    MessageBox.Show("Prior to OnFileLoaded");
                    callback.OnFileUploaded(file);
                }
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }


        public async void BigFileUpload(Stream str)
        {
            try
            {
                //read stream
                
                //Stream st = str;
                byte[] buffer = new byte[1024 * 1024];

                while (true)
                {
                    int res = await str.ReadAsync(buffer, 0, buffer.Length);
                    if (res == 0) break;                    
                }
                //st.Close();

                if (buffer.Length > 0)
                {
                    //compress and write file to root folder of storage
                    FileStream destFile = File.Create(uploadFileName);
                    GZipStream compressedzipStream = new GZipStream(destFile, CompressionMode.Compress, true);
                    compressedzipStream.Write(buffer, 0, buffer.Length);
                    compressedzipStream.Close();
                    destFile.Close();

                    //update info in the database
                    User selectedUser = (from t in context.Users
                                         where t.Login == login
                                         select t).First();

                    selectedUser.Files++;
                    selectedUser.Bytes += new System.IO.FileInfo(uploadFileName).Length;
                    context.SaveChanges();

                    //reflect changes in the storage view
                    //MyFile file = new MyFile()
                    //{
                    //    Name = fileName,
                    //    Size = new System.IO.FileInfo(uploadFileName).Length,
                    //    LastChanged = DateTime.Now,
                    //    Type = "file"
                    //};
                }

                //callback = OperationContext.Current.GetCallbackChannel<IMyCallBack>();
                //GetUpdateCallback d = new GetUpdateCallback(SendUpdate);
                //d.BeginInvoke(file, new AsyncCallback(DComplete), null);
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }

        

        public void FolderCreate(string folderIni)
        {
            string auxPath= (folderIni != "\\") ? storageUserPath + folderIni: storageUserPath;
            string folderPath = (folderIni != "\\") ? storageUserPath + folderIni+"\\New folder": storageUserPath+"\\New folder";

            List<string> fileList = new List<string>();

            DirectoryInfo d1 = new DirectoryInfo($"{auxPath}");
            if (d1.Exists == true)
            {
                try
                {
                    FileInfo[] files = d1.GetFiles();
                    foreach (FileInfo current in files)
                    {
                        fileList.Add(current.FullName);
                    }
                }
                catch (Exception ex)
                {
                    throw new FaultException<string>(ex.InnerException?.ToString());
                }
                try
                {
                    DirectoryInfo[] dirs = d1.GetDirectories();
                    foreach (DirectoryInfo current in dirs)
                    {
                        fileList.Add(current.FullName);
                    }
                }
                catch (Exception ex)
                {
                    throw new FaultException(ex.Message);
                }
            }

            int n = 1;
            if (fileList.Contains(folderPath))
            { 
                string folderPathFin = folderPath+n;
                while (fileList.Contains(folderPathFin))
                {
                    n++;
                    folderPathFin = folderPath + n;
                }
                folderPath = folderPathFin;
            }
                System.IO.Directory.CreateDirectory(folderPath);
        }

        public MyFile GetDataUsingDataContract(MyFile file)
        {
            if (file == null)
            {
                throw new ArgumentNullException("MyFile is null");
            }

            return file;
        }

        public string GetDataStatus(Code state)
        {
            if (state ==Code.login)
            {
                return "Code login";
            }
            if (state == Code.registration)
            {
                return "Code registration";
            }
            if (state == Code.passwordError)
            {
                return "Code passwordError";
            }
            if (state == Code.exit)
            {
                return "Code exit";
            }
            return string.Empty;
        }

    }
}
