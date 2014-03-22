using PintheCloudWS.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PintheCloudWS.Utilities
{
    public class FileObjectDataContext
    {
        //public static string DBConnectionString = "Data Source=isostore:/ToDo.sdf";

        public FileObjectDataContext(string constr)
        {

        }

        //public Table<FileObjectSQL> FileItems;
    }




    public class FileObjectSQL
    {
        //[Column(IsPrimaryKey = true, IsDbGenerated = false, DbType = "NVarChar(50) NOT NULL", CanBeNull = false, AutoSync = AutoSync.Default)]
        [PrimaryKey, Column("Id")]
        public string Id { get; set; }
        [SQLite.Column("Name")]
        public string Name { get; set; }
        [Column("Size")]
        public double Size { get; set; }
        [Column("Type")]
        public PintheCloudWS.Models.FileObject.FileObjectType Type { get; set; }
        [Column("Extension")]
        public string Extension { get; set; }
        [Column("Update")]
        public string UpdateAt { get; set; }
        [Column("Thumbnail")]
        public string Thumbnail { get; set; }
        [Column("DownloadUrl")]
        public string DownloadUrl { get; set; }
        [Column("MimeType")]
        public string MimeType { get; set; }
        [Column("ProfileId")]
        public string ProfileId { get; set; }
        [Column("ProfileEmail")]
        public string ProfileEmail { get; set; }
        [Column("ProfilePhoneNumber")]
        public string ProfilePhoneNumber { get; set; }
        [Column("ProfileName")]
        public string ProfileName { get; set; }
        [Column("SpotId")]
        public string SpotId { get; set; }
        [Column("ParentId")]
        public string ParentId { get; set; }
    }
}
