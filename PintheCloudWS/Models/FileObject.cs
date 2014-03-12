﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
