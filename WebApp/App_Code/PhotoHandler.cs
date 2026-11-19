
using System;
using System.IO;
using System.Web;

namespace SmartPhotoGallery
{
    public class PhotoHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            string relativePath = context.Request.QueryString["path"];

            if (String.IsNullOrWhiteSpace(relativePath))
            {
                context.Response.StatusCode = 400;
                return;
            }

            try
            {
                string root = Path.GetFullPath(GalleryConfig.PhotoRoot)
                    .TrimEnd(Path.DirectorySeparatorChar)
                    + Path.DirectorySeparatorChar;

                string requestedPath = relativePath.Replace(
                    '/', Path.DirectorySeparatorChar);

                string fullPath = Path.GetFullPath(
                    Path.Combine(root, requestedPath));

                if (!fullPath.StartsWith(
                    root, StringComparison.OrdinalIgnoreCase))
                {
                    context.Response.StatusCode = 403;
                    return;
                }

                if (!File.Exists(fullPath))
                {
                    context.Response.StatusCode = 404;
                    return;
                }

                string extension =
                    Path.GetExtension(fullPath).ToLowerInvariant();

                if (extension == ".jpg" || extension == ".jpeg")
                    context.Response.ContentType = "image/jpeg";
                else if (extension == ".png")
                    context.Response.ContentType = "image/png";
                else if (extension == ".gif")
                    context.Response.ContentType = "image/gif";
                else if (extension == ".bmp")
                    context.Response.ContentType = "image/bmp";
                else
                {
                    context.Response.StatusCode = 403;
                    return;
                }

                context.Response.TransmitFile(fullPath);
            }
            catch
            {
                context.Response.StatusCode = 404;
            }
        }
    }
}
