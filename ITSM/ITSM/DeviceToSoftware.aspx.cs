using System;
using System.Data;
using System.Web.UI.WebControls;

namespace ITSM
{
    public partial class DeviceToSoftware : System.Web.UI.Page
    {
        DBTools db = new DBTools();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string deviceId = Request.QueryString["deviceId"];
                if (!string.IsNullOrEmpty(deviceId))
                {
                    FillDropdown();
                    GetSoftwareDetails(deviceId);
                    ResetForm();


                    if (Request.QueryString["status"] == "success")
                    {
                        ShowAlert("success", "Success", "Action completed successfully.");
                    }
                }
                else
                {
                    Response.Redirect("Device.aspx");
                }
            }
        }

        private void FillDropdown()
        {
            db.Sql2AddCombo(ddlSoftware, "SELECT SoftwareID, Description FROM Software WHERE isDelete IS NULL ORDER BY Description", "", "Description", "SoftwareID", false, "-- Select Software --");
        }

        private void GetSoftwareDetails(string deviceId)
        {

            string sql = "SELECT DS.DeviceSoftwareID, S.Description, " + Environment.NewLine;
            sql += "ISNULL(S.SoftwareType, '') as SoftwareType, " + Environment.NewLine;
            sql += "ISNULL(S.LicenseType, '') as LicenseType, " + Environment.NewLine;
            sql += "DS.StartDate, DS.EndDate " + Environment.NewLine;
            sql += "FROM DeviceSoftware DS " + Environment.NewLine;
            sql += "INNER JOIN Software S ON DS.SoftwareID = S.SoftwareID " + Environment.NewLine;
            sql += "WHERE DS.DeviceID = " + deviceId + " AND DS.isDeleted IS NULL " + Environment.NewLine;
            sql += "ORDER BY DS.StartDate DESC";

            DataTable dt = db.SqlToDt(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                pnlSoftwareDetails.Visible = true;
                phNoSoftware.Visible = false;
                rptSoftware.DataSource = dt;
                rptSoftware.DataBind();
            }
            else
            {
                pnlSoftwareDetails.Visible = false;
                phNoSoftware.Visible = true;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string deviceId = Request.QueryString["deviceId"];
            string softwareId = ddlSoftware.SelectedValue;
            string start = txtStartDate.Text.Replace("T", " ");
            string end = string.IsNullOrEmpty(txtEndDate.Text) ? "NULL" : "'" + txtEndDate.Text.Replace("T", " ") + "'";
            string selectedID = hfSelectedID.Value;

            if (ddlSoftware.SelectedIndex == 0 || string.IsNullOrEmpty(txtStartDate.Text))
            {
                ShowAlert("warning", "Missing Info", "Please select software and start date.");
                return;
            }

            try
            {
                string sql = "";
                if (string.IsNullOrEmpty(selectedID))
                {

                    sql = "INSERT INTO DeviceSoftware (DeviceID, SoftwareID, StartDate, EndDate, RecordDate, RecordUser) " + Environment.NewLine;
                    sql += "VALUES (" + deviceId + ", " + softwareId + ", '" + start + "', " + end + ", GETDATE(), 'Admin')";
                }
                else
                {

                    sql = "UPDATE DeviceSoftware SET " + Environment.NewLine;
                    sql += "SoftwareID = " + softwareId + ", " + Environment.NewLine;
                    sql += "StartDate = '" + start + "', " + Environment.NewLine;
                    sql += "EndDate = " + end + ", " + Environment.NewLine;
                    sql += "UpdateDate = GETDATE(), " + Environment.NewLine;
                    sql += "UpdateUser = 'Admin' " + Environment.NewLine;
                    sql += "WHERE DeviceSoftwareID = " + selectedID;
                }

                if (db.ExecStr(sql) != -1)
                {

                    Response.Redirect("DeviceToSoftware.aspx?deviceId=" + deviceId + "&status=success");
                }
                else
                {
                    ShowAlert("error", "Database Error", db.Hata);
                }
            }
            catch (Exception ex)
            {
                ShowAlert("error", "Error", ex.Message);
            }
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            string id = btn.CommandArgument;
            hfSelectedID.Value = id;

            DataTable dt = db.SqlToDt("SELECT SoftwareID, StartDate, EndDate FROM DeviceSoftware WHERE DeviceSoftwareID = " + id);
            if (dt.Rows.Count > 0)
            {
                ddlSoftware.SelectedValue = dt.Rows[0]["SoftwareID"].ToString();
                if (dt.Rows[0]["StartDate"] != DBNull.Value)
                    txtStartDate.Text = Convert.ToDateTime(dt.Rows[0]["StartDate"]).ToString("yyyy-MM-ddTHH:mm");
                if (dt.Rows[0]["EndDate"] != DBNull.Value)
                    txtEndDate.Text = Convert.ToDateTime(dt.Rows[0]["EndDate"]).ToString("yyyy-MM-ddTHH:mm");


                btnSave.Text = "<i class='fal fa-sync'></i>";
                btnSave.CssClass = "btn btn-outline-warning btn-sm";
                btnSave.ToolTip = "Update Assignment";
            }
        }

        protected void btnDeleteConfirm_Click(object sender, EventArgs e)
        {
            string id = hfDeleteId.Value;
            string deviceId = Request.QueryString["deviceId"];


            string sql = "UPDATE DeviceSoftware SET " + Environment.NewLine;
            sql += "isDeleted = 'X', " + Environment.NewLine;
            sql += "DeleteDate = GETDATE(), " + Environment.NewLine;
            sql += "DeleteUser = 'Admin' " + Environment.NewLine;
            sql += "WHERE DeviceSoftwareID = " + id;

            if (db.ExecStr(sql) != -1)
            {

                Response.Redirect("DeviceToSoftware.aspx?deviceId=" + deviceId + "&status=success");
            }
            hfDeleteId.Value = "";
        }

        private void ResetForm()
        {
            hfSelectedID.Value = "";
            ddlSoftware.SelectedIndex = 0;
            txtStartDate.Text = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
            txtEndDate.Text = "";


            btnSave.Text = "<i class='fal fa-save'></i>";
            btnSave.CssClass = "btn btn-outline-primary btn-sm";
            btnSave.ToolTip = "Save Assignment";
        }

        private void ShowAlert(string type, string title, string message)
        {
            hfAlertType.Value = type;
            hfAlertTitle.Value = title;
            hfAlertMessage.Value = message;
        }
    }
}