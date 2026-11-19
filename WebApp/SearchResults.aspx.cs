using System;
using System.Linq;
using System.Web.UI;

namespace SmartPhotoGallery
{
    public partial class SearchResultsPage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string q = (Request.QueryString["q"] ?? "").Trim();
                lblQuery.Text = "Search: " + Server.HtmlEncode(q);
                var results = FolderScanner.GetYears().SelectMany(y => FolderScanner.GetEvents(y.YearName))
                    .Where(x => x.EventName.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                rptResults.DataSource = results;
                rptResults.DataBind();
            }
        }
    }
}
