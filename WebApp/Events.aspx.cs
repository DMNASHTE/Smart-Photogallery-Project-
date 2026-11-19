using System;
using System.Web.UI;

namespace SmartPhotoGallery
{
    public partial class EventsPage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string year = Request.QueryString["year"] ?? "";
                lblYear.Text = Server.HtmlEncode(year);
                rptEvents.DataSource = FolderScanner.GetEvents(year);
                rptEvents.DataBind();
            }
        }
    }
}
