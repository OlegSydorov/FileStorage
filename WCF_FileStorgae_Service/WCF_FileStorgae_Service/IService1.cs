using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WCF_FileStorgae_Service
{
    public interface IMyCallBack
    {
        [OperationContract]
        void OnFileUploaded(MyFile file);
        
    }

    [ServiceContract(CallbackContract = typeof(IMyCallBack))]
    public interface IService1
    {
        [OperationContract]       
        Code LogIn(string login, string password);

        [OperationContract]
        Code LogOut(string login);

        [OperationContract]
        List<MyFile> GetFileList(string path);

        [OperationContract]
        void FileDelete(string path, string name);

        //[OperationContract]
        //[FaultContract(typeof(string))]
        //void DeleteAllFiles(string path, string name);

        [OperationContract]
        void FileMove(string folderIni, string name, string folderFin);

        
        
        [OperationContract]        
        byte[] FileDownload(string folderIni, string name);

        [OperationContract]      
        Stream BigFileDownload(string folderIni, string name);


        [OperationContract]       
        void FileNameUpload(string fileName);
        
        [OperationContract]       
        void FileUpload(byte[] arr);

        [OperationContract]
        void BigFileUpload(Stream s);
        

        [OperationContract]
        void FolderCreate(string folderIni);

        [OperationContract]
        MyFile GetDataUsingDataContract(MyFile file);

        [OperationContract]
        string GetDataStatus(Code state);

    }

    [DataContract]
    public class MyFile
    {
        //[DataMember]
        //public string FileIcon { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Extension { get; set; }
        [DataMember]
        public long Size { get; set; }
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public string Path { get; set; }
        [DataMember]
        public DateTime LastChanged { get; set; }
    }

    [DataContract]
    public enum Code
    {
        [EnumMember]
        login,
        [EnumMember]
        registration,
        [EnumMember]
        passwordError,
        [EnumMember]
        exit,
        [EnumMember]
        comms     
    }
}
