using System;
using System.Configuration;
using System.IO;
using System.Web;

namespace SmartPhotoGallery
{
    public static class GalleryConfig
    {
        public static string PhotoRoot
        {
            get
            {
                string value = ConfigurationManager.AppSettings["PhotoRoot"];
                if (String.IsNullOrEmpty(value)) throw new Exception("PhotoRoot is not configured.");
                return value;
            }
        }

        public static string ImageUrlPrefix
        {
            get { return (ConfigurationManager.AppSettings["ImageUrlPrefix"] ?? "").TrimEnd('/'); }
        }

        public static string FaceApiUrl
        {
            get { return (ConfigurationManager.AppSettings["FaceApiUrl"] ?? "http://127.0.0.1:5005").TrimEnd('/'); }
        }


        public static string ToImageUrl(string relativePath)
        {
            string normalized = (relativePath ?? "")
                .Replace('\\', '/')
                .TrimStart('/');

            return VirtualPathUtility.ToAbsolute("~/Photo.ashx")
                + "?path=" + HttpUtility.UrlEncode(normalized);
        }

    }
}
