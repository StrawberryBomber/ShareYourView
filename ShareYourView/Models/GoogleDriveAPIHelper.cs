using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;

namespace ShareYourView.Models
{
    public class GoogleDriveAPIHelper
    {
        //add scope
        public static string[] Scopes = { Google.Apis.Drive.v3.DriveService.Scope.Drive };

        //create Drive API service.
        public static DriveService GetService()
        {
            //get Credentials from client_secret.json file 
            UserCredential credential;
            //Root Folder of project
            var CSPath = System.Web.Hosting.HostingEnvironment.MapPath("~/");
            using (var stream = new FileStream(Path.Combine(CSPath, "client_secret_846325098136-evv3o015nuplv4726rfo1e0v8mfs32l0.apps.googleusercontent.com.json"), FileMode.Open, FileAccess.Read))
            {
                String FolderPath = System.Web.Hosting.HostingEnvironment.MapPath("~/"); ;
                String FilePath = Path.Combine(FolderPath, "DriveServiceCredentials.json");
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(FilePath, true)).Result;
            }
            //create Drive API service.
            DriveService service = new Google.Apis.Drive.v3.DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "GoogleDriveMVCUpload",
            });
            return service;
        }


        //file Upload to the Google Drive root folder.
        public static void UplaodFileOnDrive(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                //create service
                DriveService service = GetService();
                string path = Path.Combine(HttpContext.Current.Server.MapPath("~/GoogleDriveFiles"),
                Path.GetFileNameWithoutExtension(file.FileName) + "@" + HttpContext.Current.User.Identity.Name + Path.GetExtension(file.FileName));
                file.SaveAs(path);
                var FileMetaData = new Google.Apis.Drive.v3.Data.File();
                FileMetaData.Name = (Path.GetFileNameWithoutExtension(file.FileName) + "@" + HttpContext.Current.User.Identity.Name + Path.GetExtension(file.FileName));
                FileMetaData.MimeType = MimeMapping.GetMimeMapping(path);
                FilesResource.CreateMediaUpload request;
                using (var stream = new System.IO.FileStream(path, System.IO.FileMode.Open))
                {
                    request = service.Files.Create(FileMetaData, stream, FileMetaData.MimeType);
                    request.Fields = "id";
                    request.Upload();
                }

                using (shareYourView_DBEntities db = new shareYourView_DBEntities())
                {
                    var v = db.UserDetails.Where(a => a.user_Username == HttpContext.Current.User.Identity.Name).FirstOrDefault();
                    int user_id = v.user_ID;
                    saveImageToDatabase((Path.GetFileNameWithoutExtension(file.FileName) + "@" + HttpContext.Current.User.Identity.Name + Path.GetExtension(file.FileName)), user_id);
                }

            }
        }

        //get all files from Google Drive.
        public static List<GoogleDriveFile> GetDriveFiles()
        {
            Google.Apis.Drive.v3.DriveService service = GetService();
            // Define parameters of request.
            Google.Apis.Drive.v3.FilesResource.ListRequest FileListRequest = service.Files.List();

            // for getting folders only.
            //FileListRequest.Q = "mimeType='application/vnd.google-apps.folder'";
            FileListRequest.Fields = "nextPageToken, files(*)";
            // List files.
            IList<Google.Apis.Drive.v3.Data.File> files = FileListRequest.Execute().Files;
            List<GoogleDriveFile> FileList = new List<GoogleDriveFile>();

            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    GoogleDriveFile File = new GoogleDriveFile
                    {
                        Id = file.Id,
                        Name = file.Name,
                        Size = file.Size,
                        Version = file.Version,
                        CreatedTime = file.CreatedTime
                    };
                    if(file.Name.Contains(HttpContext.Current.User.Identity.Name))
                    {
                        File._pathName = File.Name;
                        File.Name = file.Name.Replace("@" + HttpContext.Current.User.Identity.Name, "");
                        
                        FileList.Add(File);
                    }
                    
                }
            }
            return FileList;
        }

        private static void saveImageToDatabase(string fileName, int id)
        {
            using (shareYourView_DBEntities db = new shareYourView_DBEntities())
            {
                ImageDetail imgD = new ImageDetail();
                imgD.image_Name = fileName;
                imgD.user_ID = id;
                db.ImageDetails.Add(imgD);
                db.SaveChanges();
            }
        }

        public static List<GoogleDriveFile> GetShareDriveFiles()
        {
            Google.Apis.Drive.v3.DriveService service = GetService();
            // Define parameters of request.
            Google.Apis.Drive.v3.FilesResource.ListRequest FileListRequest = service.Files.List();

            FileListRequest.Fields = "nextPageToken, files(*)";

            // List files.
            IList<Google.Apis.Drive.v3.Data.File> files = FileListRequest.Execute().Files;
            List<GoogleDriveFile> FileList = new List<GoogleDriveFile>();

            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    GoogleDriveFile File = new GoogleDriveFile
                    {
                        Id = file.Id,
                        Name = file.Name,
                        Size = file.Size,
                        Version = file.Version,
                        CreatedTime = file.CreatedTime
                    };

                    using (shareYourView_DBEntities db = new shareYourView_DBEntities())
                    {
                        var v = db.UserDetails.Where(a => a.user_Username == HttpContext.Current.User.Identity.Name).FirstOrDefault();
                        if (v != null)
                        {

                            //Get List from images
                            List<ImageShared> listShare = new List<ImageShared>();
                            listShare = db.ImageShareds.Where(a => a.user_ID == v.user_ID).ToList();

                            List<ImageDetail> listDetail = new List<ImageDetail>();

                            foreach(var i in listShare)
                            {
                                //Get Image WHere the image id is equal to the userId shared imageID
                                var x = db.ImageDetails.Where(a => a.image_ID == i.image_ID).FirstOrDefault();

                                if(x.image_Name == file.Name)
                                {
                                    File._pathName = File.Name;
                                    File.Name = file.Name.Substring(0, file.Name.IndexOf('@'));                                    
                                    FileList.Add(File);
                                }

                            }
                        }
                    }                    
                    
                }
            }
            return FileList;
        }

        //Download file from Google Drive by fileId.
        public static string DownloadGoogleFile(string fileId)
        {
            DriveService service = GetService();
            string FolderPath = HttpContext.Current.Server.MapPath("/GoogleDriveFiles/");
            FilesResource.GetRequest request = service.Files.Get(fileId);
            string FileName = request.Execute().Name;
            string FilePath = Path.Combine(FolderPath, FileName);
            MemoryStream stream1 = new MemoryStream();
            // Add a handler which will be notified on progress changes.
            // It will notify on each chunk download and when the
            // download is completed or failed.
            request.MediaDownloader.ProgressChanged += (IDownloadProgress progress) =>
            {
                switch (progress.Status)
                {
                    case DownloadStatus.Downloading:
                        {
                            Console.WriteLine(progress.BytesDownloaded);
                            break;
                        }
                    case DownloadStatus.Completed:
                        {
                            Console.WriteLine("Download complete.");
                            SaveStream(stream1, FilePath);
                            break;
                        }
                    case DownloadStatus.Failed:
                        {
                            Console.WriteLine("Download failed.");
                            break;
                        }
                }
            };
            request.Download(stream1);
            return FilePath;
        }

        // file save to server path
        private static void SaveStream(MemoryStream stream, string FilePath)
        {
            using (FileStream file = new FileStream(FilePath, FileMode.Create, FileAccess.ReadWrite))
            {
                stream.WriteTo(file);
            }
        }

        public static void DeleteFile(GoogleDriveFile file)
        {
            DriveService service = GetService();

            try
            {
                if(service == null)
                {
                    throw new ArgumentNullException("service");
                }
                if (file == null)
                {
                    throw new ArgumentNullException(file.Id);
                }

                using (shareYourView_DBEntities db = new shareYourView_DBEntities())
                {
                    string fileName = file.Name.Substring(0, file.Name.IndexOf('.'));
                    fileName += ("@" + HttpContext.Current.User.Identity.Name);
                    fileName += file.Name.Substring(file.Name.IndexOf('.'));
                                       

                    var x = db.ImageDetails.Where(a => a.image_Name == fileName).FirstOrDefault();
                    if(x != null)
                    {
                        //Debug.WriteLine("\n\n\n\n\nA=" + fileName + "=A\n\n\n\n");

                        int _imageID = x.image_ID;
                        db.ImageDetails.Remove(x);
                        //db.SaveChanges();


                        List<ImageShared> listShare = new List<ImageShared>();
                        listShare = db.ImageShareds.Where(a => a.image_ID == _imageID).ToList();

                        foreach (var c in listShare)
                        {
                            //Debug.WriteLine("\n\n\n\n\nB=" + c.image_ID + "=B\n\n\n\n");
                            db.ImageShareds.Remove(c);
                            
                        }

                        List<ImageMetadata> metaData = new List<ImageMetadata>();
                        metaData = db.ImageMetadatas.Where(a => a.image_ID == _imageID).ToList();

                        foreach(var c in metaData)
                        {
                            db.ImageMetadatas.Remove(c);
                        }


                        db.SaveChanges();
                    }
                    
                }

                service.Files.Delete(file.Id).Execute();
            }
            catch (Exception ex)
            { 

                throw new Exception("Request File.Delete failed.", ex);
            }
        }

        //// Create Folder in root
        //public static void CreateFolder(string FolderName)
        //{
        //    DriveService service = GetService();
        //    var FileMetaData = new Google.Apis.Drive.v3.Data.File();
        //    FileMetaData.Name = FolderName;
        //    //this mimetype specify that we need folder in google drive
        //    FileMetaData.MimeType = "application/vnd.google-apps.folder";
        //    FilesResource.CreateRequest request;
        //    request = service.Files.Create(FileMetaData);
        //    request.Fields = "id";
        //    var file = request.Execute();

        //}

        //// File upload in existing folder
        //public static void FileUploadInFolder(string folderId, HttpPostedFileBase file)
        //{
        //    if (file != null && file.ContentLength > 0)
        //    {
        //        //create service
        //        DriveService service = GetService();
        //        //get file path
        //        string path = Path.Combine(HttpContext.Current.Server.MapPath("~/GoogleDriveFiles"),
        //        Path.GetFileName(file.FileName));
        //        file.SaveAs(path);
        //        //create file metadata
        //        var FileMetaData = new Google.Apis.Drive.v3.Data.File()
        //        {
        //            Name = Path.GetFileName(file.FileName),
        //            MimeType = MimeMapping.GetMimeMapping(path),
        //            //id of parent folder 
        //            Parents = new List<string>
        //            {
        //                folderId
        //            }
        //        };
        //        FilesResource.CreateMediaUpload request;
        //        //create stream and upload
        //        using (var stream = new System.IO.FileStream(path, System.IO.FileMode.Open))
        //        {
        //            request = service.Files.Create(FileMetaData, stream, FileMetaData.MimeType);
        //            request.Fields = "id";
        //            request.Upload();
        //        }
        //        var file1 = request.ResponseBody;
        //    }
        //}

        //public static List<GoogleDriveFile> GetDriveFolders()
        //{
        //    DriveService service = GetService();
        //    List<GoogleDriveFile> FolderList = new List<GoogleDriveFile>();
        //    FilesResource.ListRequest request = service.Files.List();
        //    request.Q = "mimeType='application/vnd.google-apps.folder'";
        //    request.Fields = "files(id, name)";
        //    Google.Apis.Drive.v3.Data.FileList result = request.Execute();
        //    foreach (var file in result.Files)
        //    {
        //        GoogleDriveFile File = new GoogleDriveFile
        //        {
        //            Id = file.Id,
        //            Name = file.Name,
        //            Size = file.Size,
        //            Version = file.Version,
        //            CreatedTime = file.CreatedTime
        //        };
        //        FolderList.Add(File);
        //    }
        //    return FolderList;
        //}

    }
}