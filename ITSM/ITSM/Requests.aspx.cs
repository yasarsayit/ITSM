using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ITSM
{
    public partial class Requests : Page
    {
        DBTools db = new DBTools();
        CookieTools cookie = new CookieTools();
        CheckTools chk = new CheckTools();
        TranslationTools trn = new TranslationTools();

        public int CurrentPage
        {
            get { return ViewState["CurrentPage"] != null ? (int)ViewState["CurrentPage"] : 1; }
            set { ViewState["CurrentPage"] = value; }
        }

        public int TotalPages
        {
            get { return ViewState["TotalPages"] != null ? (int)ViewState["TotalPages"] : 1; }
            set { ViewState["TotalPages"] = value; }
        }

        public string FilterReqId { get { return ViewState["F_ReqId"] as string ?? ""; } set { ViewState["F_ReqId"] = value; } }
        public string FilterTopic { get { return ViewState["F_Topic"] as string ?? ""; } set { ViewState["F_Topic"] = value; } }
        public string FilterDate { get { return ViewState["F_Date"] as string ?? ""; } set { ViewState["F_Date"] = value; } }
        public string FilterStatus { get { return ViewState["F_Status"] as string ?? ""; } set { ViewState["F_Status"] = value; } }


        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["User"] == null && string.IsNullOrEmpty(cookie.Oku("User")))
                {
                    Response.Redirect("Login.aspx");
                    return;
                }


                if (Session["Lang"] == null)
                {
                    Session["Lang"] = trn.GetLangCode();
                }

                if (!IsPostBack)
                {


                    if (ddlFilterStatus.Items.FindByValue("") != null)
                        ddlFilterStatus.Items.FindByValue("").Text = gettext("statusall", "All");

                    if (ddlFilterStatus.Items.FindByValue("0") != null)
                        ddlFilterStatus.Items.FindByValue("0").Text = gettext("statuspending", "Pending");

                    if (ddlFilterStatus.Items.FindByValue("1") != null)
                        ddlFilterStatus.Items.FindByValue("1").Text = gettext("statusapproved", "Approved");

                    if (ddlFilterStatus.Items.FindByValue("2") != null)
                        ddlFilterStatus.Items.FindByValue("2").Text = gettext("statusrejected", "Rejected");

                    if (ddlFilterStatus.Items.FindByValue("3") != null)
                        ddlFilterStatus.Items.FindByValue("3").Text = gettext("statuscompleted", "Completed");


                    if (ddlQualityScore.Items.FindByValue("-1") != null)
                        ddlQualityScore.Items.FindByValue("-1").Text = gettext("score_select", "-- Select Score --");

                    if (ddlQualityScore.Items.FindByValue("0") != null)
                        ddlQualityScore.Items.FindByValue("0").Text = gettext("score_0", "0 - Far below expectations");

                    if (ddlQualityScore.Items.FindByValue("1") != null)
                        ddlQualityScore.Items.FindByValue("1").Text = gettext("score_1", "1 - Below expectations");

                    if (ddlQualityScore.Items.FindByValue("2") != null)
                        ddlQualityScore.Items.FindByValue("2").Text = gettext("score_2", "2 - Met expectations");

                    if (ddlQualityScore.Items.FindByValue("3") != null)
                        ddlQualityScore.Items.FindByValue("3").Text = gettext("score_3", "3 - Above expectations");


                    txtFilterReqId.Attributes["placeholder"] = gettext("search_id", "ID...");
                    txtFilterTopic.Attributes["placeholder"] = gettext("searchtopic", "Search Topic...");
                    txtFilterDate.Attributes["placeholder"] = gettext("searchdate", "YYYY-MM-DD");
                    txtFilterDate.Attributes["placeholder"] = gettext("searchdate", "YYYY-MM-DD");
                    txtNewMessage.Attributes["placeholder"] = gettext("type_message", "Type your message here...");

                    lbAraTemizle.ToolTip = gettext("clearfilters", "Clear Filters");
                    lbAra.ToolTip = gettext("search", "Search");

                    CurrentPage = 1;
                    ResetFilters();
                    LoadRequests();
                    LoadKnowledgeBaseTopics();

                    if (Request.QueryString["op"] == "deleted")
                    {
                        ShowAlert("success", gettext("success_title", "Success"), gettext("req_deleted_msg", "The request was successfully deleted."), "");
                        string cleanScript = "window.history.replaceState({}, document.title, 'Requests.aspx');";
                        ClientScript.RegisterStartupScript(this.GetType(), "cleanUrl", cleanScript, true);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(hfChatRequestId.Value))
                    {
                        LoadChat(hfChatRequestId.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error", "Swal.fire('" + gettext("error_title", "Error") + "', '" + ex.Message.Replace("'", "") + "', 'error');", true);
            }
        }
        public string gettext(string tag, string fallback)
        {
            return trn.GetText(tag, fallback, Session["Lang"].ToString()).ToString();
        }


        private void ResetFilters()
        {
            FilterReqId = ""; FilterTopic = ""; FilterDate = ""; FilterStatus = "";
        }

        private void LoadRequests()
        {
            try
            {
                string userSession = Session["User"]?.ToString() ?? cookie.Oku("User");
                string safeUser = chk.temizle(userSession);

                int pageSize = Convert.ToInt32(ddlPageSize.SelectedValue);
                int offset = (CurrentPage - 1) * pageSize;


                string filterCondition = " AND RecordUser = '" + safeUser + "' ";


                if (!string.IsNullOrEmpty(FilterReqId)) filterCondition += " AND RequestId LIKE '%" + chk.temizle(FilterReqId) + "%' ";
                if (!string.IsNullOrEmpty(FilterTopic)) filterCondition += " AND Topic LIKE '%" + chk.temizle(FilterTopic) + "%' ";
                if (!string.IsNullOrEmpty(FilterDate)) filterCondition += " AND CONVERT(varchar, RequestDate, 23) LIKE '%" + chk.temizle(FilterDate) + "%' ";
                if (!string.IsNullOrEmpty(FilterStatus))
                {
                    if (FilterStatus == "0")
                        filterCondition += " AND (IsConfirmed IS NULL OR IsConfirmed = 0) ";
                    else
                        filterCondition += " AND IsConfirmed = " + chk.temizle(FilterStatus) + " ";
                }

                string countSql = "SELECT COUNT(RequestId) FROM Requests WHERE (isDeleted IS NULL OR isDeleted != 'X') " + filterCondition;
                DataTable dtCount = db.SqlToDt(countSql);
                int totalRecords = dtCount.Rows.Count > 0 ? Convert.ToInt32(dtCount.Rows[0][0]) : 0;

                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
                if (TotalPages == 0) TotalPages = 1;

                if (CurrentPage > TotalPages) CurrentPage = TotalPages;
                offset = (CurrentPage - 1) * pageSize;


                string sql = "SELECT RequestId, Topic, RequestDate, RecordUser, IsConfirmed, IsImportant, IsUrgent, Score " +
             "FROM Requests " +
                             "WHERE (isDeleted IS NULL OR isDeleted != 'X') " + filterCondition +
                             "ORDER BY RequestId DESC " +
                             "OFFSET " + offset + " ROWS FETCH NEXT " + pageSize + " ROWS ONLY";

                DataTable dt = db.SqlToDt(sql);

                if (dt != null && dt.Rows.Count > 0)
                {
                    rptRequests.DataSource = dt;
                    rptRequests.DataBind();
                    rptRequests.Visible = true;
                    trNoData.Visible = false;
                }
                else
                {
                    rptRequests.DataSource = null;
                    rptRequests.DataBind();
                    rptRequests.Visible = false;
                    trNoData.Visible = true;
                }

                SetupPagination(totalRecords, pageSize);
            }
            catch (Exception ex)
            {
                ShowAlert("error",
          gettext("error_title", "Error"),
          gettext("load_req_error", "Could not load requests: ") + ex.Message.Replace("'", ""),
          "");
            }
        }

        protected void lbAra_Click(object sender, EventArgs e)
        {
            FilterReqId = txtFilterReqId.Text.Trim();
            FilterTopic = txtFilterTopic.Text.Trim();
            FilterDate = txtFilterDate.Text.Trim();
            FilterStatus = ddlFilterStatus.SelectedValue;

            CurrentPage = 1;
            LoadRequests();
        }
        protected string GetScoreBadge(object score, object isConfirmed)
        {
            string status = isConfirmed != DBNull.Value ? isConfirmed.ToString() : "0";


            if (score == null || score == DBNull.Value || string.IsNullOrEmpty(score.ToString()))
            {

                if (status == "3")
                    return $"<span class='badge bg-light text-muted border' style='font-size:10px;' title='{gettext("not_rated", "Not Rated")}'>{gettext("not_rated", "Not Rated")}</span>";


                return "<span class='text-muted' style='font-size:11px;'>-</span>";
            }


            string s = score.ToString();
            if (s == "0") return "<span class='badge bg-danger' title='0 - Far below expectations'><i class='fas fa-star text-white'></i> 0</span>";
            if (s == "1") return "<span class='badge bg-warning text-dark' title='1 - Below expectations'><i class='fas fa-star text-dark'></i> 1</span>";
            if (s == "2") return "<span class='badge bg-primary' title='2 - Met expectations'><i class='fas fa-star text-white'></i> 2</span>";
            if (s == "3") return "<span class='badge bg-success' title='3 - Above expectations'><i class='fas fa-star text-white'></i> 3</span>";

            return "<span class='badge bg-secondary'>" + s + "</span>";
        }
        protected void lbAraTemizle_Click(object sender, EventArgs e)
        {
            txtFilterReqId.Text = "";
            txtFilterTopic.Text = "";
            txtFilterDate.Text = "";
            ddlFilterStatus.SelectedIndex = 0;

            ResetFilters();
            CurrentPage = 1;
            LoadRequests();
        }


        private void SetupPagination(int totalRecords, int pageSize)
        {
            int startRecord = ((CurrentPage - 1) * pageSize) + 1;
            int endRecord = Math.Min(CurrentPage * pageSize, totalRecords);

            if (totalRecords == 0)
                lblPageInfo.Text = "Showing 0 entries";
            else
                lblPageInfo.Text = $"Showing {startRecord} to {endRecord} of {totalRecords} entries";

            DataTable dtPages = new DataTable();
            dtPages.Columns.Add("PageNumber");
            dtPages.Columns.Add("IsActive", typeof(bool));

            int startPage = Math.Max(1, CurrentPage - 3);
            int endPage = Math.Min(TotalPages, CurrentPage + 3);

            for (int i = startPage; i <= endPage; i++)
            {
                dtPages.Rows.Add(i, i == CurrentPage);
            }

            rptPagination.DataSource = dtPages;
            rptPagination.DataBind();

            liPrev.Attributes["class"] = CurrentPage > 1 ? "page-item" : "page-item disabled";
            liNext.Attributes["class"] = CurrentPage < TotalPages ? "page-item" : "page-item disabled";
        }

        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentPage = 1;
            LoadRequests();
        }

        protected void lbPrev_Click(object sender, EventArgs e)
        {
            if (CurrentPage > 1) { CurrentPage--; LoadRequests(); }
        }

        protected void lbNext_Click(object sender, EventArgs e)
        {
            if (CurrentPage < TotalPages) { CurrentPage++; LoadRequests(); }
        }

        protected void rptPagination_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Page")
            {
                CurrentPage = Convert.ToInt32(e.CommandArgument);
                LoadRequests();
            }
        }


        private void ShowAlert(string type, string title, string message, string redirectUrl = "")
        {
            hfAlertType.Value = type;
            hfAlertTitle.Value = title;
            hfAlertMessage.Value = message;
            hfRedirectUrl.Value = redirectUrl;
        }

        private void LoadKnowledgeBaseTopics()
        {
            try
            {
                string sql = "SELECT TOP 5 KnowledgeID, Title, Summary FROM KnowledgeBase WHERE IsVisible = 1 AND (IsDeleted IS NULL OR IsDeleted != 'X') ORDER BY RecordDate DESC";
                DataTable dt = db.SqlToDt(sql);

                if (dt != null && dt.Rows.Count > 0)
                {
                    rptTopics.DataSource = dt;
                    rptTopics.DataBind();
                }
            }
            catch (Exception) { }
        }

        protected bool IsPending(object isConfirmed)
        {
            if (isConfirmed == null || isConfirmed == DBNull.Value || string.IsNullOrEmpty(isConfirmed.ToString()))
                return true;

            string status = isConfirmed.ToString().Trim();
            if (status == "1" || status == "2" || status == "3")
                return false;

            return true;
        }
        protected void btnNewRequest_Click(object sender, EventArgs e)
        {

            string userSession = Session["User"]?.ToString() ?? cookie.Oku("User");
            string safeUser = chk.temizle(userSession);


            string userType = (Session["UserType"]?.ToString() ?? cookie.Oku("UserType") ?? "").Trim().ToLower();


            if (userType == "admin")
            {
                Response.Redirect("RequestForm.aspx");
                return;
            }


            string sqlCheck = "SELECT TOP 1 RequestId FROM Requests WHERE IsConfirmed = 3 AND Score IS NULL AND RecordUser = '" + safeUser + "' AND (isDeleted IS NULL OR isDeleted != 'X')";
            DataTable dtCheck = db.SqlToDt(sqlCheck);

            if (dtCheck != null && dtCheck.Rows.Count > 0)
            {
                ShowAlert("warning",
          gettext("action_required", "Action Required"),
          gettext("need_score_msg", "You need to rate the quality of completed quantities. Please rate your previous orders before opening a new one."),
          "");
            }
            else
            {
                Response.Redirect("RequestForm.aspx");
            }
        }

        protected void btnDeleteConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                string userSession = Session["User"]?.ToString() ?? cookie.Oku("User");
                if (string.IsNullOrEmpty(userSession)) userSession = "System";

                int delId = Convert.ToInt32(hfDeleteId.Value);

                string sqlCheck = "SELECT IsConfirmed FROM Requests WHERE RequestId = " + delId;
                DataTable dtCheck = db.SqlToDt(sqlCheck);

                if (dtCheck != null && dtCheck.Rows.Count > 0)
                {
                    object isConf = dtCheck.Rows[0]["IsConfirmed"];
                    if (!IsPending(isConf))
                    {
                        ShowAlert("warning", "Warning", "This request is currently in progress by an admin and cannot be deleted.", "");
                        hfDeleteId.Value = "";
                        return;
                    }
                }

                string sqlGetFiles = "SELECT FilePath, FileName FROM RequestFiles WHERE RequestId = " + delId + " AND (IsDeleted IS NULL OR IsDeleted != 'X')";
                DataTable dtFiles = db.SqlToDt(sqlGetFiles);

                if (dtFiles != null && dtFiles.Rows.Count > 0)
                {
                    foreach (DataRow row in dtFiles.Rows)
                    {
                        string folder = row["FilePath"].ToString();
                        string fName = row["FileName"].ToString();
                        string physicalPath = Server.MapPath("~/UploadedFolders/" + folder + "/" + fName);
                        if (File.Exists(physicalPath)) File.Delete(physicalPath);
                    }
                    string sqlDeleteFilesDB = "UPDATE RequestFiles SET IsDeleted = 'X', DeleteDate = GETDATE(), DeleteUser = '" + userSession.Replace("'", "''") + "' WHERE RequestId = " + delId;
                    db.ExecStr(sqlDeleteFilesDB);
                }

                string sqlDelete = "UPDATE Requests SET isDeleted = 'X', DeleteDate = GETDATE(), DeleteUser = '" + userSession.Replace("'", "''") + "' WHERE RequestId = " + delId;

                if (db.ExecStr(sqlDelete) > 0)
                {
                    ShowAlert("success", "Success", "The request and attached files were successfully deleted.", "");
                    Response.Redirect("Requests.aspx?op=deleted");
                    hfDeleteId.Value = "";
                    LoadRequests();
                }
                else
                {
                    ShowAlert("error", "Error", "Delete operation failed.", "");
                }
            }
            catch (Exception ex)
            {
                ShowAlert("error", "System Error", ex.Message.Replace("'", ""), "");
            }
        }

        protected void btnSaveScore_Click(object sender, EventArgs e)
        {
            try
            {
                string userSession = Session["User"] != null ? Session["User"].ToString() : cookie.Oku("User");
                string loggedInId = "0";

                string reqId = hfChatRequestId.Value;
                string score = ddlQualityScore.SelectedValue;
                string scoreDesc = chk.temizle(txtScoreDesc.Text).Replace("'", "''");

                if (string.IsNullOrEmpty(reqId) || score == "-1")
                {
                    ShowAlert("warning", gettext("warning_title", "Warning"), gettext("select_valid_score", "Please select a valid quality score."), "");
                    return;
                }

                if ((score == "0" || score == "1") && string.IsNullOrWhiteSpace(scoreDesc))
                {
                    ShowAlert("warning", gettext("warning_title", "Warning"), gettext("score_desc_required", "Lütfen düşük puan verme nedeninizi açıklayınız. (Please provide a reason for the low score)"), "");
                    return;
                }

                string sql = "UPDATE Requests SET Score = " + chk.temizle(score) + ", ScoreDesc = '" + scoreDesc + "' WHERE RequestId = " + chk.temizle(reqId);

                if (db.ExecStr(sql) > 0)
                {
                    ShowAlert("success", gettext("thank_you", "Thank You!"), gettext("score_saved_msg", "Your quality score has been saved successfully."), "");
                    LoadRequestDetails(reqId);


                    if (score == "0" || score == "1")
                    {


                        DataTable dtUser = db.SqlToDt("SELECT UserID FROM Users WHERE (Username = '" + chk.temizle(userSession) + "' OR NameSurname = '" + chk.temizle(userSession) + "') AND isDeleted IS NULL");
                        if (dtUser.Rows.Count > 0)
                        {
                            loggedInId = dtUser.Rows[0]["UserID"].ToString();
                            Session["UserID"] = loggedInId;
                        }

                        try
                        {

                            string infoSql = @"SELECT r.Topic, r.Description, 
                                       ISNULL(u_owner.NameSurname, r.RecordUser) AS OwnerName, 
                                       u_admin.Email AS AdminEmail 
                                       FROM Requests r 
                                       LEFT JOIN Users u_owner ON r.UserID = u_owner.UserID 
                                       LEFT JOIN Users u_admin ON r.ConfirmedBy = u_admin.UserID 
                                       WHERE r.RequestId = " + chk.temizle(reqId);

                            DataTable dtInfo = db.SqlToDt(infoSql);
                            if (dtInfo != null && dtInfo.Rows.Count > 0)
                            {
                                string topic = dtInfo.Rows[0]["Topic"].ToString();
                                string desc = dtInfo.Rows[0]["Description"].ToString();
                                string ownerName = dtInfo.Rows[0]["OwnerName"].ToString();
                                string adminEmail = dtInfo.Rows[0]["AdminEmail"].ToString();




                                List<string> mailList = new List<string>();


                                string requesterEmail = cookie.Oku("useremail");
                                if (string.IsNullOrWhiteSpace(requesterEmail) && Session["DCMAIL"] != null)
                                    requesterEmail = Session["DCMAIL"].ToString();

                                if (!string.IsNullOrWhiteSpace(requesterEmail))
                                    mailList.Add(requesterEmail.Trim());


                                if (loggedInId != "NULL")
                                {


                                    ADTools ad = new ADTools();
                                    string ccemail = ad.GetEmail(loggedInId);
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
                        }
                        catch (Exception exMail)
                        {
                            System.Diagnostics.Debug.WriteLine("Mail Info Fetch Error: " + exMail.Message);
                        }
                    }

                }
                else
                {
                    ShowAlert("error", gettext("error_title", "Error"), gettext("score_save_error", "Score could not be saved."), "");
                }
            }
            catch (Exception ex)
            {
                ShowAlert("error", "System Error", ex.Message, "");
            }
        }

        protected void rptTopics_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "ShowTopic")
            {
                try
                {
                    string topicId = e.CommandArgument.ToString();
                    string sql = "SELECT Title, Content FROM KnowledgeBase WHERE KnowledgeID = " + topicId;
                    DataTable dt = db.SqlToDt(sql);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        string title = HttpUtility.JavaScriptStringEncode(dt.Rows[0]["Title"].ToString());
                        string content = HttpUtility.JavaScriptStringEncode(dt.Rows[0]["Content"].ToString());

                        string script = "Swal.fire({" +
                                        "title: '<strong>" + title + "</strong>'," +
                                        "html: '" + content + "'," +
                                        "width: '800px'," +
                                        "showCloseButton: true," +
                                        "showConfirmButton: false" +
                                        "});";

                        ScriptManager.RegisterStartupScript(this, GetType(), "showTopicModal", script, true);
                    }
                }
                catch (Exception ex)
                {
                    string safeError = HttpUtility.JavaScriptStringEncode(ex.Message);
                    ScriptManager.RegisterStartupScript(this, GetType(), "error", "Swal.fire('Error', 'Topic could not be loaded: " + safeError + "', 'error');", true);
                }
            }
        }

        protected void rptRequests_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "ViewRequest")
            {
                string reqId = chk.temizle(e.CommandArgument.ToString());
                hfChatRequestId.Value = reqId;
                lblChatReqId.Text = reqId;

                pnlTable.Visible = false;
                pnlChat.Visible = true;

                LoadRequestDetails(reqId);
                LoadChat(reqId);

                ScriptManager.RegisterStartupScript(this, GetType(), "scrollChat", "setTimeout(function() { var div = document.getElementById('userChatContainer'); if(div) div.scrollTop = div.scrollHeight; }, 100);", true);
            }
        }

        private void LoadRequestDetails(string reqId)
        {
            try
            {
                string sqlReq = "SELECT r.Topic, r.Description, r.RequestDate, r.DueDate, r.IsImportant, r.IsUrgent,r.ScoreDesc, r.IsConfirmed, r.Score, " +
                                "(d.Brand + ' ' + d.Model) as DeviceName, " +
                                "rt.Description as TypeName, " +
                                "rst.Description as SubTypeName " +
                                "FROM Requests r " +
                                "LEFT JOIN device d ON r.DeviceID = d.DeviceID " +
                                "LEFT JOIN RequestTypes rt ON r.RequestType = rt.TypeID " +
                                "LEFT JOIN RequestSubTypes rst ON r.RequestSubtype = rst.SubTypeID " +
                                "WHERE r.RequestId = " + reqId;

                DataTable dtReq = db.SqlToDt(sqlReq);
                if (dtReq != null && dtReq.Rows.Count > 0)
                {
                    DataRow row = dtReq.Rows[0];
                    string topic = HttpUtility.HtmlEncode(row["Topic"].ToString());
                    string desc = HttpUtility.HtmlEncode(row["Description"].ToString()).Replace("\n", "<br/>");
                    string reqDate = Convert.ToDateTime(row["RequestDate"]).ToString("dd.MM.yyyy HH:mm");

                    string device = row["DeviceName"] != DBNull.Value
                        ? HttpUtility.HtmlEncode(row["DeviceName"].ToString())
                        : $"<span class='text-muted fst-italic'>{gettext("not_selected", "Not Selected")}</span>";

                    string typeName = row["TypeName"] != DBNull.Value
                        ? HttpUtility.HtmlEncode(row["TypeName"].ToString())
                        : $"<span class='text-muted fst-italic'>{gettext("not_appointed", "Not appointed yet")}</span>";

                    string subTypeName = row["SubTypeName"] != DBNull.Value ? " / <span style='color:#6c757d;'>" + HttpUtility.HtmlEncode(row["SubTypeName"].ToString()) + "</span>" : "";
                    string fullCategory = typeName + subTypeName;

                    string dueDateHtml = row["DueDate"] != DBNull.Value ? $"<div class='mb-2'><strong>Targeted End Date:</strong> <span style='color:#198754; font-weight:bold;'>{Convert.ToDateTime(row["DueDate"]).ToString("dd.MM.yyyy")}</span></div>" : "";

                    string priorityHtml = "";
                    if (row["IsUrgent"] != DBNull.Value && row["IsUrgent"].ToString() == "1") priorityHtml += "<span class='badge bg-danger me-1'>URGENT</span>";
                    if (row["IsImportant"] != DBNull.Value && row["IsImportant"].ToString() == "1") priorityHtml += "<span class='badge bg-warning text-dark'>IMPORTANT</span>";

                    if (!string.IsNullOrEmpty(priorityHtml))
                    {
                        priorityHtml = $"<div class='mb-2'><strong>Priority:</strong> {priorityHtml}</div>";
                    }


                    string sqlFiles = "SELECT FileName, FilePath, FileType FROM RequestFiles WHERE RequestId = " + reqId + " AND (IsDeleted IS NULL OR IsDeleted != 'X')";
                    DataTable dtFiles = db.SqlToDt(sqlFiles);
                    string filesHtml = "<div class='d-flex flex-wrap gap-2 mt-2'>";

                    if (dtFiles != null && dtFiles.Rows.Count > 0)
                    {
                        foreach (DataRow fRow in dtFiles.Rows)
                        {

                            string fName = fRow["FileName"].ToString();

                            string fileUrl = ResolveUrl("~/UploadedFolders/" + fRow["FilePath"].ToString() + "/" + fName);

                            filesHtml += $"<a href='{fileUrl}' target='_blank' class='badge bg-white text-dark border p-2' style='font-size: 11px; text-decoration:none;'><i class='fal fa-file-download text-primary me-1'></i> {fName}</a>";
                        }
                    }
                    else
                    {
                        filesHtml += $"<span class='text-muted fst-italic'>{gettext("no_file", "No file attached.")}</span>";
                    }
                    filesHtml += "</div>";


                    string sqlTasks = "SELECT t.TaskID, u.NameSurname as EmployeeName, t.StartDate, t.EndDate FROM Tasks t LEFT JOIN Users u ON t.EmployeeID = u.UserID WHERE t.RequestID = " + reqId + " ORDER BY t.StartDate DESC";
                    DataTable dtTasks = db.SqlToDt(sqlTasks);
                    string tasksHtml = "";

                    if (dtTasks != null && dtTasks.Rows.Count > 0)
                    {
                        tasksHtml += "<div class='table-responsive mt-2'><table class='table table-sm table-bordered bg-white shadow-sm' style='font-size: 11.5px;'>";
                        tasksHtml += $"<thead style='background-color:#f1f5f9; color:#0F406B;'><tr><th>{gettext("th_personnel", "Personnel")}</th><th>{gettext("th_start", "Start")}</th><th>{gettext("th_end", "End")}</th></tr></thead><tbody>";
                        foreach (DataRow tRow in dtTasks.Rows)
                        {
                            string empName = tRow["EmployeeName"].ToString();
                            string sDate = tRow["StartDate"] != DBNull.Value ? Convert.ToDateTime(tRow["StartDate"]).ToString("dd.MM.yyyy HH:mm") : "-";
                            string eDate = tRow["EndDate"] != DBNull.Value ? Convert.ToDateTime(tRow["EndDate"]).ToString("dd.MM.yyyy HH:mm") : "-";
                            tasksHtml += $"<tr><td class='fw-bold text-primary'><i class='fal fa-user-cog me-1'></i>{empName}</td><td>{sDate}</td><td>{eDate}</td></tr>";
                        }
                        tasksHtml += "</tbody></table></div>";
                    }
                    else
                    {
                        tasksHtml = $"<span class='text-muted fst-italic'>{gettext("no_personnel", "No personnel assigned yet.")}</span>";
                    }

                    string sqlForeign = "SELECT Section, Explanation, StartDate, EndDate FROM ForeignDemands WHERE RequestId = " + reqId + " ORDER BY StartDate DESC";
                    DataTable dtForeign = db.SqlToDt(sqlForeign);
                    string foreignHtml = "";

                    if (dtForeign != null && dtForeign.Rows.Count > 0)
                    {
                        foreignHtml += "<div class='table-responsive mt-2'><table class='table table-sm table-bordered bg-white shadow-sm' style='font-size: 11.5px;'>";
                        foreignHtml += $"<thead style='background-color:#17a2b8; color:white;'><tr><th>{gettext("th_section", "Section")}</th><th>{gettext("th_explanation", "Explanation")}</th><th>{gettext("th_date_range", "Date Range")}</th></tr></thead><tbody>";
                        foreach (DataRow fRow in dtForeign.Rows)
                        {
                            string fSection = fRow["Section"].ToString();
                            string fExp = fRow["Explanation"].ToString();
                            string sDate = fRow["StartDate"] != DBNull.Value ? Convert.ToDateTime(fRow["StartDate"]).ToString("dd.MM.yyyy") : "-";
                            string eDate = fRow["EndDate"] != DBNull.Value ? Convert.ToDateTime(fRow["EndDate"]).ToString("dd.MM.yyyy") : "-";
                            foreignHtml += $"<tr><td class='fw-bold ' style='white-space:nowrap;'>{fSection}</td><td>{fExp}</td><td style='white-space:nowrap;'>{sDate} - {eDate}</td></tr>";
                        }
                        foreignHtml += "</tbody></table></div>";
                    }
                    else
                    {
                        foreignHtml = $"<span class='text-muted fst-italic'>{gettext("no_foreign", "No foreign demands recorded.")}</span>";
                    }


                    string htmlContent = $@"
            <h5 style='color: #0F406B; font-weight: 700; margin-bottom: 15px;'>{topic}</h5>
            <div class='mb-3 pb-3 border-bottom'>
               <div class='mb-2'><strong>{gettext("lbl_date", "Date:")}</strong> {reqDate}</div>
               <div class='mb-2'><strong>{gettext("lbl_category", "Category:")}</strong> {fullCategory}</div>
               <div class='mb-2'><strong>{gettext("lbl_device", "Device:")}</strong> {device}</div>
               {dueDateHtml}
               {priorityHtml}
            </div>
            <div class='mb-3 pb-3 border-bottom'>
                <h6 style='color: #0F406B; font-weight: 700; font-size: 12px;'>{gettext("desc", "DESCRIPTION")}</h6>
                <div class='p-3 bg-white border rounded' style='line-height: 1.6;'>{desc}</div>
            </div>
            <div class='mb-3 pb-3 border-bottom'>
                <h6 style='color: #0F406B; font-weight: 700; font-size: 12px;'>{gettext("attachedfiletext", "ATTACHED FILES")}</h6>
                {filesHtml}
            </div>
            <div class='mb-3 pb-3 border-bottom'>
                <h6 style='color: #0F406B; font-weight: 700; font-size: 12px;'>{gettext("assignedpertext", "ASSIGNED IT PERSONNEL")}</h6>
                {tasksHtml}
            </div>
            <div>
                <h6 style='color: #0F406B; font-weight: 700; font-size: 12px;'>{gettext("foreigntitle", "FOREIGN DEMANDS")}</h6>
                {foreignHtml}
            </div>";

                    litRequestDetails.Text = htmlContent;


                    string status = row["IsConfirmed"] != DBNull.Value ? row["IsConfirmed"].ToString() : "0";
                    if (status == "3")
                    {
                        txtNewMessage.Enabled = false;
                        txtNewMessage.Attributes["placeholder"] = gettext("chat_disabled", "This request has been completed. Messaging is disabled.");
                        btnSendMessage.Enabled = false;
                        btnSendMessage.CssClass = "btn btn-secondary h-100 disabled";
                        pnlQualityScore.Visible = true;

                        if (row["Score"] != DBNull.Value && !string.IsNullOrEmpty(row["Score"].ToString()))
                        {
                            ddlQualityScore.SelectedValue = row["Score"].ToString();
                            ddlQualityScore.Enabled = false;
                            btnSaveScore.Visible = false;
                            lblScoreThanks.Visible = true;
                            smScoreInfo.Visible = false;
                        }
                        else
                        {
                            ddlQualityScore.SelectedIndex = 0;
                            ddlQualityScore.Enabled = true;
                            btnSaveScore.Visible = true;
                            lblScoreThanks.Visible = false;
                            smScoreInfo.Visible = true;
                        }
                    }
                    else
                    {
                        txtNewMessage.Enabled = true;
                        txtNewMessage.Attributes["placeholder"] = gettext("type_message", "Type your message...");
                        btnSendMessage.Enabled = true;
                        btnSendMessage.CssClass = "btn btn-send-custom h-100";
                        pnlQualityScore.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                litRequestDetails.Text = $"<div class='alert alert-danger'>Error loading details: {chk.temizle(ex.Message)}</div>";
            }
        }

        private void LoadChat(string requestId)
        {
            try
            {
                string sql = $"SELECT c.Messages, c.RecordDate, ISNULL(u.NameSurname, 'System / Admin') as SenderName, ISNULL(u.UserType, 'admin') as UserType FROM Chats c LEFT JOIN Users u ON c.UserID = u.UserID WHERE c.RequestID = {requestId} ORDER BY c.ChatID ASC";
                DataTable dt = db.SqlToDt(sql);

                rptChat.DataSource = dt;
                rptChat.DataBind();
                lblNoMessages.Visible = (dt == null || dt.Rows.Count == 0);
            }
            catch (Exception ex) { ShowAlert("error", "Error", "Chat load error: " + chk.temizle(ex.Message), ""); }
        }

        protected void btnSendMessage_Click(object sender, EventArgs e)
        {
            try
            {
                string reqId = hfChatRequestId.Value;
                string msg = chk.temizle(txtNewMessage.Text).Replace("'", "''");

                if (string.IsNullOrEmpty(reqId) || string.IsNullOrWhiteSpace(msg)) return;

                string userSession = Session["User"]?.ToString() ?? cookie.Oku("User");
                string loggedInId = "NULL";
                string cleanSession = chk.temizle(userSession);
                string sqlFindUser = "SELECT UserID FROM Users WHERE (UserName = '" + cleanSession + "' OR NameSurname = '" + cleanSession + "' OR Email = '" + cleanSession + "') AND isDeleted IS NULL";
                DataTable dtUser = db.SqlToDt(sqlFindUser);
                if (dtUser.Rows.Count > 0) { loggedInId = dtUser.Rows[0]["UserID"].ToString(); }

                string sql = "INSERT INTO Chats (RequestID, UserID, Messages, RecordDate) VALUES (" + reqId + ", " + loggedInId + ", '" + msg + "', GETDATE())";

                if (db.ExecStr(sql) > 0)
                {
                    txtNewMessage.Text = "";
                    LoadChat(reqId);

                    string successScript = @"
                            if (window.history.replaceState) {
                                var cleanUrl = window.location.href.split('#')[0];
                                window.history.replaceState(null, null, cleanUrl + '#chatSection');
                            }
                            setTimeout(function() { 
                                var div = document.getElementById('userChatContainer');
                                if(div) { div.scrollTop = div.scrollHeight; }
                            }, 50);";

                    ScriptManager.RegisterStartupScript(this, GetType(), "msgSent", successScript, true);
                }
            }
            catch (Exception ex)
            {
                ShowAlert("error", "Error", "Message could not be sent: " + chk.temizle(ex.Message), "");
            }
        }

        protected void btnBackToTable_Click(object sender, EventArgs e)
        {
            pnlChat.Visible = false;
            pnlTable.Visible = true;
            hfChatRequestId.Value = "";
            LoadRequests();
        }

        protected string GetRowColorClass(object isConfirmed)
        {

            if (isConfirmed.ToString() == "0") return "table-warning";
            if (isConfirmed.ToString() == "1") return "table-success";
            if (isConfirmed.ToString() == "2") return "opacity-50 text-muted";
            if (isConfirmed.ToString() == "3") return "";

            return "";
        }

        protected string GetStatusBadge(object isConfirmed)
        {
            if (isConfirmed == null || isConfirmed == DBNull.Value) return "<span class='badge bg-secondary'><i class='fal fa-clock me-1'></i></span>";
            if (isConfirmed.ToString() == "1") return "<span class='badge bg-success' title='APPROVED'><i class='fal fa-check me-1'></i></span>";
            if (isConfirmed.ToString() == "2") return "<span class='badge bg-danger' title='REJECTED'><i class='fal fa-times me-1'></i></span>";
            if (isConfirmed.ToString() == "3") return "<span class='badge bg-info' title='COMPLETED'><i class='fal fa-check-double me-1'></i></span>";
            return "<span class='badge bg-secondary' title='PENDING'><i class='fal fa-clock me-1'></i></span>";
        }

        protected string GetTimelineClass(object senderName)
        {
            return "timeline-item left";
        }

        protected string GetPriorityBadges(object isImportant, object isUrgent)
        {
            bool urgent = isUrgent != DBNull.Value && (Convert.ToBoolean(isUrgent) || isUrgent.ToString() == "1");
            bool important = isImportant != DBNull.Value && (Convert.ToBoolean(isImportant) || isImportant.ToString() == "1");
            string html = "";
            html += urgent ? $"<span class='badge bg-danger me-1' title='URGENT'><i class=\"far fa-exclamation\"></i></span>" : "";
            html += important ? $"<span class='badge bg-warning text-dark me-1' title='IMPORTANT'><i class=\"fal fa-user-crown\"></i></span>" : "";
            return html;
        }

    }
}