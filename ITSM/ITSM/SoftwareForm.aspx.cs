
using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ITSM
{
    public partial class SoftwareForm : System.Web.UI.Page
    {
        DBTools db = new DBTools();
        CookieTools cookie = new CookieTools();

        private string GetCurrentUser()
        {
            string currentUser = cookie.Oku("User");
            if (string.IsNullOrEmpty(currentUser))
                currentUser = "System";
            return currentUser;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    DataTable dtTemp = new DataTable();
                    dtTemp.Columns.Add("SoftwarePurchaseID", typeof(int));
                    dtTemp.Columns.Add("PurchaseDate", typeof(DateTime));
                    dtTemp.Columns.Add("LicenseStart", typeof(DateTime));
                    dtTemp.Columns.Add("LicenseEnd", typeof(DateTime));
                    dtTemp.Columns.Add("RecordDate", typeof(DateTime));
                    dtTemp.Columns.Add("RecordUser", typeof(string));
                    ViewState["TempPurchases"] = dtTemp;

                    string id = Request.QueryString["id"];
                    if (!string.IsNullOrEmpty(id))
                    {
                        LoadData(id);
                    }
                    else
                    {
                        BindGrid();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowAlert("error", "Error", "Page load error: " + ex.Message);
            }
        }
        private void BindGrid()
        {
            DataTable dtFinal = new DataTable();
            dtFinal.Columns.Add("SoftwarePurchaseID", typeof(int));
            dtFinal.Columns.Add("PurchaseDate", typeof(DateTime));
            dtFinal.Columns.Add("LicenseStart", typeof(DateTime));
            dtFinal.Columns.Add("LicenseEnd", typeof(DateTime));
            dtFinal.Columns.Add("RecordDate", typeof(DateTime));
            dtFinal.Columns.Add("RecordUser", typeof(string));

            if (!string.IsNullOrEmpty(hfID.Value))
            {
                string sql = "SELECT SoftwarePurchaseID, PurchaseDate, LicenseStart, LicenseEnd, RecordDate, RecordUser FROM SoftwarePurchase WHERE SoftwareID = " + hfID.Value + " ORDER BY LicenseEnd DESC";
                DataTable dtDB = db.SqlToDt(sql);
                if (dtDB != null)
                {
                    foreach (DataRow row in dtDB.Rows)
                    {
                        dtFinal.ImportRow(row);
                    }
                }
            }

            if (ViewState["TempPurchases"] != null)
            {
                DataTable dtTemp = (DataTable)ViewState["TempPurchases"];
                foreach (DataRow row in dtTemp.Rows)
                {
                    dtFinal.ImportRow(row);
                }
            }

            DataView dv = dtFinal.DefaultView;
            dv.Sort = "LicenseEnd DESC";
            gvPurchases.DataSource = dv;
            gvPurchases.DataBind();
        }

        private void LoadData(string id)
        {
            try
            {
                string sql = "SELECT * FROM Software WHERE SoftwareID = " + id;
                DataTable dt = db.SqlToDt(sql);

                if (dt.Rows.Count > 0)
                {
                    hfID.Value = id;
                    txtDescription.Text = dt.Rows[0]["Description"].ToString();
                    txtLicenseNumber.Text = dt.Rows[0]["LicenseNumber"].ToString();
                    txtUserCount.Text = dt.Rows[0]["UserCount"].ToString();
                    txtAccountingCode.Text = dt.Rows[0]["AccountingCode"].ToString();

                    if (ddlStatus.Items.FindByValue(dt.Rows[0]["Status"].ToString()) != null)
                        ddlStatus.SelectedValue = dt.Rows[0]["Status"].ToString();

                    if (dt.Rows[0]["PurchaseDate"] != DBNull.Value)
                        txtPurchaseDate.Text = Convert.ToDateTime(dt.Rows[0]["PurchaseDate"]).ToString("yyyy-MM-dd");

                    if (dt.Rows[0]["ExpiryDate"] != DBNull.Value)
                        txtExpiryDate.Text = Convert.ToDateTime(dt.Rows[0]["ExpiryDate"]).ToString("yyyy-MM-dd");

                    if (ddlSoftwareType.Items.FindByValue(dt.Rows[0]["SoftwareType"].ToString()) != null)
                        ddlSoftwareType.SelectedValue = dt.Rows[0]["SoftwareType"].ToString();

                    if (ddlLicenseType.Items.FindByValue(dt.Rows[0]["LicenseType"].ToString()) != null)
                        ddlLicenseType.SelectedValue = dt.Rows[0]["LicenseType"].ToString();

                    BindGrid();
                    LoadAttachments(id);
                }
            }
            catch (Exception ex)
            {
                ShowAlert("error", "Error", "Data load error: " + ex.Message);
            }
        }

        protected void btnAddPurchase_Click(object sender, EventArgs e)
        {
            Page.Validate("AddGroup");
            if (!Page.IsValid) return;

            try
            {
                DataTable dtTemp = (DataTable)ViewState["TempPurchases"];
                DataRow dr = dtTemp.NewRow();

                int tempId = -1;
                if (dtTemp.Rows.Count > 0)
                    tempId = Convert.ToInt32(dtTemp.Compute("MIN(SoftwarePurchaseID)", string.Empty)) - 1;

                dr["SoftwarePurchaseID"] = tempId;
                dr["PurchaseDate"] = Convert.ToDateTime(txtNewPurchaseDate.Text);
                dr["LicenseStart"] = Convert.ToDateTime(txtNewLicenseStart.Text);
                dr["LicenseEnd"] = Convert.ToDateTime(txtNewLicenseEnd.Text);
                dr["RecordDate"] = DateTime.Now;
                dr["RecordUser"] = GetCurrentUser();

                dtTemp.Rows.Add(dr);
                ViewState["TempPurchases"] = dtTemp;

                txtNewPurchaseDate.Text = "";
                txtNewLicenseStart.Text = "";
                txtNewLicenseEnd.Text = "";

                BindGrid();
                Page.Validate("SaveGroup");
                ScriptManager.RegisterStartupScript(this, GetType(), "ScrollTop", "window.scrollTo({ top: 0, behavior: 'smooth' });", true);
            }
            catch (Exception ex)
            {
                ShowAlert("error", "System Error", "Lütfen geçerli tarihler giriniz.");
            }
        }

        protected void gvPurchases_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteRecord")
            {
                try
                {
                    int id = Convert.ToInt32(e.CommandArgument);

                    if (id < 0)
                    {
                        DataTable dtTemp = (DataTable)ViewState["TempPurchases"];
                        DataRow[] rows = dtTemp.Select("SoftwarePurchaseID = " + id);
                        if (rows.Length > 0)
                        {
                            dtTemp.Rows.Remove(rows[0]);
                            ViewState["TempPurchases"] = dtTemp;
                            BindGrid();
                        }
                    }
                    else
                    {
                        string sql = "DELETE FROM SoftwarePurchase WHERE SoftwarePurchaseID = " + id;
                        int result = db.ExecStr(sql);

                        if (result > 0)
                        {
                            BindGrid();
                            ShowAlert("success", "Deleted", "The license history record has been deleted successfully.");
                        }
                        else
                        {
                            ShowAlert("error", "Error", "Record could not be deleted: " + db.Hata.Replace("'", ""));
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowAlert("error", "System Error", ex.Message);
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            try
            {
                string currentUser = GetCurrentUser();
                string sql = "";
                int result = 0;
                string currentSoftwareID = hfID.Value;

                if (string.IsNullOrEmpty(currentSoftwareID))
                {
                    sql = "INSERT INTO Software " + Environment.NewLine;
                    sql += "(Description, LicenseNumber, LicenseType, SoftwareType, PurchaseDate, ExpiryDate, UserCount, AccountingCode, Status, RecordDate, RecordUser) " + Environment.NewLine;
                    sql += "VALUES (" + Environment.NewLine;
                    sql += "'" + txtDescription.Text.Replace("'", "''") + "', " + Environment.NewLine;
                    sql += "'" + txtLicenseNumber.Text.Replace("'", "''") + "', " + Environment.NewLine;
                    sql += "'" + ddlLicenseType.SelectedValue + "', " + Environment.NewLine;
                    sql += "'" + ddlSoftwareType.SelectedValue + "', " + Environment.NewLine;
                    sql += (string.IsNullOrEmpty(txtPurchaseDate.Text) ? "NULL" : "'" + txtPurchaseDate.Text + "'") + ", " + Environment.NewLine;
                    sql += (string.IsNullOrEmpty(txtExpiryDate.Text) ? "NULL" : "'" + txtExpiryDate.Text + "'") + ", " + Environment.NewLine;
                    sql += (string.IsNullOrEmpty(txtUserCount.Text) ? "0" : txtUserCount.Text) + ", " + Environment.NewLine;
                    sql += "'" + txtAccountingCode.Text.Replace("'", "''") + "', " + Environment.NewLine;
                    sql += ddlStatus.SelectedValue + ", " + Environment.NewLine;
                    sql += "GETDATE(), " + Environment.NewLine;
                    sql += "'" + currentUser + "')";

                    result = db.ExecStr(sql);
                    if (result > 0)
                    {
                        DataTable dtId = db.SqlToDt("SELECT MAX(SoftwareID) as SonID FROM Software");
                        if (dtId != null && dtId.Rows.Count > 0)
                        {
                            currentSoftwareID = dtId.Rows[0]["SonID"].ToString();
                            hfID.Value = currentSoftwareID;
                        }
                    }
                }
                else
                {
                    sql = "UPDATE Software SET " + Environment.NewLine;
                    sql += "Description = '" + txtDescription.Text.Replace("'", "''") + "', " + Environment.NewLine;
                    sql += "LicenseNumber = '" + txtLicenseNumber.Text.Replace("'", "''") + "', " + Environment.NewLine;
                    sql += "LicenseType = '" + ddlLicenseType.SelectedValue + "', " + Environment.NewLine;
                    sql += "SoftwareType = '" + ddlSoftwareType.SelectedValue + "', " + Environment.NewLine;
                    sql += "PurchaseDate = " + (string.IsNullOrEmpty(txtPurchaseDate.Text) ? "NULL" : "'" + txtPurchaseDate.Text + "'") + ", " + Environment.NewLine;
                    sql += "ExpiryDate = " + (string.IsNullOrEmpty(txtExpiryDate.Text) ? "NULL" : "'" + txtExpiryDate.Text + "'") + ", " + Environment.NewLine;
                    sql += "UserCount = " + (string.IsNullOrEmpty(txtUserCount.Text) ? "0" : txtUserCount.Text) + ", " + Environment.NewLine;
                    sql += "AccountingCode = '" + txtAccountingCode.Text.Replace("'", "''") + "', " + Environment.NewLine;
                    sql += "Status = " + ddlStatus.SelectedValue + ", " + Environment.NewLine;
                    sql += "UpdateDate = GETDATE(), " + Environment.NewLine;
                    sql += "UpdateUser = '" + currentUser + "' " + Environment.NewLine;
                    sql += "WHERE SoftwareID = " + hfID.Value;

                    result = db.ExecStr(sql);
                }
                if (fuAttachment.HasFiles)
                {
                    string uploadDir = Server.MapPath("~/Uploads/");
                    if (!System.IO.Directory.Exists(uploadDir))
                        System.IO.Directory.CreateDirectory(uploadDir);

                    foreach (var file in fuAttachment.PostedFiles)
                    {
                        string fileName = System.IO.Path.GetFileName(file.FileName);
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + fileName;
                        string filePath = "~/Uploads/" + uniqueFileName;

                        file.SaveAs(Server.MapPath(filePath));

                        string sqlFile = $@"INSERT INTO SoftwareAttachment (SoftwareID, FileName, FilePath, RecordDate, RecordUser) 
                            VALUES ({currentSoftwareID}, '{fileName}', '{filePath}', GETDATE(), '{currentUser}')";
                        db.ExecStr(sqlFile);
                    }
                }

                if (result > 0)
                {
                    if (ViewState["TempPurchases"] != null)
                    {
                        DataTable dtTemp = (DataTable)ViewState["TempPurchases"];
                        foreach (DataRow row in dtTemp.Rows)
                        {
                            string pDate = row["PurchaseDate"] != DBNull.Value ? "'" + Convert.ToDateTime(row["PurchaseDate"]).ToString("yyyy-MM-dd") + "'" : "NULL";
                            string lStart = row["LicenseStart"] != DBNull.Value ? "'" + Convert.ToDateTime(row["LicenseStart"]).ToString("yyyy-MM-dd") + "'" : "NULL";
                            string lEnd = row["LicenseEnd"] != DBNull.Value ? "'" + Convert.ToDateTime(row["LicenseEnd"]).ToString("yyyy-MM-dd") + "'" : "NULL";

                            string sqlPurchase = $@"INSERT INTO SoftwarePurchase 
                    (SoftwareID, PurchaseDate, LicenseStart, LicenseEnd, RecordDate, RecordUser) 
                    VALUES ({currentSoftwareID}, {pDate}, {lStart}, {lEnd}, GETDATE(), '{currentUser}')";

                            db.ExecStr(sqlPurchase);
                        }

                        ViewState["TempPurchases"] = null;
                    }

                    string updateDatesSql = @"UPDATE Software SET 
                PurchaseDate = (SELECT TOP 1 PurchaseDate FROM SoftwarePurchase WHERE SoftwareID = Software.SoftwareID ORDER BY LicenseEnd DESC),
                ExpiryDate = (SELECT TOP 1 LicenseEnd FROM SoftwarePurchase WHERE SoftwareID = Software.SoftwareID ORDER BY LicenseEnd DESC)
                WHERE SoftwareID = " + currentSoftwareID;
                    db.ExecStr(updateDatesSql);

                    ShowAlert("success", "Success", "Record saved successfully.", "Software.aspx");
                }
                else
                {
                    ShowAlert("error", "Error", "Save error: " + db.Hata.Replace("'", ""));
                }
            }
            catch (Exception ex)
            {
                ShowAlert("error", "Error", "System error: " + ex.Message);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Software.aspx");
        }


        private void LoadAttachments(string softwareID)
        {

            string sql = "SELECT * FROM SoftwareAttachment WHERE SoftwareID = " + softwareID;
            DataTable dt = db.SqlToDt(sql);
            rptAttachments.DataSource = dt;
            rptAttachments.DataBind();
        }

        protected void rptAttachments_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DeleteFile")
            {
                string attachmentId = e.CommandArgument.ToString();
                db.ExecStr("DELETE FROM SoftwareAttachment WHERE SoftwareAttachmentID = " + attachmentId);
                LoadAttachments(hfID.Value);
            }
        }

        private void ShowAlert(string type, string title, string message, string redirectUrl = "")
        {
            hfAlertType.Value = type;
            hfAlertTitle.Value = title;
            hfAlertMessage.Value = message;
            hfRedirectUrl.Value = redirectUrl;
        }
    }
}