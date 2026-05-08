using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;


namespace ITSM
{
    public partial class RequestForm : Page
    {
        DBTools db = new DBTools();
        CookieTools cookie = new CookieTools();
        CheckTools chk = new CheckTools();
        TranslationTools trn = new TranslationTools();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["User"] == null && string.IsNullOrEmpty(cookie.Oku("User")))
                {
                    Response.Redirect("Login.aspx");
                    return;
                }

                if (!IsPostBack)
                {

                    txtTopic.Attributes["placeholder"] = gettext("brfsumis", "Brief summary of the issue");
                    txtDescription.Attributes["placeholder"] = gettext("detexp", "Detailed explanation...");
                    txtRequestDate.Text = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
                    string userType = (cookie.Oku("UserType") ?? "").Trim().ToLowerInvariant();

                    pnlRequestOwner.Visible = (userType == "admin");

                    LoadDevices();
                    if (pnlRequestOwner.Visible)
                        LoadRequestOwners();
                    LoadDCUsers();

                }
            }
            catch (Exception ex)
            {

                Response.Write(ex.ToString());
            }

        }

        public string gettext(string tag, string fallback)
        {
            return trn.GetText(tag, fallback, Session["Lang"].ToString()).ToString();
        }
        private void LoadDevices()
        {
            DataTable dt = db.SqlToDt("SELECT DeviceID, (Brand + ' ' + Model + ' (' + SerialNumber + ')') as DeviceInfo FROM device WHERE (isDeleted IS NULL OR isDeleted != 'X')");

            ddlDevices.DataSource = dt;
            ddlDevices.DataTextField = "DeviceInfo";
            ddlDevices.DataValueField = "DeviceID";
            ddlDevices.DataBind();
            ddlDevices.Items.Insert(0, new System.Web.UI.WebControls.ListItem(gettext("seldev", "-- Select Device --"), "0"));
        }

        private void LoadRequestOwners()
        {
            string sql = "SELECT UserID, " + Environment.NewLine +
                         "(ISNULL(Department, 'Belirtilmemiş') + ' - ' + ISNULL(NameSurname, '')) as FullName " + Environment.NewLine +
                         "FROM Users " + Environment.NewLine +
                         "WHERE (isDeleted IS NULL OR isDeleted != 'X') " + Environment.NewLine +
                         "ORDER BY ISNULL(Department, 'ZZZ') ASC, NameSurname ASC";

            DataTable dt = db.SqlToDt(sql);

            ddlRequestOwner.DataSource = dt;
            ddlRequestOwner.DataTextField = "FullName";
            ddlRequestOwner.DataValueField = "UserID";
            ddlRequestOwner.DataBind();
            ddlRequestOwner.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Select Owner --", "0"));
        }


        private void LoadDCUsers()
        {
            try
            {

                string userSession = Session["User"] != null ? Session["User"].ToString() : cookie.Oku("User");
                string sql1 = "SELECT Department FROM Users WHERE UserName = N'" + userSession + "'";



                DataTable dt = db.SqlToDt(sql1);

                string bolum = dt.Rows[0]["Department"].ToString();




                string sql = "SELECT NameSurname,Username FROM Users WHERE Department = '" + bolum + "'";
                db.Sql2AddCombo(ddlUsers, sql, "0", "NameSurname", "Username", true, "Seçiniz");
            }
            catch (Exception ex)
            {

                Response.Write(ex.ToString());
            }
        }

        protected void btnLoadSuggestions_Click(object sender, EventArgs e)
        {
            try
            {
                string fullText = (txtTopic.Text + " " + txtDescription.Text).Trim();

                if (string.IsNullOrWhiteSpace(fullText) || fullText.Length < 10)
                {
                    rptSuggestions.DataSource = null;
                    rptSuggestions.DataBind();
                    pnlSuggestions.Visible = false;
                    updSuggestions.Update();
                    return;
                }

                string search = CheckTools.BuildFullTextSearch(fullText);

                if (string.IsNullOrWhiteSpace(search))
                {
                    rptSuggestions.DataSource = null;
                    rptSuggestions.DataBind();
                    pnlSuggestions.Visible = false;
                    updSuggestions.Update();
                    return;
                }

                string sql = "EXEC ITSM.dbo.usp_GetRequestSuggestions @SearchQuery";

                Dictionary<string, object> prms = new Dictionary<string, object>();
                prms.Add("@SearchQuery", search);

                DataTable dt = db.GetDataTableWithParams(sql, prms);

                if (dt != null && dt.Rows.Count > 0)
                {
                    if (!dt.Columns.Contains("ShortDescription"))
                        dt.Columns.Add("ShortDescription", typeof(string));

                    foreach (DataRow row in dt.Rows)
                    {
                        string desc = row["Description"] == DBNull.Value ? "" : row["Description"].ToString();
                        row["ShortDescription"] = desc.Length > 150 ? desc.Substring(0, 150) + "..." : desc;
                    }

                    rptSuggestions.DataSource = dt;
                    rptSuggestions.DataBind();
                    pnlSuggestions.Visible = true;
                }
                else
                {
                    rptSuggestions.DataSource = null;
                    rptSuggestions.DataBind();
                    pnlSuggestions.Visible = false;
                }

                updSuggestions.Update();
            }
            catch (Exception ex)
            {
                rptSuggestions.DataSource = null;
                rptSuggestions.DataBind();
                pnlSuggestions.Visible = false;
                updSuggestions.Update();

                ScriptManager.RegisterStartupScript(this, GetType(), "suggErr",
                    "console.log('Suggestion Error: " + chk.temizle(ex.Message).Replace("'", "") + "');", true);
            }
        }
        private void ShowAlert(string type, string title, string message)
        {
            string script = $"Swal.fire('{title}', '{message}', '{type}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", script, true);
        }

        private static string FormatSqlDateTime(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "NULL";
            string s = input.Trim().Replace('T', ' ');
            DateTime dt;
            if (!DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) &&
                !DateTime.TryParse(s, CultureInfo.CurrentCulture, DateTimeStyles.None, out dt) &&
                !DateTime.TryParse(s, CultureInfo.GetCultureInfo("tr-TR"), DateTimeStyles.None, out dt))
                return "NULL";
            return "'" + dt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + "'";
        }

        protected void btnSubmitRequest_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtTopic.Text) || string.IsNullOrWhiteSpace(txtDescription.Text))
                {
                    ShowAlert("warning", gettext("warning", "Warning"), gettext("req_fields", "Topic and Description are required!"));
                    return;
                }

                string userSession = Session["User"] != null ? Session["User"].ToString() : cookie.Oku("User");
                string loggedInId = "0";

                DataTable dtUser = db.SqlToDt("SELECT UserID FROM Users WHERE (Username = '" + chk.temizle(userSession) + "' OR NameSurname = '" + chk.temizle(userSession) + "') AND isDeleted IS NULL");
                if (dtUser.Rows.Count > 0)
                {
                    loggedInId = dtUser.Rows[0]["UserID"].ToString();
                    Session["UserID"] = loggedInId;
                }

                string ownerId = loggedInId;
                if (pnlRequestOwner.Visible && !string.IsNullOrEmpty(ddlRequestOwner.SelectedValue) && ddlRequestOwner.SelectedValue != "0")
                    ownerId = ddlRequestOwner.SelectedValue;

                string topic = chk.temizle(txtTopic.Text).Replace("'", "''");
                string description = chk.temizle(txtDescription.Text).Replace("'", "''");
                string deviceId = (ddlDevices.SelectedIndex <= 0) ? "NULL" : ddlDevices.SelectedValue;
                string relatedUserId = (ddlUsers.SelectedIndex <= 0) ? "NULL" : ddlUsers.SelectedValue;

                string startSql = FormatSqlDateTime(txtRequestDate.Text);

                string sqlRequest = "INSERT INTO Requests (Topic, Description, DeviceID, UserID, RequestDate, RecordDate, RecordUser, isDeleted) " +
                                    "VALUES ('" + topic + "', '" + description + "', " + deviceId + ", " + ownerId + ", " + startSql + " , GETDATE(), '" + chk.temizle(userSession) + "', NULL); " +
                                    "SELECT SCOPE_IDENTITY() as SONID";


                DataTable sonid = db.SqlToDt(sqlRequest);

                if (sonid.Rows.Count > 0 && sonid.Rows[0]["SONID"].ToString() != "0")
                {
                    string requestId = sonid.Rows[0]["SONID"].ToString();


                    if (fuAttachment.HasFiles)
                    {
                        string dateFolderName = DateTime.Now.ToString("yyyy-MM");

                        string folderPath = Server.MapPath("~/UploadedFolders/" + dateFolderName + "/");


                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }

                        foreach (HttpPostedFile postedFile in fuAttachment.PostedFiles)
                        {
                            if (postedFile.ContentLength > 0)
                            {
                                string extension = Path.GetExtension(postedFile.FileName);


                                if (!chk.KabulEdilenUzanti(extension))
                                {
                                    ShowAlert("warning", gettext("invalid_file", "Invalid File"), gettext("file_not_supported", "File type not supported:") + " " + extension);
                                    return;
                                }


                                string originalFileName = Path.GetFileName(postedFile.FileName);

                                string safeFileName = Guid.NewGuid().ToString().Substring(0, 8) + "_" + originalFileName.Replace(" ", "_");


                                string fullPath = Path.Combine(folderPath, safeFileName);
                                postedFile.SaveAs(fullPath);



                                string sqlFile = "INSERT INTO RequestFiles (RequestId, FileName, FileType, FilePath, RecordDate, RecordUser, isDeleted) " +
                                                 "VALUES (" + requestId + ", '" + safeFileName.Replace("'", "''") + "', '" + extension + "', '" + dateFolderName + "', GETDATE(), '" + chk.temizle(userSession) + "', NULL)";

                                db.ExecStr(sqlFile);
                            }
                        }
                    }



                    try
                    {
                        List<string> mailList = new List<string>();

                        // Talebi açan kullanıcı
                        string requesterEmail = cookie.Oku("useremail");
                        if (string.IsNullOrWhiteSpace(requesterEmail) && Session["DCMAIL"] != null)
                            requesterEmail = Session["DCMAIL"].ToString();

                        if (!string.IsNullOrWhiteSpace(requesterEmail))
                            mailList.Add(requesterEmail.Trim());


                        if (relatedUserId != "NULL")
                        {


                            ADTools ad = new ADTools();
                            string ccemail = ad.GetEmail(relatedUserId);
                            mailList.Add(ccemail);

                        }


                        List<string> finalMailList = new List<string>();

                        foreach (string mail in mailList)
                        {
                            if (!string.IsNullOrWhiteSpace(mail))
                            {
                                string temizMail = mail.Trim();

                                bool exists = false;
                                foreach (string existingMail in finalMailList)
                                {
                                    if (string.Equals(existingMail, temizMail, StringComparison.OrdinalIgnoreCase))
                                    {
                                        exists = true;
                                        break;
                                    }
                                }

                                if (!exists)
                                    finalMailList.Add(temizMail);
                            }
                        }

                        if (finalMailList.Count > 0)
                        {

                        }
                    }
                    catch (Exception exMail)
                    {
                        System.Diagnostics.Debug.WriteLine("Mail hazırlama hatası: " + exMail.Message);
                    }

                    string successTitle = gettext("success", "Success");
                    string successMsg = gettext("save_success", "Your request has been saved.");

                    string script = $"Swal.fire('{successTitle}', '{successMsg}', 'success').then(() => {{ window.location.href='Requests.aspx'; }});";
                    ScriptManager.RegisterStartupScript(this, GetType(), "successRedirect", script, true);
                }
                else
                {
                    ShowAlert("error", gettext("error", "Error"), gettext("db_error", "Database error:") + " " + chk.temizle(db.Hata));
                }
            }
            catch (Exception ex)
            {
                ShowAlert("error", gettext("error", "Error"), chk.temizle(ex.Message));
            }
        }


        protected void ddlRequestOwner_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlRequestOwner.SelectedIndex > 0)
            {
                string sql1 = "SELECT Department FROM ITSM.dbo.Users WHERE UserID = '" + ddlRequestOwner.SelectedValue + "'";


                DataTable dt = db.SqlToDt(sql1);

                string bolum = dt.Rows[0]["Department"].ToString();


                string sql = "SELECT NameSurname,Username FROM ITSM.dbo.Users WHERE Department = N'" + bolum + "'";
                db.Sql2AddCombo(ddlUsers, sql, "0", "NameSurname", "Username", true, "Seçiniz");
            }

        }


    }
}