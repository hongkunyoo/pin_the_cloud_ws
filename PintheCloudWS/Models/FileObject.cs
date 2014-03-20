﻿using Newtonsoft.Json;
using PintheCloudWS.Helpers;
using PintheCloudWS.Managers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace PintheCloudWS.Models
{
    public class FileObject
    {
        #region Variables
        public enum FileObjectType { FOLDER, FILE, NOTEBOOK, GOOGLE_DOC };
        /// <summary>
        /// The id to Get, Upload, Download
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// The name of the file
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The parent Id to get back to the parent tree.
        /// </summary>
        //public string ParentId { get; set; }
        /// <summary>
        /// The size of the file
        /// </summary>
        public double Size { get; set; }
        /// <summary>
        /// file or folder
        /// </summary>
        public FileObjectType Type { get; set; }
        /// <summary>
        /// The file extension such as mp3, pdf
        /// </summary>
        public string Extension { get; set; }
        /// <summary>
        /// For created time & updated time.
        /// </summary>
        public string UpdateAt { get; set; }
        /// <summary>
        /// Thumbnail information
        /// </summary>
        public string Thumbnail { get; set; }
        /// <summary>
        /// download Url for GoogleDrive.
        /// </summary>
        public string DownloadUrl { get; set; }
        /// <summary>
        /// MimeType for GoogleDrive.
        /// </summary>
        public string MimeType { get; set; }
        /// <summary>
        /// The child list of the folder.
        /// </summary>
        public ProfileObject Owner { get; set; }

        public string SpotId { get; set; }

        public List<FileObject> FileList { get; set; }
        #endregion

        public FileObject()
        {
        }

        public FileObject(string Id, string Name, double Size, FileObjectType Type, string Extension, string UpdateAt, string Thumbnail = null, string DownloadUrl = null, string MimeType = null)
        {
            this.Id = Id;
            this.Name = Name;
            this.Size = Size;
            this.Type = Type;
            this.Extension = Extension;
            this.UpdateAt = UpdateAt;
            this.Thumbnail = Thumbnail;
            this.DownloadUrl = DownloadUrl;
            this.MimeType = MimeType;
        }


        public async Task<StorageFile> DownloadToLocal(StorageFile file = null)
        {
            if (file == null)
            {
                file = await ApplicationData.Current.LocalFolder.CreateFileAsync(this.Name, CreationCollisionOption.ReplaceExisting);
            }
            return await App.BlobStorageManager.DownloadFileAsync(this.Id, file);
        }

        public async Task<bool> DownloadToCloud(IStorageManager storageManager = null)
        {
            try
            {
                if (storageManager == null)
                    storageManager = Switcher.GetMainStorage();
                Stream instream = await App.BlobStorageManager.DownloadFileStreamAsync(this.Id);
                FileObject root = await storageManager.GetRootFolderAsync();
                await storageManager.UploadFileStreamAsync(root.Id, this.Name, instream);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static FileObject ConvertToFileObject(MSFileObject msfo)
        {
            FileObject fo = new FileObject();
            fo.Id = msfo.id;
            fo.Name = msfo.name;
            fo.Type = (FileObjectType)Enum.Parse(typeof(FileObjectType), msfo.file_type);
            fo.Size = msfo.size;
            fo.Extension = msfo.extension;
            fo.UpdateAt = msfo.update_at;
            fo.DownloadUrl = msfo.download_url;
            fo.MimeType = msfo.mime_type;
            fo.SpotId = msfo.spot_id;
            fo.Owner = new ProfileObject(msfo.owner_account_id, msfo.owner_account_name);
            return fo;
        }

        public static MSFileObject ConvertToMSFileObject(FileObject fo)
        {
            return new MSFileObject(fo.Name, fo.Type.ToString(), fo.Size, fo.Extension, fo.UpdateAt, fo.DownloadUrl, fo.MimeType, fo.Owner.Id, fo.Owner.Name, fo.SpotId);
        }


        #region Test Methods
        public static void PrintFile(FileObject file)
        {
            Debug.WriteLine("id : " + file.Id);
            Debug.WriteLine("Name : " + file.Name);
            Debug.WriteLine("Size : " + file.Size);
            Debug.WriteLine("Type : " + file.Type);
            Debug.WriteLine("Extension : " + file.Extension);
            Debug.WriteLine("UpdateAt : " + file.UpdateAt);
            Debug.WriteLine("Thumbnail : " + file.Thumbnail);
            Debug.WriteLine("DownloadUrl : " + file.DownloadUrl);
            Debug.WriteLine("MimeType : " + file.MimeType);

            Debug.WriteLine("----child-----");
            PrintFileList(file.FileList);
        }

        public static void PrintFileList(List<FileObject> list)
        {
            if (list == null) return;
            foreach (FileObject file in list)
            {
                PrintFile(file);
            }
        }
        #endregion
    }

    #region Mobile Service Object
    public class MSFileObject
    {
        public string id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string name { get; set; }

        [JsonProperty(PropertyName = "file_type")]
        public string file_type { get; set; }

        [JsonProperty(PropertyName = "size")]
        public double size { get; set; }

        [JsonProperty(PropertyName = "extension")]
        public string extension { get; set; }

        [JsonProperty(PropertyName = "update_at")]
        public string update_at { get; set; }

        [JsonProperty(PropertyName = "download_url")]
        public string download_url { get; set; }

        [JsonProperty(PropertyName = "mime_type")]
        public string mime_type { get; set; }

        [JsonProperty(PropertyName = "owner_account_id")]
        public string owner_account_id { get; set; }

        [JsonProperty(PropertyName = "owner_account_name")]
        public string owner_account_name { get; set; }

        [JsonProperty(PropertyName = "spot_id")]
        public string spot_id { get; set; }

        public MSFileObject(string name, string file_type, double size, string extension, string update_at, string download_url,
            string mime_type, string owner_account_id, string owner_account_name, string spot_id)
        {
            this.name = name;
            this.file_type = file_type;
            this.size = size;
            this.extension = extension;
            this.update_at = update_at;
            this.download_url = download_url;
            this.mime_type = mime_type;
            this.owner_account_id = owner_account_id;
            this.owner_account_name = owner_account_name;
            this.spot_id = spot_id;
        }
    }
    #endregion
}
