
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;

namespace SmartPhotoGallery
{
    public partial class DefaultPage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Keep year cards available after every search.
            rptYears.DataSource = FolderScanner.GetYears();
            rptYears.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string query = txtSearch.Text.Trim();
            string searchType = ddlSearchType.SelectedValue;

            pnlPersonResults.Visible = false;
            lblMessage.Text = "";

            if (String.IsNullOrWhiteSpace(query))
            {
                lblMessage.Text = "Please enter something to search.";
                return;
            }

            // Existing event search is preserved.
            if (searchType == "Event")
            {
                Response.Redirect(
                    "SearchResults.aspx?q=" + Server.UrlEncode(query));
                return;
            }

            // Person name and designation use the same API.
            SearchPersonPhotos(query);
        }

        private void SearchPersonPhotos(string query)
        {
            try
            {
                string apiUrl =
                    GalleryConfig.FaceApiUrl.TrimEnd('/')
                    + "/search-person";

                JavaScriptSerializer serializer =
                    new JavaScriptSerializer();

                serializer.MaxJsonLength = Int32.MaxValue;

                Dictionary<string, string> requestData =
                    new Dictionary<string, string>();

                requestData.Add("query", query);

                string jsonRequest =
                    serializer.Serialize(requestData);

                byte[] requestBytes =
                    Encoding.UTF8.GetBytes(jsonRequest);

                HttpWebRequest request =
                    (HttpWebRequest)WebRequest.Create(apiUrl);

                request.Method = "POST";
                request.ContentType = "application/json";
                request.Accept = "application/json";
                request.Timeout = 30000;
                request.ReadWriteTimeout = 30000;
                request.ContentLength = requestBytes.Length;

                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(
                        requestBytes, 0, requestBytes.Length);
                }

                string jsonResponse;

                using (HttpWebResponse response =
                    (HttpWebResponse)request.GetResponse())
                using (StreamReader reader =
                    new StreamReader(response.GetResponseStream()))
                {
                    jsonResponse = reader.ReadToEnd();
                }

                FaceSearchApiResponse apiResult =
                    serializer.Deserialize<FaceSearchApiResponse>(
                        jsonResponse);

                if (apiResult == null || apiResult.results == null)
                {
                    lblMessage.Text = "No results received from face search.";
                    return;
                }

                List<FaceSearchPhoto> photos = apiResult.results;

                foreach (FaceSearchPhoto photo in photos)
                {
                    photo.ImageUrl =
                        GalleryConfig.ToImageUrl(photo.RelativePath);
                }

                if (photos.Count == 0)
                {
                    lblMessage.Text =
                        "No matching photos found for: "
                        + Server.HtmlEncode(query);

                    return;
                }

                rptPersonResults.DataSource = photos;
                rptPersonResults.DataBind();

                pnlPersonResults.Visible = true;

                lblMessage.Text =
                    photos.Count + " matching photo records found. "
                    + "AI matches should be reviewed before confirming identities.";
            }
            catch (WebException ex)
            {
                string errorMessage = ex.Message;

                if (ex.Response != null)
                {
                    using (Stream responseStream =
                        ex.Response.GetResponseStream())
                    {
                        if (responseStream != null)
                        {
                            using (StreamReader reader =
                                new StreamReader(responseStream))
                            {
                                errorMessage = reader.ReadToEnd();
                            }
                        }
                    }
                }

                lblMessage.Text =
                    "Face search service error: "
                    + Server.HtmlEncode(errorMessage);
            }
            catch (Exception ex)
            {
                lblMessage.Text =
                    "Search error: "
                    + Server.HtmlEncode(ex.Message);
            }
        }
    }

    public class FaceSearchApiResponse
    {
        public string status { get; set; }
        public int total_photos { get; set; }
        public string query { get; set; }

        public List<FaceSearchPhoto> results { get; set; }
    }

    public class FaceSearchPhoto
    {
        public int PersonId { get; set; }
        public string PersonName { get; set; }
        public string Designation { get; set; }

        public int PhotoId { get; set; }
        public string FileName { get; set; }
        public string RelativePath { get; set; }

        public string YearName { get; set; }
        public string EventName { get; set; }

        public double Similarity { get; set; }
        public int Reviewed { get; set; }

        public string ImageUrl { get; set; }

        public double SimilarityPercent
        {
            get
            {
                return Math.Round(Similarity * 100, 1);
            }
        }
    }
}
