
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
    public partial class FaceSearchPage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string query = txtSearch.Text.Trim();

            pnlResults.Visible = false;
            rptResults.DataSource = null;
            rptResults.DataBind();

            if (String.IsNullOrWhiteSpace(query))
            {
                lblStatus.Text = "Please enter a person name or designation.";
                return;
            }

            try
            {
                string apiUrl = GalleryConfig.FaceApiUrl
                    + "/search-person";

                JavaScriptSerializer serializer =
                    new JavaScriptSerializer();

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

                serializer.MaxJsonLength = Int32.MaxValue;

                FaceSearchApiResponse apiResult =
                    serializer.Deserialize<FaceSearchApiResponse>(
                        jsonResponse);

                if (apiResult == null || apiResult.results == null)
                {
                    lblStatus.Text = "No search results received.";
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
                    lblStatus.Text =
                        "No matching photos found for: "
                        + Server.HtmlEncode(query);

                    return;
                }

                rptResults.DataSource = photos;
                rptResults.DataBind();

                pnlResults.Visible = true;

                lblStatus.Text =
                    photos.Count
                    + " matching photo records found. "
                    + "Please review AI matches before confirming identities.";
            }
            catch (WebException ex)
            {
                string errorMessage = ex.Message;

                if (ex.Response != null)
                {
                    using (StreamReader reader =
                        new StreamReader(ex.Response.GetResponseStream()))
                    {
                        errorMessage = reader.ReadToEnd();
                    }
                }

                lblStatus.Text =
                    "Face search service error: "
                    + Server.HtmlEncode(errorMessage);
            }
            catch (Exception ex)
            {
                lblStatus.Text =
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
