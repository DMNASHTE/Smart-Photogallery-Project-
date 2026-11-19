using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;

namespace SmartPhotoGallery
{
    public class PhotoItem
    {
        public string ImageUrl { get; set; }
        public string FileName { get; set; }
    }

    public partial class GalleryPage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string year = Request.QueryString["year"] ?? "";
                string eventName = Request.QueryString["event"] ?? "";
                string eventPath = Path.Combine(GalleryConfig.PhotoRoot, year, eventName);
                lblTitle.Text = Server.HtmlEncode(year + " - " + eventName);
                List<PhotoItem> items = new List<PhotoItem>();
                if (Directory.Exists(eventPath))
                {
                    foreach (string file in FolderScanner.GetPhotos(eventPath))
                    {
                        items.Add(new PhotoItem {
                            FileName = Path.GetFileName(file),
                            ImageUrl = GalleryConfig.ToImageUrl(FolderScanner.MakeRelative(file))
                        });
                    }
                }
                rptPhotos.DataSource = items;
                rptPhotos.DataBind();
            }
        }
    }
}
