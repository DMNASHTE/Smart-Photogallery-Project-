
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;

namespace SmartPhotoGallery
{
    public partial class PersonManagementPage : Page
    {
        // ==========================================
        // PERSON MODEL
        // ==========================================

        public class PersonRow
        {
            public int PersonId { get; set; }

            public string PersonName { get; set; }

            public string Designation { get; set; }

            public string CreatedAt { get; set; }

            public int MatchedPhotos { get; set; }
        }

        // ==========================================
        // PYTHON API URL
        // ==========================================

        private string FaceApiUrl
        {
            get
            {
                string url =
                    ConfigurationManager.AppSettings["FaceApiUrl"];

                if (String.IsNullOrWhiteSpace(url))
                {
                    throw new Exception(
                        "FaceApiUrl is missing in Web.config.");
                }

                return url.TrimEnd('/');
            }
        }

        // ==========================================
        // PAGE LOAD
        // ==========================================

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CheckConnectionStatus();
                LoadRegisteredPersons();
            }
        }

        // ==========================================
        // CHECK PYTHON AND MYSQL CONNECTION
        // ==========================================

        private void CheckConnectionStatus()
        {
            try
            {
                string responseText =
                    SendGetRequest(
                        FaceApiUrl + "/health", 5000);

                JavaScriptSerializer serializer =
                    new JavaScriptSerializer();

                Dictionary<string, object> result =
                    serializer.Deserialize
                    <Dictionary<string, object>>(responseText);

                lblPythonStatus.Text = "Connected";

                lblPythonStatus.ForeColor =
                    System.Drawing.Color.Green;

                string dbStatus =
                    result.ContainsKey("database_status")
                    ? Convert.ToString(result["database_status"])
                    : "Unknown";

                if (dbStatus == "connected")
                {
                    lblDatabaseStatus.Text = "Connected";

                    lblDatabaseStatus.ForeColor =
                        System.Drawing.Color.Green;
                }
                else
                {
                    lblDatabaseStatus.Text = "Disconnected";

                    lblDatabaseStatus.ForeColor =
                        System.Drawing.Color.Red;
                }

                lblDatabaseName.Text =
                    result.ContainsKey("database_name")
                    ? Convert.ToString(result["database_name"])
                    : "Unknown";

                bool modelsLoaded =
                    result.ContainsKey("models_loaded") &&
                    Convert.ToBoolean(result["models_loaded"]);

                lblModelStatus.Text =
                    modelsLoaded ? "Loaded" : "Not loaded";

                lblModelStatus.ForeColor =
                    modelsLoaded
                    ? System.Drawing.Color.Green
                    : System.Drawing.Color.Gray;
            }
            catch
            {
                lblPythonStatus.Text = "Disconnected";

                lblPythonStatus.ForeColor =
                    System.Drawing.Color.Red;

                lblDatabaseStatus.Text = "Not verified";

                lblDatabaseStatus.ForeColor =
                    System.Drawing.Color.Red;

                lblDatabaseName.Text = "Not available";

                lblModelStatus.Text = "Not available";
            }
        }

        // ==========================================
        // COMMON GET REQUEST
        // ==========================================

        private string SendGetRequest(
            string url, int timeout)
        {
            HttpWebRequest request =
                (HttpWebRequest)WebRequest.Create(url);

            request.Method = "GET";
            request.Timeout = timeout;
            request.ReadWriteTimeout = timeout;

            using (HttpWebResponse response =
                (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader reader =
                    new StreamReader(response.GetResponseStream()))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        // ==========================================
        // LOAD REGISTERED PEOPLE
        // ==========================================


        private void LoadRegisteredPersons()
        {
            try
            {
                string responseText =
                    SendGetRequest(FaceApiUrl + "/persons", 15000);

                JavaScriptSerializer serializer =
                    new JavaScriptSerializer();

                Dictionary<string, object> result =
                    serializer.Deserialize<Dictionary<string, object>>(
                        responseText);

                if (!result.ContainsKey("status") ||
                    Convert.ToString(result["status"]) != "success")
                {
                    throw new Exception("Unable to load registered people.");
                }

                // IMPORTANT: JSON ARRAY IS DESERIALIZED AS ArrayList
                ArrayList apiPersons =
                    result["persons"] as ArrayList;

                List<PersonRow> persons = new List<PersonRow>();

                if (apiPersons != null)
                {
                    foreach (object item in apiPersons)
                    {
                        Dictionary<string, object> person =
                            item as Dictionary<string, object>;

                        if (person == null)
                            continue;

                        PersonRow row = new PersonRow();

                        row.PersonId =
                            Convert.ToInt32(person["PersonId"]);

                        row.PersonName =
                            Convert.ToString(person["PersonName"]);

                        row.Designation =
                            person["Designation"] == null
                            ? ""
                            : Convert.ToString(person["Designation"]);

                        row.CreatedAt =
                            person["CreatedAt"] == null
                            ? ""
                            : Convert.ToString(person["CreatedAt"]);

                        row.MatchedPhotos =
                            Convert.ToInt32(person["MatchedPhotos"]);

                        persons.Add(row);
                    }
                }

                rptPersons.DataSource = persons;
                rptPersons.DataBind();

                lblTotalPersons.Text =
                    "Total Registered People: " + persons.Count;

                lblPeopleMessage.Text =
                    persons.Count == 0
                    ? "No registered people found."
                    : "";

                lblPeopleMessage.ForeColor =
                    System.Drawing.Color.Gray;
            }
            catch (Exception ex)
            {
                rptPersons.DataSource = null;
                rptPersons.DataBind();

                lblTotalPersons.Text =
                    "Total Registered People: Not available";

                lblPeopleMessage.Text =
                    "Error loading people: " +
                    Server.HtmlEncode(ex.Message);

                lblPeopleMessage.ForeColor =
                    System.Drawing.Color.Red;
            }
        }


        // ==========================================
        // PHOTO VALIDATION
        // ==========================================

        protected void cvPhoto_ServerValidate(
            object source,
            ServerValidateEventArgs args)
        {
            args.IsValid = fuReferencePhoto.HasFile;
        }

        // ==========================================
        // REGISTER PERSON
        // ==========================================

        protected void btnRegister_Click(
            object sender, EventArgs e)
        {
            lblStatus.Text = "";

            Page.Validate();

            if (!Page.IsValid)
                return;

            string personName =
                txtPersonName.Text.Trim();

            string designation =
                txtDesignation.Text.Trim();

            if (String.IsNullOrWhiteSpace(personName))
            {
                ShowStatus(
                    "Please enter person name.", false);
                return;
            }

            if (personName.Length > 200 ||
                designation.Length > 200)
            {
                ShowStatus(
                    "Name and designation must not exceed 200 characters.",
                    false);
                return;
            }

            if (!fuReferencePhoto.HasFile)
            {
                ShowStatus(
                    "Please select a reference photo.", false);
                return;
            }

            if (fuReferencePhoto.PostedFile.ContentLength <= 0)
            {
                ShowStatus(
                    "The selected photo is empty.", false);
                return;
            }

            if (fuReferencePhoto.PostedFile.ContentLength >
                10 * 1024 * 1024)
            {
                ShowStatus(
                    "Photo size must be below 10 MB.", false);
                return;
            }

            string extension =
                Path.GetExtension(
                    fuReferencePhoto.FileName).ToLowerInvariant();

            string[] allowedExtensions =
                { ".jpg", ".jpeg", ".png", ".bmp" };

            if (Array.IndexOf(
                allowedExtensions, extension) < 0)
            {
                ShowStatus(
                    "Only JPG, JPEG, PNG and BMP files are allowed.",
                    false);
                return;
            }

            string photoDirectory =
                Server.MapPath(
                    "~/App_Data/PersonReferencePhotos/");

            string savedPhotoPath = "";

            bool registrationSucceeded = false;

            try
            {
                if (!Directory.Exists(photoDirectory))
                {
                    Directory.CreateDirectory(photoDirectory);
                }

                string fileName =
                    Guid.NewGuid().ToString("N") + extension;

                savedPhotoPath =
                    Path.Combine(photoDirectory, fileName);

                fuReferencePhoto.SaveAs(savedPhotoPath);

                // PREPARE PYTHON REGISTER API REQUEST

                Dictionary<string, object> requestData =
                    new Dictionary<string, object>();

                requestData["person_name"] = personName;

                requestData["designation"] = designation;

                requestData["image_path"] = savedPhotoPath;

                JavaScriptSerializer serializer =
                    new JavaScriptSerializer();

                string json =
                    serializer.Serialize(requestData);

                byte[] data =
                    Encoding.UTF8.GetBytes(json);

                HttpWebRequest request =
                    (HttpWebRequest)WebRequest.Create(
                        FaceApiUrl + "/register");

                request.Method = "POST";

                request.ContentType = "application/json";

                request.ContentLength = data.Length;

                request.Timeout = 120000;

                request.ReadWriteTimeout = 120000;

                using (Stream stream =
                    request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                string responseText = "";

                using (HttpWebResponse response =
                    (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader =
                        new StreamReader(response.GetResponseStream()))
                    {
                        responseText = reader.ReadToEnd();
                    }
                }

                Dictionary<string, object> result =
                    serializer.Deserialize
                    <Dictionary<string, object>>(responseText);

                if (result.ContainsKey("status") &&
                    Convert.ToString(result["status"]) == "success")
                {
                    registrationSucceeded = true;

                    string personId =
                        result.ContainsKey("person_id")
                        ? Convert.ToString(result["person_id"])
                        : "Not returned";

                    ShowStatus(
                        "Person registered successfully! Person ID: " +
                        personId +
                        ". Person details and face embedding saved in MySQL.",
                        true);

                    txtPersonName.Text = "";

                    txtDesignation.Text = "";

                    // REFRESH PEOPLE LIST
                    LoadRegisteredPersons();
                }
                else
                {
                    string errorMessage =
                        result.ContainsKey("error")
                        ? Convert.ToString(result["error"])
                        : "Registration failed.";

                    ShowStatus(errorMessage, false);
                }
            }
            catch (WebException ex)
            {
                string errorMessage = ex.Message;

                if (ex.Response != null)
                {
                    using (StreamReader reader =
                        new StreamReader(
                            ex.Response.GetResponseStream()))
                    {
                        errorMessage = reader.ReadToEnd();
                    }
                }

                ShowStatus(
                    "AI service error: " + errorMessage,
                    false);
            }
            catch (Exception ex)
            {
                ShowStatus(
                    "Registration error: " + ex.Message,
                    false);
            }
            finally
            {
                // REMOVE FILE IF REGISTRATION FAILED
                if (!registrationSucceeded &&
                    !String.IsNullOrEmpty(savedPhotoPath) &&
                    File.Exists(savedPhotoPath))
                {
                    try
                    {
                        File.Delete(savedPhotoPath);
                    }
                    catch
                    {
                        // Preserve the original error.
                    }
                }
            }
        }

        // ==========================================
        // DISPLAY STATUS MESSAGE
        // ==========================================

        private void ShowStatus(
            string message, bool success)
        {
            lblStatus.Text =
                Server.HtmlEncode(message);

            lblStatus.ForeColor =
                success
                ? System.Drawing.Color.Green
                : System.Drawing.Color.Red;
        }
    }
}
