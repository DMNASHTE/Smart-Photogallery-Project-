using System;
using System.Web.UI;

namespace SmartPhotoGallery
{
    public partial class DefaultPage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                rptYears.DataSource = FolderScanner.GetYears();
                rptYears.DataBind();
            }
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            Response.Redirect("SearchResults.aspx?q=" + Server.UrlEncode(txtSearch.Text.Trim()));
        }
    }
}
