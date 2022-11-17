using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FileStorage_Client.ServiceReference1;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace FileStorage_Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IService1Callback
    {
        bool loginTbFlag, passwordTbFlag, loginButtonFlag, connectionFlag, folderSelect;
        string selectionFlag, login, password, pswrdTxt, storagePath, movingFileName, pathIni, pathFin, downloadFolder;
        int storageLevelCount;
        List<MyFile> uploadList, downloadList;
        
        Random rand = new Random();
        Service1Client client;

        SolidColorBrush c1;
        SolidColorBrush c2;

        public MainWindow()
        {
            InitializeComponent();
            loginTbFlag = false;
            connectionFlag = false;
            passwordTbFlag = false;
            loginButtonFlag = false;
            folderSelect = false;
            downloadFolder = null;
            selectionFlag = null;
            password = null;
            pswrdTxt = null;
            login = null;
            storagePath = null;
            movingFileName = null;
            pathIni = null;
            pathFin = null;
            storageLevelCount = 0;
            uploadList = new List<MyFile>();
            downloadList = new List<MyFile>();

            SkinChange();
            selectionFlag = "blank";
            BlankTab.Focus();
        }

        private void SkinChange()
        {
            int R = rand.Next(256);
            int G = rand.Next(256);
            int B = rand.Next(256);
            SolidColorBrush color1 = new SolidColorBrush(Color.FromRgb((byte)R, (byte)G, (byte)B));


            double r, g, b, min, max, br, delta, s, h;
            int v = 0;
            r = R / 255.0;
            g = G / 255.0;
            b = B / 255.0;
            min = r < g ? r : g;
            min = min < b ? min : b;
            max = r > g ? r : g;
            max = max > b ? max : b;
            br = max;
            delta = max - min;

            if (delta < 0.00001)
            {
                s = 0;
                h = 0;
                return;
            }
            if (max > 0.0)
            {
                s = (delta / max);
            }
            else
            {
                s = 0.0;
                h = double.NaN;
            }
            if (r >= max)
                h = (g - b) / delta;
            else
                if (g >= max)
                h = 2.0 + (b - r) / delta;
            else
                h = 4.0 + (r - g) / delta;

            h *= 60.0;

            if (h < 0.0)
                h += 360.0;

            if (br < 0.6) v = 255;
            else if (s > 0.6)
            {
                if (h < 40 || h > 190) v = 255;
                else v = 0;
            }
            else if (h < 50 || h > 200) v = 255;


            //messageTb.Text = br + " " + s + " " + h;
            //chatSpace.Items.Add(br + " " + s + " " + h);

            SolidColorBrush color2 = new SolidColorBrush(Color.FromRgb((byte)v, (byte)v, (byte)v));
            c1 = color1;
            c2 = color2;

            mainBorder.BorderBrush = color1;
            mainBorder.Background = color2;
            loginButton.Background = color1;
            loginButton.Foreground = color2;
            loginButton.Tag = color2;

            connectButton.Background = color1;
            connectButton.Foreground = color2;
            loginTb.Background = color1;
            loginTb.Foreground = color2;
            passwordTb.Background = color1;
            passwordTb.Foreground = color2;

            connectButton.Background = color1;
            connectButton.Foreground = color2;

            credentialsTb.Background = color1;
            credentialsTb.Foreground = color2;            

            viewButton.Background = color1;
            viewButton.Foreground = color2;
            uploadViewButton.Background = color1;
            uploadViewButton.Foreground = color2;
            downloadViewButton.Background = color1;
            downloadViewButton.Foreground = color2;

            downloadButton.Background = color1;
            downloadButton.Foreground = color2;
            uploadButton.Background = color1;
            uploadButton.Foreground = color2;

            skinButton.Background = color1;
            skinButton.Foreground = color2;

            helpButton.Background = color1;
            helpButton.Foreground = color2;

            aboutButton.Background = color1;
            aboutButton.Foreground = color2;

            exitButton.Background = color1;
            exitButton.Foreground = color2;         

            downloadServerL.Foreground = color1;
            downloadClientL.Foreground = color1;
            uploadServerL.Foreground = color1;
            uploadClientL.Foreground = color1;
            viewServerL.Foreground = color1;
            BlankGrid.Background = color2;            
        }

        private void loginTbMouseDown(object sender, MouseEventArgs e)
        {
            if (loginTbFlag == false)
            {
                loginTbFlag = true;
                loginTb.Text = "";
            }
        }

        private void passwordTbMouseDown(object sender, MouseEventArgs e)
        {
            if (passwordTbFlag == false)
            {
                passwordTbFlag = true;
                passwordTb.Text = "";
            }
        }
        private void passwordTb_KeyUp(object sender, KeyEventArgs e)
        {
            if (passwordTbFlag == true && passwordTb.Text.Length > 0)
            {
                password += passwordTb.Text[passwordTb.Text.Length - 1];
                pswrdTxt += "*";
                passwordTb.Text = pswrdTxt;
                passwordTb.CaretIndex = passwordTb.Text.Length;
            }
        }
        private void grid_click(object sender, RoutedEventArgs e)
        {
            this.DragMove();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (loginButtonFlag == false)
            {
                loginTb.Visibility = Visibility.Visible;
                passwordTb.Visibility = Visibility.Visible;
                connectButton.Visibility = Visibility.Visible;
                loginButtonFlag = true;
            }
            else if (loginButtonFlag == true)
            {
                try
                {
                    if (await client.LogOutAsync(login) == Code.exit)
                    {
                        MessageBox.Show("Succefully logged out!");
                        credentialsTb.Visibility = Visibility.Hidden;
                        loginButton.Content = "Log In";
                        loginButtonFlag = false;
                        selectionFlag = "blank";
                        BlankTab.Focus();                      
                        passwordTb.Visibility = Visibility.Hidden;
                        loginTb.Visibility = Visibility.Hidden;
                        loginTb.Text = "Enter login";
                        loginTbFlag = false;
                        passwordTbFlag = false;
                        passwordTb.Text = "Enter password";
                        connectButton.Visibility = Visibility.Hidden;

                        viewButton.Visibility = Visibility.Hidden;
                        uploadViewButton.Visibility = Visibility.Hidden;
                        downloadViewButton.Visibility = Visibility.Hidden;

                        login = null;
                        password = null;
                        pswrdTxt = null;
                        connectionFlag = false;
                    }
                }
                catch (FaultException ex)
                {
                    MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace+"\r\n" + ex.Reason);
                }
                catch (CommunicationException ex)
                {
                    MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (loginTb.Text.Length > 0 && loginTb.Text != "Enter login")
            {
                if (passwordTb.Text.Length > 0 && passwordTb.Text != "Enter password")
                {
                    login = loginTb.Text;
                    
                    IService1Callback callback = this as IService1Callback;
                    InstanceContext context = new InstanceContext(callback);
                    client = new Service1Client();
                    try
                    {
                        switch (await client.LogInAsync(login, password))
                        {
                            case Code.login:
                                {
                                    MessageBox.Show("Successfully logged in");
                                    loginTb.Visibility = Visibility.Hidden;
                                    passwordTb.Visibility = Visibility.Hidden;
                                    connectButton.Visibility = Visibility.Hidden;
                                    loginButton.Content = "Log out";
                                    loginButtonFlag = true;
                                    credentialsTb.Visibility = Visibility.Visible;
                                    credentialsTb.Text = "You are logged in as: " + login;
                                    viewButton.Visibility = Visibility.Visible;
                                    uploadViewButton.Visibility = Visibility.Visible;
                                    downloadViewButton.Visibility = Visibility.Visible;
                                    connectionFlag = true;
                                    break;
                                }
                            case Code.registration:
                                {
                                    MessageBox.Show("Successfully registered and logged in");
                                    loginTb.Visibility = Visibility.Hidden;
                                    passwordTb.Visibility = Visibility.Hidden;
                                    connectButton.Visibility = Visibility.Hidden;
                                    loginButton.Content = "Log out";
                                    loginButtonFlag = true;
                                    credentialsTb.Visibility = Visibility.Visible;
                                    credentialsTb.Text = "You are logged in as: " + login;
                                    viewButton.Visibility = Visibility.Visible;
                                    uploadViewButton.Visibility = Visibility.Visible;
                                    downloadViewButton.Visibility = Visibility.Visible;
                                    connectionFlag = true;
                                    break;
                                }

                            case Code.passwordError:
                                {
                                    MessageBox.Show("Incorrect password!");
                                    passwordTbFlag = false;
                                    passwordTb.Text = "Enter password";
                                    password = null;
                                    pswrdTxt = null;
                                    break;
                                }
                        }
                    }
                    catch (FaultException ex)
                    {
                        MessageBox.Show(ex.Message + "\r\n" + ex.Reason + "\r\n" + ex.StackTrace);
                    }
                    catch (CommunicationException ex)
                    {
                        MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void viewButton_Click(object sender, RoutedEventArgs e)
        {
            selectionFlag = "view";
            ViewTab.Focus();
            storagePath = "\\";
            storageLevelCount = 1;
            RefreshStorageView();
        }

        private void uploadViewButton_Click(object sender, RoutedEventArgs e)
        {
            selectionFlag = "upload";
            UploadTab.Focus();
            storagePath = "\\";
            storageLevelCount = 1;
            RefreshStorageView();
        }

        private void downloadViewButton_Click(object sender, RoutedEventArgs e)
        {
            selectionFlag = "download";
            DownloadTab.Focus();
            storagePath = "\\";
            storageLevelCount = 1;
            RefreshStorageView();
        }

        private async void downloadButton_Click(object sender, RoutedEventArgs e)
        {
            if (folderSelect == false)
            {
                var dialog = new CommonOpenFileDialog();
                dialog.IsFolderPicker = true;
                dialog.InitialDirectory = @"c:\";
                CommonFileDialogResult result = dialog.ShowDialog();

                if (result == CommonFileDialogResult.Ok)
                {
                    downloadFolder = null;
                    downloadFolder = dialog.FileName;
                    folderSelect = true;
                    downloadButton.Content = "Download selected files";                    
                }
            }
            else if (folderSelect == true)
            {
                foreach (MyFile f in downloadList)
                {
                    string newFileName = null;
                    //destination name
                    if (f.Name.Contains(".packed")) newFileName = f.Name.Substring(0, f.Name.LastIndexOf(".packed"));
                    else newFileName = f.Name;
                    string destinationFile = downloadFolder + "\\" + newFileName;
                    // MessageBox.Show(destinationFile);

                    //check if file with the same name exists in the destination folder
                    List<string> fileList = new List<string>();
                    DirectoryInfo d1 = new DirectoryInfo(downloadFolder);
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
                        catch (FaultException ex)
                        {
                            MessageBox.Show(ex.Message + "\r\n" + ex.Reason + "\r\n" + ex.StackTrace);
                        }
                        catch (CommunicationException ex)
                        {
                            MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        try
                        {
                            DirectoryInfo[] dirs = d1.GetDirectories();
                            foreach (DirectoryInfo current in dirs)
                            {
                                fileList.Add(current.FullName);
                            }
                        }
                        catch (FaultException ex)
                        {
                            MessageBox.Show(ex.Message + "\r\n" + ex.Reason + "\r\n" + ex.StackTrace);
                        }
                        catch (CommunicationException ex)
                        {
                            MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                    //if it does its name is altered - e.g. text.txt->text_Copy_0.txt
                    int a = 0;
                    string destinationFileName = destinationFile;
                    if (fileList.Contains(destinationFileName))
                    {
                        destinationFileName = destinationFile.Insert(destinationFile.LastIndexOf('.'), "_Copy_" + a);

                        while (fileList.Contains(destinationFileName))
                        {
                            a++;
                            destinationFileName = destinationFile.Insert(destinationFile.LastIndexOf('.'), "_Copy_" + a);
                        }
                        destinationFile = destinationFileName;
                    }
                    //MessageBox.Show(destinationFile);

                    if (f.Size < 157286400)
                    {
                        try
                        {  //save file to destination folder
                            byte[] buffer = null;
                            buffer = await client.FileDownloadAsync(storagePath, f.Name);
                            FileStream destFileStream = new FileStream(destinationFile, FileMode.Create, FileAccess.Write);
                            BinaryWriter bw = new BinaryWriter(destFileStream, Encoding.Default);
                            bw.Write(buffer);
                            destFileStream.Close();
                        }
                        catch (FaultException ex)
                        {
                            MessageBox.Show(ex.Message + "\r\n" + ex.Reason + "\r\n" + ex.StackTrace);
                        }
                        catch (CommunicationException ex)
                        {
                            MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }

                    else
                    {
                        try
                        {
                            FileStream destFile = File.Create(destinationFile);
                            Stream st = await client.BigFileDownloadAsync(storagePath, f.Name);
                            byte[] buffer = new byte[1024 * 1024];
                            while (true)
                            {
                                int res = await st.ReadAsync(buffer, 0, buffer.Length);
                                if (res == 0) break;
                                destFile.Write(buffer, 0, res);
                            }

                            destFile.Close();
                            st.Close();
                        }
                        catch (FaultException ex)
                        {
                            MessageBox.Show(ex.Message + "\r\n" + ex.Reason + "\r\n" + ex.StackTrace);
                        }
                        catch (CommunicationException ex)
                        {
                            MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
                downloadList.Clear();
                DownloadUpdate();

                downloadFolder = null;
                folderSelect = false;
                downloadButton.Content = "Select folder for download";
            }
        }

        private void skinButton_Click(object sender, RoutedEventArgs e)
        {
            SkinChange();
        }

        private void helpButton_Click(object sender, RoutedEventArgs e)
        {
            RulesWin rw = new RulesWin(c1, c2);
            bool result = (bool)rw.ShowDialog();
        }

        private void aboutButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("FileStorage application was designed and developed by Oleg Sydorov in February 2022\r\nas part of study course in ITStep Academy.\r\nNo rights reserved.\r\nNo complaints accepted.");
        }

        private async void exitButton_Click(object sender, RoutedEventArgs e)
        {
            if (connectionFlag==true)
            try
            {
                if (await client.LogOutAsync(login) == Code.exit)
                {
                    MessageBox.Show("Succefully logged out!");
                    this.Close();
                }
            }
            catch (FaultException<string> ex)
            {
                MessageBox.Show(ex.Detail);
            }
            else this.Close();
        }

        private void uploadClientDragEnter(object sender, DragEventArgs e)
        {   
            if (e.Data.GetDataPresent(DataFormats.FileDrop) &&
                (e.AllowedEffects & DragDropEffects.Copy) != 0)
            {
         
                e.Effects = DragDropEffects.Copy;
            }
        }

        private void uploadClientDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) &&
               (e.AllowedEffects & DragDropEffects.Copy) != 0 )
            {
                string[] str = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string s in str)
                {
                    FileInfo f = new FileInfo(s);
                    if (f.Exists) uploadList.Add(new MyFile() { Name = f.Name, Extension = f.Extension, LastChanged = f.LastWriteTime, Size = f.Length, Type = "file", Path=f.FullName});
                    else MessageBox.Show("Folders cannot be added to tray!");
                }
                UploadUpdate();
            }
        }

        private void UploadUpdate()
        {
            ObservableCollection<MyFile> observableCollection = new ObservableCollection<MyFile>(uploadList);
            CollectionViewSource collection = new CollectionViewSource() { Source = observableCollection };
            UploadClientDataGrid.ItemsSource = null;
            UploadClientDataGrid.ItemsSource = collection.View;
        }        

        delegate void RefreshViewCallbackDelegate();

        private async void RefreshStorageView()
        {
            if (!Dispatcher.CheckAccess())   // запущена ли эта функция в чужом потоке?
            {
                RefreshViewCallbackDelegate r = new RefreshViewCallbackDelegate(RefreshStorageView);

                Dispatcher.Invoke(r, new object[] { });
            }
            else
            {
                String baseDirectoryPath = System.AppDomain.CurrentDomain.BaseDirectory;
                switch (selectionFlag)
                {
                    case "view":
                        {
                            try
                            {
                                var list = await client?.GetFileListAsync(storagePath);
                                foreach (MyFile f in list) f.Path = (f.Type == "file") ? baseDirectoryPath + "\\archiveIcon.png" : baseDirectoryPath + "\\folderIcon.jpg";
                                ObservableCollection<MyFile> observableCollection = new ObservableCollection<MyFile>(list);
                                CollectionViewSource collection = new CollectionViewSource() { Source = observableCollection };
                                ViewDataGrid.ItemsSource = null;
                                ViewDataGrid.ItemsSource = collection.View;
                            }
                            catch (FaultException ex)
                            {
                                MessageBox.Show(ex.Message + "\r\n"+ex.Reason + "\r\n" + ex.StackTrace);
                            }
                            catch (CommunicationException ex)
                            {
                                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                            break;
                        }
                    case "upload":
                        {
                            try
                            {
                                var list = await client?.GetFileListAsync(storagePath);
                                ObservableCollection<MyFile> observableCollection = new ObservableCollection<MyFile>(list);
                                CollectionViewSource collection = new CollectionViewSource() { Source = observableCollection };
                                UploadServerDataGrid.ItemsSource = null;
                                UploadServerDataGrid.ItemsSource = collection.View;
                            }
                            catch (FaultException ex)
                            {
                                MessageBox.Show(ex.Message + "\r\n" + ex.Reason + "\r\n" + ex.StackTrace);
                            }
                            catch (CommunicationException ex)
                            {
                                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                            break;
                        }
                    case "download":
                        {
                            try
                            {
                                var result = await client?.GetFileListAsync(storagePath);
                                ObservableCollection<MyFile> observableCollection = new ObservableCollection<MyFile>(result);
                                CollectionViewSource collection = new CollectionViewSource() { Source = observableCollection };
                                DownloadServerDataGrid.ItemsSource = null;
                                DownloadServerDataGrid.ItemsSource = collection.View;
                            }
                            catch (FaultException ex)
                            {
                                MessageBox.Show(ex.Message + "\r\n" + ex.Reason + "\r\n" + ex.StackTrace);
                            }
                            catch (CommunicationException ex)
                            {
                                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                            break;
                        }
                }
            }
        }

        private async void uploadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (MyFile d in uploadList)
                {

                    await client.FileNameUploadAsync(d.Name);

                    //read bytes and send them to the server
                    //if (d.Size < 157286400)
                    //{
                        FileStream infile = new FileStream(d.Path, FileMode.Open, FileAccess.Read, FileShare.Read);
                        BinaryReader br = new BinaryReader(infile, Encoding.Default);
                        byte[] buffer = null;
                        buffer = br.ReadBytes(Convert.ToInt32(infile.Length));
                        infile.Close();
                        await client.FileUploadAsync(buffer);
                    //}
                    //else
                    //{
                    //    Stream st;
                    //    st = new FileStream(d.Path, FileMode.Open, FileAccess.Read);
                    //    await client.BigFileUploadAsync(st);
                    //}
                    // UploadClientDataGrid.Items.Remove(d);
                }
            }
            catch (FaultException ex)
            {
                MessageBox.Show(ex.Message+"\r\n"+ex.StackTrace+"\r\n"+ex.Reason);
            }
            catch (CommunicationException ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            uploadList.Clear();
            UploadUpdate();
            RefreshStorageView();
            //UploadClientDataGrid.Items.Clear();
        }

        public void OnFileUploaded(MyFile file)
        {
            //UploadServerDataGrid.Items.Add(file);
            MessageBox.Show("OnFileUploaded");
            RefreshStorageView();
        }

        private async void deleteFile_Click(object sender, RoutedEventArgs e)
        {
            if (ViewDataGrid.SelectedItems.Count > 0)
            {
                MyFile selectedFile = ViewDataGrid.SelectedItem as MyFile;

                if (selectedFile.Type == "file")
                {
                    try
                    {
                        await client.FileDeleteAsync(storagePath, selectedFile.Name);
                    }
                    catch (FaultException ex)
                    {
                        MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace + "\r\n" + ex.Reason);
                    }
                    catch (CommunicationException ex)
                    {
                        MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    RefreshStorageView();

                }
                else MessageBox.Show("No file selected!");
            }
            else MessageBox.Show("No files selected!");
        }

        private void openFolderDownloadGrid_Click(object sender, RoutedEventArgs e)
        {
            if (DownloadServerDataGrid.SelectedItems.Count > 0)
            {
                MyFile selectedFile = DownloadServerDataGrid.SelectedItem as MyFile;

                if (selectedFile.Type == "folder")
                {
                    storageLevelCount++;
                    if (storagePath == "\\") storagePath = "\\" + selectedFile.Name;
                    else storagePath += "\\" + selectedFile.Name;
                    RefreshStorageView();

                }
                else MessageBox.Show("No folders selected!");
            }
            else MessageBox.Show("No files selected!");
        }

        private async void deleteFolder_Click(object sender, RoutedEventArgs e)
        {
            if (ViewDataGrid.SelectedItems.Count > 0)
            {
                MyFile selectedFile = ViewDataGrid.SelectedItem as MyFile;

                if (selectedFile.Type == "folder")
                {
                    try
                    {
                        await client.FileDeleteAsync(storagePath, selectedFile.Name);
                    }
                    catch (FaultException ex)
                    {
                        MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace + "\r\n" + ex.Reason);
                    }
                    catch (CommunicationException ex)
                    {
                        MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    RefreshStorageView();
                }
                else MessageBox.Show("No folders selected!");
            }
            else MessageBox.Show("No files selected!");
        }
        private async void createFolder_Click(object sender, RoutedEventArgs e)
        {
            await client.FolderCreateAsync(storagePath);
            RefreshStorageView();
        }
        private void openFolder_Click(object sender, RoutedEventArgs e)
        {
            if (ViewDataGrid.SelectedItems.Count > 0)
            {
                MyFile selectedFile = ViewDataGrid.SelectedItem as MyFile;

                if (selectedFile.Type == "folder")
                {
                    storageLevelCount++;
                    if (storagePath=="\\") storagePath = "\\"+selectedFile.Name;
                    else storagePath+= "\\" + selectedFile.Name;
                    RefreshStorageView();

                }
                else MessageBox.Show("No folders selected!");
            }
            else MessageBox.Show("No files selected!");           
        }

        private void cutFile_Click(object sender, RoutedEventArgs e)
        {
            if (ViewDataGrid.SelectedItems.Count > 0)
            {
                MyFile selectedFile = ViewDataGrid.SelectedItem as MyFile;

                if (selectedFile.Type == "file")
                {
                    movingFileName = selectedFile.Name;
                    pathIni = storagePath;
                }
                else MessageBox.Show("No file selected!");
            }
            else MessageBox.Show("No files selected!");
        }
        private async void pasteFile_Click(object sender, RoutedEventArgs e)
        {
            pathFin = storagePath;
            if (pathFin != pathIni)
            {
                try
                {
                    await client.FileMoveAsync(pathIni, movingFileName, pathFin);
                }
                catch (FaultException ex)
                {
                    MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace + "\r\n" + ex.Reason);
                }
                catch (CommunicationException ex)
                {
                    MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                RefreshStorageView();
            }
        }


        private void removeFromUploadTray_Click(object sender, RoutedEventArgs e)
        {
            if (UploadClientDataGrid.SelectedItems.Count > 0)
            {
                MyFile selectedFile = UploadClientDataGrid.SelectedItem as MyFile;
                uploadList.Remove(selectedFile);
                UploadUpdate();
            }
            else MessageBox.Show("No files selected!");
        }

        private void clearUploadTray_Click(object sender, RoutedEventArgs e)
        {
            uploadList.Clear();
            UploadUpdate();
        }

        private void removeFromDownloadTray_Click(object sender, RoutedEventArgs e)
        {
            if (DownloadClientDataGrid.SelectedItems.Count > 0)
            {
                MyFile selectedFile = DownloadClientDataGrid.SelectedItem as MyFile;
                downloadList.Remove(selectedFile);
                DownloadUpdate();
            }
            else MessageBox.Show("No files selected!");
        }

        private void clearDownloadTray_Click(object sender, RoutedEventArgs e)
        {
           downloadList.Clear();
            DownloadUpdate();
        }

        private void downloadServerGridPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point pt = e.GetPosition(this);
            HitTestResult res = System.Windows.Media.VisualTreeHelper.HitTest(this, pt);
            if (!(res.VisualHit is TextBlock))
                return;
            TextBlock tb = (TextBlock)res.VisualHit;
            MyFile file = (tb).DataContext as MyFile;           
            string msg = null;
            msg += file.Name;
            msg += "\\" + file.Extension;
            msg += "\\" + file.Size;
            msg += "\\"+file.Type;
            msg += "\\" + file.LastChanged;
            DataObject data1 = new DataObject(msg);
            // MessageBox.Show(msg);
            //data1.SetFileDropList(col);
            data1.SetText(msg);
          //  data1.SetData("MyAppformat", 0);
            DragDropEffects dde = DragDrop.DoDragDrop(this, data1, DragDropEffects.Copy);
        }

        private void downloadClientDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.StringFormat) &&
                (e.AllowedEffects & DragDropEffects.Copy) != 0)
                //&& e.Data.GetDataPresent("Myappformat"))
            {
                e.Effects = DragDropEffects.Copy;
            }
        }

        private void downloadClientDrop(object sender, DragEventArgs e)
        {
           // MessageBox.Show("drop");
            if (e.Data.GetDataPresent(typeof(string)))
            {
             //   MessageBox.Show("start extricate");
                string str = (string)e.Data.GetData(DataFormats.StringFormat);
                //string str = e.Data.();
              //  MessageBox.Show(str);
                char[] separators = new char[] { '\\' };
                string[] SUBs = str.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                if (SUBs[3] == "file")
                {
                    downloadList.Add(new MyFile()
                    {
                        Name = SUBs[0],
                        Extension = SUBs[1],
                        Size = Convert.ToInt64(SUBs[2]),
                        Type = SUBs[3],
                        LastChanged = Convert.ToDateTime(SUBs[4]),
                    });
                    DownloadUpdate();
                }
                else MessageBox.Show("Folders cannot be added to tray!");

                //if (str[3] == "file")
                //{
                //    downloadList.Add(new MyFile()
                //    {
                //        Name = str[0],
                //        Extension = str[1],
                //        Size = Convert.ToInt64(str[2]),
                //        Type = str[3],
                //        LastChanged = Convert.ToDateTime(str[4]),
                //    });
                //}
            }           
        }

        private void DownloadUpdate()
        {
            ObservableCollection<MyFile> observableCollection = new ObservableCollection<MyFile>(downloadList);
            CollectionViewSource collection = new CollectionViewSource() { Source = observableCollection };
            DownloadClientDataGrid.ItemsSource = null;
            DownloadClientDataGrid.ItemsSource = collection.View;
        }

    }

    internal interface IService1Callback
    {
    }
}
