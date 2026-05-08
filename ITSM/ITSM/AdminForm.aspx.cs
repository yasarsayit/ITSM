using ITSM;
using System;
using System.Data;
using System.Globalization;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ITSM
{
    public partial class AdminForm : System.Web.UI.Page
    {
        DBTools db = new DBTools();
        CookieTools cookie = new CookieTools();
        CheckTools chk = new CheckTools();

        public string CurrentAdminName { get; set; } = "System / Admin";
        public string FilterForeignSection { get { return ViewState["F_ForSec"] as string ?? ""; } set { ViewState["F_ForSec"] = value; } }
        public string FilterForeignExp { get { return ViewState["F_ForExp"] as string ?? ""; } set { ViewState["F_ForExp"] = value; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["User"] == null && string.IsNullOrEmpty(cookie.Oku("User")))
            {
                Response.Redirect("Login.aspx");
                return;
            }

            string uType = cookie.Oku("UserType")?.Trim().ToLower();
            if (uType != "admin")
            {
                Response.Redirect("Default.aspx");
                return;
            }

            string cleanSession = chk.temizle(Session["User"]?.ToString() ?? cookie.Oku("User"));
            DataTable dtUser = db.SqlToDt("SELECT NameSurname FROM Users WHERE (UserName = '" + cleanSession + "' OR NameSurname = '" + cleanSession + "') AND isDeleted IS NULL");
            if (dtUser.Rows.Count > 0) CurrentAdminName = dtUser.Rows[0]["NameSurname"].ToString();

            string qsRequestId = chk.temizle(Request.QueryString["id"] ?? "");
            if (!string.IsNullOrEmpty(qsRequestId))
                hfSelectedRequestId.Value = qsRequestId;

            if (!IsPostBack)
            {
                LoadRequestTypes();
                LoadUsersAndDevices();
                LoadEmployees();
                LoadForeignDepartments();
                LoadForeignDemandsList();


                if (!string.IsNullOrEmpty(qsRequestId))
                {
                    LoadRequestDetails(qsRequestId);
                    FillFormDetails(qsRequestId);
                }
                else
                {
                    Response.Redirect("Admin.aspx");
                }
            }
        }


        private void LoadRequestDetails(string reqId)
        {
            hfSelectedRequestId.Value = reqId;
            lblReqIdDisplay.Text = reqId;

            try
            {
                string sql = "SELECT * FROM Requests WHERE RequestId = " + reqId;
                DataTable dt = db.SqlToDt(sql);

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];

                    if (txtTopic != null) txtTopic.Text = row["Topic"].ToString();
                    if (txtDescription != null) txtDescription.Text = row["Description"].ToString();

                    string userId = row["UserID"] != DBNull.Value ? row["UserID"].ToString() : "0";
                    if (ddlRequestOwner.Items.FindByValue(userId) != null) ddlRequestOwner.SelectedValue = userId;

                    string relatedUserId = row["RelatedUserID"] != DBNull.Value ? row["RelatedUserID"].ToString() : "0";
                    if (ddlRelatedUser.Items.FindByValue(relatedUserId) != null) ddlRelatedUser.SelectedValue = relatedUserId;

                    string deviceId = row["DeviceID"] != DBNull.Value ? row["DeviceID"].ToString() : "0";
                    if (ddlRelatedDevice.Items.FindByValue(deviceId) != null) ddlRelatedDevice.SelectedValue = deviceId;

                    string typeVal = row["RequestType"] != DBNull.Value ? row["RequestType"].ToString() : "";
                    if (!string.IsNullOrEmpty(typeVal) && ddlRequestType.Items.FindByValue(typeVal) != null)
                    {
                        ddlRequestType.SelectedValue = typeVal;
                        LoadRequestSubtypes(typeVal);

                        if (row["RequestSubtype"] != DBNull.Value)
                        {
                            string subTypeVal = row["RequestSubtype"].ToString();
                            if (ddlRequestSubtype.Items.FindByValue(subTypeVal) != null)
                                ddlRequestSubtype.SelectedValue = subTypeVal;
                        }
                        ddlRequestSubtype.Enabled = true;
                    }

                    chkIsImportant.Checked = row["IsImportant"] != DBNull.Value && (Convert.ToBoolean(row["IsImportant"]) || row["IsImportant"].ToString() == "1");
                    chkIsUrgent.Checked = row["IsUrgent"] != DBNull.Value && (Convert.ToBoolean(row["IsUrgent"]) || row["IsUrgent"].ToString() == "1");

                    if (row["Score"] != DBNull.Value && !string.IsNullOrEmpty(row["Score"].ToString()))
                    {
                        string scoreVal = row["Score"].ToString();
                        if (scoreVal == "0") { lblQualityScore.Text = "0 - Far below expectations"; lblQualityScore.CssClass = "badge bg-danger"; }
                        else if (scoreVal == "1") { lblQualityScore.Text = "1 - Below expectations"; lblQualityScore.CssClass = "badge bg-warning text-dark"; }
                        else if (scoreVal == "2") { lblQualityScore.Text = "2 - Met expectations"; lblQualityScore.CssClass = "badge bg-primary"; }
                        else if (scoreVal == "3") { lblQualityScore.Text = "3 - Above expectations"; lblQualityScore.CssClass = "badge bg-success"; }
                    }
                    else
                    {
                        lblQualityScore.Text = "Not Rated Yet";
                        lblQualityScore.CssClass = "badge bg-light text-muted border";
                    }

                    if (row.Table.Columns.Contains("ScoreDesc") && row["ScoreDesc"] != DBNull.Value && !string.IsNullOrWhiteSpace(row["ScoreDesc"].ToString()))
                    {
                        txtAdminScoreDesc.Text = row["ScoreDesc"].ToString();
                        pnlAdminScoreDesc.Visible = true; 
                    }
                    else
                    {
                        pnlAdminScoreDesc.Visible = false; 
                    }

                    txtDueDate.Text = row["DueDate"] != DBNull.Value ? Convert.ToDateTime(row["DueDate"]).ToString("yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture) : "";
                    txtEndDate.Text = row["EndDate"] != DBNull.Value ? Convert.ToDateTime(row["EndDate"]).ToString("yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture) : "";

                    string isApp = row["IsConfirmed"] != DBNull.Value ? row["IsConfirmed"].ToString() : "0";

                    if (row["EndDate"] != DBNull.Value)
                    {
                        isApp = "3";
                    }

                    if (ddlApproval.Items.FindByValue(isApp) != null) ddlApproval.SelectedValue = isApp;

                    if (isApp == "3")
                    {
                        ddlApproval.Style["pointer-events"] = "none";
                        ddlApproval.Style["background-color"] = "#e9ecef";

                        btnAssign.Enabled = false;
                        btnAssign.CssClass = "btn btn-secondary btn-sm w-100 fw-bold shadow-sm disabled";
                        ddlEmployee.Enabled = false;
                        txtTaskStart.Enabled = false;
                        txtTaskEnd.Enabled = false;
                    }
                    else
                    {
                        ddlApproval.Style["pointer-events"] = "auto";
                        ddlApproval.Style["background-color"] = "#ffffff";

                        btnAssign.Enabled = true;
                        btnAssign.CssClass = "btn btn-primary btn-sm w-100 fw-bold shadow-sm";
                        ddlEmployee.Enabled = true;
                        txtTaskStart.Enabled = true;
                        txtTaskEnd.Enabled = true;
                    }

                    string sqlFiles = "SELECT FileName, FilePath FROM RequestFiles WHERE RequestId = " + reqId + " AND (IsDeleted IS NULL OR IsDeleted != 'X')";
                    DataTable dtFiles = db.SqlToDt(sqlFiles);
                    string filesHtml = "";

                    if (dtFiles != null && dtFiles.Rows.Count > 0)
                    {
                        filesHtml += "<div class='d-flex flex-wrap gap-2'>";
                        foreach (DataRow fRow in dtFiles.Rows)
                        {
                            string fName = fRow["FileName"].ToString();
                            string fPath = fRow["FilePath"].ToString();
                            string fileUrl = ResolveUrl("~/UploadedFolders/" + fPath + "/" + fName);
                            filesHtml += $"<a href='{fileUrl}' target='_blank' class='badge bg-light text-dark border p-2' style='font-size: 11px; text-decoration:none;'><i class='fal fa-file me-1'></i> {fName}</a>";
                        }
                        filesHtml += "</div>";
                    }
                    else
                    {
                        filesHtml = "<span class='text-muted fst-italic'>No files attached.</span>";
                    }
                    litExistingFiles.Text = filesHtml;

                    LoadTasks(reqId);
                    LoadChat(reqId);
                }
            }
            catch (Exception ex)
            {
                ShowAlert("error", "Database Error", "Could not retrieve record. Detail: " + chk.temizle(ex.Message));
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Admin.aspx");
        }

        protected void btnSaveAdmin_Click(object sender, EventArgs e)
        {
            try
            {
                string reqId = chk.temizle(hfSelectedRequestId.Value);
                if (string.IsNullOrEmpty(reqId))
                    reqId = chk.temizle(Request.QueryString["id"] ?? "");
                if (string.IsNullOrEmpty(reqId))
                {
                    ShowAlert("error", "Error", "Request id is missing. Open the request again from the admin list.");
                    return;
                }

                string userSession = Session["User"]?.ToString() ?? cookie.Oku("User");
                if (string.IsNullOrEmpty(userSession)) userSession = "System";

                string loggedInId = "NULL";
                string cleanSession = chk.temizle(userSession);

                string sqlFindUser = "SELECT UserID FROM Users WHERE (UserName = '" + cleanSession + "' OR NameSurname = '" + cleanSession + "' OR Email = '" + cleanSession + "') AND isDeleted IS NULL";
                DataTable dtUser = db.SqlToDt(sqlFindUser);

                if (dtUser.Rows.Count > 0) loggedInId = dtUser.Rows[0]["UserID"].ToString();

                string newTopic = chk.temizle(txtTopic.Text).Replace("'", "''");
                string newDesc = chk.temizle(txtDescription.Text).Replace("'", "''");

                string newOwnerId = (!string.IsNullOrEmpty(ddlRequestOwner.SelectedValue) && ddlRequestOwner.SelectedValue != "0") ? ddlRequestOwner.SelectedValue : "NULL";

                if (newOwnerId == "NULL")
                {
                    ShowAlert("warning", "Eksik Bilgi", "Lütfen 'Request Owner' (Talep Sahibi) seçiniz. Bu alan veritabanında boş bırakılamaz.");
                    return;
                }

                string newRelatedUserId = (!string.IsNullOrEmpty(ddlRelatedUser.SelectedValue) && ddlRelatedUser.SelectedValue != "0") ? ddlRelatedUser.SelectedValue : "NULL";
                string newDeviceId = (!string.IsNullOrEmpty(ddlRelatedDevice.SelectedValue) && ddlRelatedDevice.SelectedValue != "0") ? ddlRelatedDevice.SelectedValue : "NULL";
                string typeSql = (!string.IsNullOrEmpty(ddlRequestType.SelectedValue) && ddlRequestType.SelectedValue != "0") ? ddlRequestType.SelectedValue : "NULL";
                string subtypeSql = (!string.IsNullOrEmpty(ddlRequestSubtype.SelectedValue) && ddlRequestSubtype.SelectedValue != "0") ? ddlRequestSubtype.SelectedValue : "NULL";

                string endSql = FormatSqlDateTime(txtEndDate.Text);
                string isAppSql = chk.temizle(ddlApproval.SelectedValue);

                string newScoreDesc = chk.temizle(txtAdminScoreDesc.Text).Replace("'", "''");
                string scoreDescSql = string.IsNullOrEmpty(newScoreDesc) ? "NULL" : "'" + newScoreDesc + "'";

                if (endSql != "NULL")
                {
                    isAppSql = "3";
                }
                else if (string.IsNullOrEmpty(isAppSql))
                {
                    isAppSql = "0";
                }

                if (isAppSql == "3" && hfConfirmTaskClose.Value != "1")
                {

                    string checkTasksSql = "SELECT COUNT(*) FROM Tasks WHERE RequestID = " + reqId + " AND EndDate IS NULL AND IsDeleted IS NULL  ";
                    int openTaskCount = db.SqlToDt(checkTasksSql)?.Rows.Count > 0 ? Convert.ToInt32(db.SqlToDt(checkTasksSql).Rows[0][0]) : 0;

                    string checkForeignSql = "SELECT COUNT(*) FROM ForeignDemands WHERE RequestId = " + reqId + " AND EndDate IS NULL AND IsDeleted IS NULL";
                    int openForeignCount = db.SqlToDt(checkForeignSql)?.Rows.Count > 0 ? Convert.ToInt32(db.SqlToDt(checkForeignSql).Rows[0][0]) : 0;

                    if (openTaskCount > 0 || openForeignCount > 0)
                    {
                        string itemsText = "";
                        if (openTaskCount > 0 && openForeignCount > 0) itemsText = "personel görevleri ve dış talepler";
                        else if (openTaskCount > 0) itemsText = "personel görev(ler)i";
                        else itemsText = "dış talep(ler)";

                        string dateText = string.IsNullOrEmpty(txtEndDate.Text) ? "şu anki zaman" : "girmiş olduğunuz Bitiş Tarihi";

                        string script = $@"
                    Swal.fire({{
                        title: 'Açık İşlemler Bulundu!',
                        text: 'Bu talebe bağlı ve henüz bitiş tarihi girilmemiş {itemsText} var. Talebi kapattığınızda, bu açık işlemlerin bitiş tarihleri otomatik olarak {dateText} baz alınarak sonlandırılacaktır. Onaylıyor musunuz?',
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#0F406B',
                        cancelButtonColor: '#d33',
                        confirmButtonText: 'Evet, Hepsini Kapat ve Kaydet!',
                        cancelButtonText: 'İptal'
                    }}).then((result) => {{
                        if (result.isConfirmed) {{
                            document.getElementById('hfConfirmTaskClose').value = '1';
                            document.getElementById('{btnSaveAdmin.ClientID}').click();
                        }}
                    }});

                ";
                        ScriptManager.RegisterStartupScript(this, GetType(), "confirmTaskClose", script, true);

                        return;
                    }
                }

                string confirmDateSql = (isAppSql == "1" || isAppSql == "2" || isAppSql == "3") ? "GETDATE()" : "NULL";
                string confirmBySql = (isAppSql == "1" || isAppSql == "2" || isAppSql == "3") ? loggedInId : "NULL";

                string isImp = chkIsImportant.Checked ? "1" : "0";
                string isUrg = chkIsUrgent.Checked ? "1" : "0";
                string dueSql = FormatSqlDateTime(txtDueDate.Text);

                string currentAdmin = chk.temizle(Session["User"]?.ToString() ?? cookie.Oku("User"));

                if (fuAdminAttachment.HasFiles)
                {
                    string dateFolderName = DateTime.Now.ToString("yyyy-MM");
                    string folderPath = Server.MapPath("~/UploadedFolders/" + dateFolderName + "/");
                    if (!System.IO.Directory.Exists(folderPath)) System.IO.Directory.CreateDirectory(folderPath);

                    foreach (System.Web.HttpPostedFile postedFile in fuAdminAttachment.PostedFiles)
                    {
                        if (postedFile.ContentLength > 0)
                        {
                            string fullFileName = System.IO.Path.GetFileName(postedFile.FileName);
                            string extension = System.IO.Path.GetExtension(postedFile.FileName);

                            if (chk.KabulEdilenUzanti(extension))
                            {
                                postedFile.SaveAs(folderPath + fullFileName);
                                string fileNameOnly = System.IO.Path.GetFileNameWithoutExtension(fullFileName);
                                string sqlFile = "INSERT INTO RequestFiles (RequestId, FileName, FileType, FilePath, RecordDate, RecordUser, isDeleted) " +
                                                 "VALUES (" + reqId + ", '" + fileNameOnly.Replace("'", "''") + "', '" + extension + "', '" + dateFolderName + "', GETDATE(), '" + cleanSession + "', NULL)";
                                db.ExecStr(sqlFile);
                            }
                        }
                    }
                }

                string scoreSql = "Score";
                string endDateSql;

                if (isAppSql == "3")
                {
                    endDateSql = (endSql == "NULL") ? "GETDATE()" : endSql;
                }
                else
                {
                    scoreSql = "NULL";
                    endDateSql = endSql;
                }

                string sqlUpdate = "UPDATE Requests SET " +
                            "Topic = '" + newTopic + "', " +
                            "Description = '" + newDesc + "', " +
                            "UserID = " + newOwnerId + ", " +
                            "RelatedUserID = " + newRelatedUserId + ", " +
                            "DeviceID = " + newDeviceId + ", " +
                            "RequestType = " + typeSql + ", " +
                            "RequestSubtype = " + subtypeSql + ", " +
                            "IsImportant = " + isImp + ", " +
                            "IsUrgent = " + isUrg + ", " +
                            "DueDate = " + dueSql + ", " +
                            "IsConfirmed = " + isAppSql + ", " +
                            "ConfirmDate = " + confirmDateSql + ", " +
                            "ConfirmBy = " + confirmBySql + ", " +
                            "ConfirmedBy = " + loggedInId + ", " +
                            "Score = " + scoreSql + ", " +
                            "ScoreDesc = " + scoreDescSql + ", " +
                            "EndDate = " + endDateSql + ", " +
                            "UpdateDate = GETDATE(), " +
                            "UpdateUser = '" + cleanSession + "' " +
                            "WHERE RequestId = " + reqId;

                int result = db.ExecStr(sqlUpdate);

                if (result > 0)
                {
                    if (endDateSql != "NULL")
                    {
                        db.ExecStr("UPDATE Tasks SET EndDate = " + endDateSql + " WHERE RequestID = " + reqId + " AND EndDate IS NULL");
                        db.ExecStr("UPDATE ForeignDemands SET EndDate = " + endDateSql + " WHERE RequestId = " + reqId + " AND EndDate IS NULL");
                    }

                    hfConfirmTaskClose.Value = "0";
                    LoadRequestDetails(reqId);
                    LoadTasks(reqId);
                    LoadForeignDemandsList();
                    ScriptManager.RegisterStartupScript(this, GetType(), "successMsg", "Swal.fire('Success', 'Request updated successfully.', 'success');", true);
                }
                else if (result < 0)
                {
                    ShowAlert("error", "Database Error", string.IsNullOrEmpty(db.Hata) ? "Update failed." : chk.temizle(db.Hata));
                }
                else
                {
                    ShowAlert("error", "Error", "No row was updated (wrong id or no change). " + chk.temizle(db.Hata));
                }
            }
            catch (Exception ex)
            {
                ShowAlert("error", "System Error", chk.temizle(ex.Message));
            }
        }

        private void LoadUsersAndDevices()
        {
            try
            {
                string sqlUsers = "SELECT UserID, NameSurname FROM Users WHERE isDeleted IS NULL ORDER BY NameSurname ASC";
                db.Sql2AddCombo(ddlRequestOwner, sqlUsers, "", "NameSurname", "UserID", true, "-- Select Owner --");
                db.Sql2AddCombo(ddlRelatedUser, sqlUsers, "", "NameSurname", "UserID", true, "-- Select Related Person --");

                string sqlDevices = "SELECT DeviceID, (Brand + ' ' + Model) as DeviceName FROM device WHERE IsDeleted IS NULL ORDER BY Brand ASC";
                db.Sql2AddCombo(ddlRelatedDevice, sqlDevices, "", "DeviceName", "DeviceID", true, "-- Select Device --");
            }
            catch { }
        }

        private void LoadRequestTypes()
        {
            try
            {
                string sql = "SELECT TypeID, Description FROM RequestTypes ORDER BY Description";
                db.Sql2AddCombo(ddlRequestType, sql, "", "Description", "TypeID", true, "-- Select Type --");
            }
            catch { }
        }

        protected void ddlRequestType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlRequestType.SelectedIndex > 0)
            {
                string typeId = chk.temizle(ddlRequestType.SelectedValue);
                LoadRequestSubtypes(typeId);
                ddlRequestSubtype.Enabled = true;
            }
            else
            {
                ddlRequestSubtype.Items.Clear();
                ddlRequestSubtype.Items.Add(new ListItem("-- Select Type First --", "0"));
                ddlRequestSubtype.Enabled = false;
            }
        }

        private void LoadRequestSubtypes(string typeId)
        {
            try
            {
                string sql = "SELECT SubTypeID as TypeID, Description FROM RequestSubTypes WHERE ReqTypeId = " + typeId + " ORDER BY Description ASC";
                db.Sql2AddCombo(ddlRequestSubtype, sql, "", "Description", "TypeID", true, "-- Select Subtype --");
            }
            catch { }
        }

        private void LoadEmployees()
        {
            string sql = "SELECT UserID, NameSurname FROM Users WHERE Department = N'Bilgi Teknolojileri' AND isDeleted IS NULL ORDER BY NameSurname ASC";
            db.Sql2AddCombo(ddlEmployee, sql, "", "NameSurname", "UserID", true, "-- Select IT Personnel --");
        }

        private void LoadTasks(string requestId)
        {
      
            string sql = "SELECT t.TaskID, u.NameSurname as EmployeeName, t.StartDate, t.EndDate FROM Tasks t LEFT JOIN Users u ON t.EmployeeID = u.UserID WHERE t.IsDeleted IS NULL And t.RequestID = " + requestId + " ORDER BY t.StartDate DESC";
            gvTasks.DataSource = db.SqlToDt(sql);
            gvTasks.DataBind();
        }

        protected void gvTasks_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string taskId = e.CommandArgument.ToString();

            if (e.CommandName == "DeleteTask")
            {

                db.ExecStr("UPDATE  Tasks SET IsDeleted='X' WHERE TaskID = " + taskId);
                LoadTasks(hfSelectedRequestId.Value);
                ShowAlert("success", "Deleted", "Assignment has been removed.");
            }
            else if (e.CommandName == "EditTask")
            {
                try
                {
                    DataTable dt = db.SqlToDt("SELECT * FROM Tasks WHERE TaskID = " + taskId);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataRow dr = dt.Rows[0];
                        ddlEmployee.SelectedValue = dr["EmployeeID"].ToString();
                        if (dr["StartDate"] != DBNull.Value) txtTaskStart.Text = Convert.ToDateTime(dr["StartDate"]).ToString("yyyy-MM-ddTHH:mm");
                        if (dr["EndDate"] != DBNull.Value) txtTaskEnd.Text = Convert.ToDateTime(dr["EndDate"]).ToString("yyyy-MM-ddTHH:mm");

                        hfSelectedTaskId.Value = taskId;
                        btnAssign.Text = "<i class='fal fa-sync me-1'></i> UPDATE TASK";
                        btnAssign.CssClass = "btn btn-warning btn-sm w-100 fw-bold shadow-sm text-white";
                    }
                }
                catch (Exception ex) { ShowAlert("error", "Error", ex.Message); }
            }
        }

        protected void btnAssign_Click(object sender, EventArgs e)
        {
            try
            {
                string reqId = hfSelectedRequestId.Value;


                string checkStatusSql = "SELECT IsConfirmed FROM Requests WHERE RequestId = " + reqId;
                DataTable dtCheck = db.SqlToDt(checkStatusSql);
                if (dtCheck != null && dtCheck.Rows.Count > 0 && dtCheck.Rows[0]["IsConfirmed"].ToString() == "3")
                {
                    ShowAlert("warning", "Uyarı", "Bu talep tamamlandığı için yeni personel ataması yapılamaz.");
                    return;
                }


                string empId = ddlEmployee.SelectedValue;
                string taskId = hfSelectedTaskId.Value;
                string user = chk.temizle(Session["User"]?.ToString() ?? cookie.Oku("User"));

                if (string.IsNullOrEmpty(reqId) || empId == "0") { ShowAlert("error", "Selection Required", "Please select a personnel."); return; }

                string start = string.IsNullOrEmpty(txtTaskStart.Text) ? "GETDATE()" : "'" + txtTaskStart.Text.Replace("T", " ") + "'";
                string end = string.IsNullOrEmpty(txtTaskEnd.Text) ? "NULL" : "'" + txtTaskEnd.Text.Replace("T", " ") + "'";


                string sqlUpdateReq = $"UPDATE Requests SET RelatedUserID = '{chk.temizle(empId)}' WHERE RequestId = {chk.temizle(reqId)}";
                db.ExecStr(sqlUpdateReq);

                string sql = string.IsNullOrEmpty(taskId)
                    ? $"INSERT INTO Tasks (RequestID, EmployeeID, StartDate, EndDate, RecordDate, RecordUser) VALUES ({reqId}, {empId}, {start}, {end}, GETDATE(), '{user}')"
                    : $"UPDATE Tasks SET EmployeeID = {empId}, StartDate = {start}, EndDate = {end}, UpdateDate = GETDATE(), UpdateUser = '{user}' WHERE TaskID = {taskId}";

                if (db.ExecStr(sql) > 0)
                {
                    ShowAlert("success", "Success", string.IsNullOrEmpty(taskId) ? "Task assigned." : "Task updated.");
                    hfSelectedTaskId.Value = "";
                    btnAssign.Text = "<i class='fal fa-plus-circle me-1'></i> ASSIGN TASK";
                    btnAssign.CssClass = "btn btn-primary btn-sm w-100 fw-bold shadow-sm";
                    txtTaskStart.Text = ""; txtTaskEnd.Text = ""; ddlEmployee.SelectedIndex = 0;
                    LoadTasks(reqId);
                }
            }
            catch (Exception ex) { ShowAlert("error", "Error", ex.Message); }
        }

        private void LoadChat(string requestId)
        {
            try
            {
                string sql = "SELECT c.Messages, c.RecordDate, ISNULL(u.NameSurname, 'System / Admin') as SenderName, ISNULL(u.UserType, 'admin') as UserType FROM Chats c LEFT JOIN Users u ON c.UserID = u.UserID WHERE c.RequestID = " + requestId + " ORDER BY c.ChatID ASC";
                DataTable dt = db.SqlToDt(sql);

                if (dt != null)
                {
                    rptChat.DataSource = dt;
                    rptChat.DataBind();
                    lblNoMessages.Visible = (dt.Rows.Count == 0);
                }

                string statusSql = "SELECT IsConfirmed FROM Requests WHERE RequestId = " + requestId;
                DataTable dtStatus = db.SqlToDt(statusSql);

                if (dtStatus != null && dtStatus.Rows.Count > 0)
                {
                    string status = dtStatus.Rows[0]["IsConfirmed"].ToString();

                    if (status == "3")
                    {
                        txtNewMessage.Enabled = false;
                        txtNewMessage.Attributes["placeholder"] = "This request has been completed. Messaging is disabled.";
                        btnSendMessage.Enabled = false;
                        btnSendMessage.CssClass = "btn btn-secondary d-flex align-items-center justify-content-center disabled";
                    }
                    else
                    {
                        txtNewMessage.Enabled = true;
                        txtNewMessage.Attributes["placeholder"] = "Type your message...";
                        btnSendMessage.Enabled = true;
                        btnSendMessage.CssClass = "btn btn-send-custom";
                    }
                }
            }
            catch (Exception ex) { ShowAlert("error", "Error", "Could not load messages: " + chk.temizle(ex.Message)); }
        }

        protected void btnSendMessage_Click(object sender, EventArgs e)
        {
            try
            {
                string reqId = hfSelectedRequestId.Value;
                string msg = chk.temizle(txtNewMessage.Text).Replace("'", "''");
                if (string.IsNullOrEmpty(reqId) || string.IsNullOrWhiteSpace(msg)) return;

                string userSession = Session["User"]?.ToString() ?? cookie.Oku("User");
                string loggedInId = "NULL";
                string cleanSession = chk.temizle(userSession);

                DataTable dtUser = db.SqlToDt("SELECT UserID FROM Users WHERE (UserName = '" + cleanSession + "' OR NameSurname = '" + cleanSession + "') AND isDeleted IS NULL");
                if (dtUser.Rows.Count > 0) loggedInId = dtUser.Rows[0]["UserID"].ToString();

                string sql = "INSERT INTO Chats (RequestID, UserID, Messages, RecordDate) VALUES (" + reqId + ", " + loggedInId + ", '" + msg + "', GETDATE())";

                if (db.ExecStr(sql) > 0)
                {
                    txtNewMessage.Text = "";
                    LoadChat(reqId);
                    ScriptManager.RegisterStartupScript(this, GetType(), "scrollAndClean", "var div = document.getElementById('chatContainer'); if(div) { div.scrollTop = div.scrollHeight; } if ( window.history.replaceState ) { window.history.replaceState( null, null, window.location.href ); }", true);


                    ShowAlert("success", "Successful", "Your message has been sent successfully.");
                }
            }
            catch (Exception ex) { ShowAlert("error", "Error", "Message could not be sent: " + chk.temizle(ex.Message)); }
        }

        private void LoadForeignDepartments()
        {
            db.Sql2AddCombo(ddlForeignSection, "SELECT  Bolum AS Department FROM JHR.dbo.PerlistAll GROUP BY Bolum ORDER BY Bolum", "", "Department", "Department", true, "-- Select Department --");
        }

        private void LoadForeignDemandsList()
        {
            string reqId = hfSelectedRequestId.Value;
            if (string.IsNullOrEmpty(reqId)) reqId = Request.QueryString["id"];

            if (string.IsNullOrEmpty(reqId)) return;

            string filterCondition = " AND RequestId = " + chk.temizle(reqId);

            if (!string.IsNullOrEmpty(FilterForeignSection))
                filterCondition += " AND Section LIKE '%" + chk.temizle(FilterForeignSection) + "%' ";

            if (!string.IsNullOrEmpty(FilterForeignExp))
                filterCondition += " AND Explanation LIKE '%" + chk.temizle(FilterForeignExp) + "%' ";

            string sql = "SELECT * FROM ForeignDemands WHERE IsDeleted IS NULL AND 1=1  " + filterCondition + " ORDER BY ForeignDemandID DESC";

            DataTable dt = db.SqlToDt(sql);
            rptForeignDemands.DataSource = dt;
            rptForeignDemands.DataBind();
        }

        protected void lbSearchForeign_Click(object sender, EventArgs e)
        {
            FilterForeignSection = txtFilterForeignSection.Text.Trim();
            FilterForeignExp = txtFilterForeignExp.Text.Trim();
            LoadForeignDemandsList();
        }

        protected void lbClearForeignFilter_Click(object sender, EventArgs e)
        {
            txtFilterForeignSection.Text = "";
            txtFilterForeignExp.Text = "";
            FilterForeignSection = "";
            FilterForeignExp = "";
            LoadForeignDemandsList();
        }

        protected void btnForeignSend_Click(object sender, EventArgs e)
        {
            try
            {
                string reqId = hfSelectedRequestId.Value;
                if (string.IsNullOrEmpty(reqId)) { ShowAlert("error", "Error", "Ana talep ID bulunamadı."); return; }

                if (string.IsNullOrEmpty(txtForeignExplanation.Text)) { ShowAlert("warning", "Eksik Bilgi", "Lütfen açıklama giriniz."); return; }

                string user = Session["User"]?.ToString() ?? cookie.Oku("User") ?? "SystemAdmin";
                string cleanUser = chk.temizle(user);
                string section = ddlForeignSection.SelectedValue;
                string explanation = txtForeignExplanation.Text.Replace("'", "''");
                string startSql = string.IsNullOrEmpty(txtForeignStartDate.Text) ? "GETDATE()" : "'" + txtForeignStartDate.Text + "'";

                string endSql = string.IsNullOrEmpty(txtForeignEndDate.Text) ? "NULL" : "'" + txtForeignEndDate.Text + "'";

                string sql = !string.IsNullOrEmpty(hfEditingForeignId.Value)
                    ? $@"UPDATE ForeignDemands SET 
                Explanation = '{explanation}', 
                Section = '{section}', 
                StartDate = {startSql}, 
                EndDate = {endSql} 
                WHERE ForeignDemandID = {hfEditingForeignId.Value}"
                    : $@"INSERT INTO ForeignDemands (RequestId, Subject, Explanation, Title, Section, StartDate, EndDate, RecordUser, RecordDate) 
                VALUES ({reqId}, '{section} Demand', '{explanation}', '{section} Request', '{section}', {startSql}, {endSql}, '{cleanUser}', GETDATE())";
                if (db.ExecStr(sql) > 0)
                {
                    ClearForeignForm();
                    LoadForeignDemandsList();
                    ShowAlert("success", "Başarılı", "İşlem tamamlandı.");
                }
            }
            catch (Exception ex) { ShowAlert("error", "Sistem Hatası", ex.Message); }
        }

        protected void rptForeignDemands_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string id = e.CommandArgument.ToString();
            if (e.CommandName == "EditItem")
            {
                DataTable dt = db.SqlToDt("SELECT * FROM ForeignDemands WHERE ForeignDemandID = " + id);
                if (dt.Rows.Count > 0)
                {
                    hfEditingForeignId.Value = id;
                    txtForeignExplanation.Text = dt.Rows[0]["Explanation"].ToString();
                    if (ddlForeignSection.Items.FindByValue(dt.Rows[0]["Section"].ToString()) != null) ddlForeignSection.SelectedValue = dt.Rows[0]["Section"].ToString();
                    if (dt.Rows[0]["StartDate"] != DBNull.Value) txtForeignStartDate.Text = Convert.ToDateTime(dt.Rows[0]["StartDate"]).ToString("yyyy-MM-dd");
                    if (dt.Rows[0]["EndDate"] != DBNull.Value) txtForeignEndDate.Text = Convert.ToDateTime(dt.Rows[0]["EndDate"]).ToString("yyyy-MM-dd");
                    btnForeignSend.Text = "<i class='fas fa-sync me-2'></i> UPDATE DEMAND";
                    btnCancelEdit.Visible = true;
                }
            }
            else if (e.CommandName == "DeleteDemand")
            {

                if (db.ExecStr("UPDATE ForeignDemands SET IsDeleted='X' WHERE ForeignDemandID = " + id) > 0)
                {
                    LoadForeignDemandsList(); ShowAlert("success", "Deleted", "Demand removed successfully.");
                }
            }
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

        protected void btnDeleteForeignConfirm_Click(object sender, EventArgs e)
        {
            string id = hfDeleteForeignId.Value;
            if (!string.IsNullOrEmpty(id))
            {

                if (db.ExecStr("UPDATE ForeignDemands SET IsDeleted='X' WHERE ForeignDemandID = " + chk.temizle(id)) > 0)
                {
                    hfDeleteForeignId.Value = "";
                    LoadForeignDemandsList();
                    ShowAlert("success", "Deleted", "Demand removed successfully.");
                }
                else
                {
                    ShowAlert("error", "Error", "Record could not be deleted.");
                }
            }
        }

        private void ClearForeignForm()
        {
            hfEditingForeignId.Value = ""; txtForeignExplanation.Text = ""; txtForeignStartDate.Text = ""; txtForeignEndDate.Text = "";
            btnForeignSend.Text = "<i class='fas fa-save me-2'></i> SAVE DEMAND"; btnCancelEdit.Visible = false;
        }

        protected void btnCancelEdit_Click(object sender, EventArgs e) { ClearForeignForm(); }

        private void ShowAlert(string type, string title, string text)
        {
            string cleanText = chk.temizle(text).Replace("'", "\\'").Replace("\n", " ").Replace("\r", " ");
            ScriptManager.RegisterStartupScript(this, GetType(), "SweetAlert", $"Swal.fire({{ icon: '{type}', title: '{title}', text: '{cleanText}' }});", true);
        }

        private void FillFormDetails(string reqId)
        {
            DataTable dt = db.SqlToDt("SELECT * FROM Requests WHERE RequestId = " + chk.temizle(reqId));
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];

                hfSelectedRequestId.Value = dr["RequestId"].ToString();
                txtTopic.Text = dr["Topic"].ToString();
                txtDescription.Text = dr["Description"].ToString();

                if (ddlRequestOwner.Items.FindByValue(dr["UserID"].ToString()) != null)
                    ddlRequestOwner.SelectedValue = dr["UserID"].ToString();

                if (ddlRelatedUser.Items.FindByValue(dr["RelatedUserID"].ToString()) != null)
                    ddlRelatedUser.SelectedValue = dr["RelatedUserID"].ToString();

                
            }
        }
    }
}