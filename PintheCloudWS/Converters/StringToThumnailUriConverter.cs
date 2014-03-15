﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace PintheCloudWS.Converters
{
    public class StringToThumnailUriConverter : IValueConverter
    {
        // Instances
        private const string FOLDER_IMAGE_PATH = "/Assets/pajeon/pick/png/icon_file_folder.png";
        private const string DOC_IMAGE_PATH = "/Assets/pajeon/pick/png/icon_file_folder.png";
        private const string ETC_IMAGE_PATH = "/Assets/pajeon/pick/png/icon_file_etc.png";
        private const string HWP_IMAGE_PATH = "/Assets/pajeon/pick/png/icon_file_hwp.png";
        private const string IMG_IMAGE_PATH = "/Assets/pajeon/pick/png/icon_file_image.png";
        private const string PDF_IMAGE_PATH = "/Assets/pajeon/pick/png/icon_file_pdf.png";
        private const string PPT_IMAGE_PATH = "/Assets/pajeon/pick/png/icon_file_ppt.png";
        private const string SOUND_IMAGE_PATH = "/Assets/pajeon/pick/png/icon_file_sound.png";
        private const string VIDEO_IMAGE_PATH = "/Assets/pajeon/pick/png/icon_file_video.png";
        private const string XLS_IMAGE_PATH = "/Assets/pajeon/pick/png/icon_file_xls.png";
        private const string ZIP_IMAGE_PATH = "/Assets/pajeon/pick/png/icon_file_zip.png";

        // Folder
        private const string FOLDER = "folder";

        // Sound
        private const string MP3 = "mp3";
        private const string WMA = "wma";

        // Video
        private const string MP4 = "mp4";
        private const string MPEG = "mpeg";
        private const string WMV = "wmv";
        private const string AVI = "avi";
        private const string MKV = "mkv";
        private const string FLV = "flv";
        private const string MOV = "mov";

        // Image
        private const string JPG = "jpg";
        private const string JPEG = "jpeg";
        private const string PNG = "png";
        private const string GIF = "gif";
        private const string BMP = "bmp";

        // Document
        private const string PPT = "ppt";
        private const string PPTX = "pptx";
        private const string PDF = "pdf";
        private const string DOC = "doc";
        private const string DOCX = "docx";
        private const string XLS = "xls";
        private const string XLSX = "xlsx";
        private const string HWP = "hwp";

        // ETC
        private const string ZIP = "zip";


        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string type = ((string)value).ToLower();

            if (type.Equals(FOLDER))  // Folder
                return FOLDER_IMAGE_PATH;

            else if (type.Equals(MP3) || type.Equals(WMA))  // Sound
                return SOUND_IMAGE_PATH;

            else if (type.Equals(MP4) || type.Equals(WMV) || type.Equals(MPEG) || type.Equals(AVI) || type.Equals(MKV) || type.Equals(FLV) || type.Equals(MOV))  // video
                return VIDEO_IMAGE_PATH;

            else if (type.Equals(JPG) || type.Equals(JPEG) || type.Equals(PNG) || type.Equals(GIF) || type.Equals(BMP))  // Image
                return IMG_IMAGE_PATH;

            else if (type.Equals(PPT) || type.Equals(PPTX))  // PPT
                return PPT_IMAGE_PATH;

            else if (type.Equals(DOC) || type.Equals(DOCX))  // Word
                return DOC_IMAGE_PATH;

            else if (type.Equals(XLS) || (type.Equals(XLSX))) // XLS
                return XLS_IMAGE_PATH;

            else if (type.Equals(HWP))  // HWP
                return HWP_IMAGE_PATH;

            else if (type.Equals(PDF))  // PDF
                return PDF_IMAGE_PATH;

            else if (type.Equals(ZIP))  // ZIP
                return ZIP_IMAGE_PATH;

            else
                return ETC_IMAGE_PATH;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
