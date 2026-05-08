using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ITSM
{
    public partial class UserToDevice : System.Web.UI.Page
    {
        DBTools db = new DBTools();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string userId = Request.QueryString["userid"];

                if (!string.IsNullOrEmpty(userId))
                {
                    GetUserInfo(userId);
                    FillDevices();
                    GetUserDevice(userId);

                    txtStartDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

                    if (Session["AlertType"] != null && Session["AlertMessage"] != null)
                    {
                        ShowAlert(Session["AlertType"].ToString(), "Success", Session["AlertMessage"].ToString());
                        Session.Remove("AlertType");
                        Session.Remove("AlertMessage");
                    }
                }
                else
                {
                    Response.Redirect("Users.aspx");
                }
            }
        }

        private void ShowAlert(string type, string title, string message)
        {
            hfAlertType.Value = type;
            hfAlertTitle.Value = title;
            hfAlertMessage.Value = message;
        }


        private void FillDevices()
        {
            try
            {

                string sql = @"SELECT d.DeviceID, d.Brand + ' ' + d.Model + ' | ' + d.SerialNumber as DeviceName 
                       FROM device d 
                       WHERE (d.IsDeleted IS NULL OR d.IsDeleted <> 'X') 
                       AND d.DeviceID NOT IN (
                           SELECT DeviceID FROM UserDevice WHERE (isDeleted IS NULL OR isDeleted <> '1')
                       ) 
                       ORDER BY d.Brand, d.Model";

                DataTable dt = db.SqlToDt(sql);
                ddlDevices.DataSource = dt;
                ddlDevices.DataTextField = "DeviceName";
                ddlDevices.DataValueField = "DeviceID";
                ddlDevices.DataBind();
                ddlDevices.Items.Insert(0, new ListItem("-- Select Device --", ""));
            }
            catch (Exception ex) { ShowAlert("error", "Error", "Devices could not be loaded: " + ex.Message); }
        }

        private void GetUserDevice(string userId)
        {
            try
            {

                string sql = $@"SELECT ud.UserDeviceID, d.Brand, d.Model, d.SerialNumber, ud.StartDate, ud.EndDate 
                        FROM UserDevice ud 
                        INNER JOIN Device d ON ud.DeviceID = d.DeviceID 
                        WHERE ud.UserID = {userId} AND (ud.isDeleted IS NULL OR ud.isDeleted <> '1')";
                DataTable dt = db.SqlToDt(sql);

                if (dt.Rows.Count > 0)
                {
                    rptUserDevices.DataSource = dt;
                    rptUserDevices.DataBind();
                    pnlNoDevice.Visible = false;
                }
                else
                {

                    pnlNoDevice.Visible = true;
                }
            }
            catch (Exception ex) { ShowAlert("error", "Error", "Assignments could not be loaded: " + ex.Message); }
        }


        protected void btnAssign_Click(object sender, EventArgs e)
        {
            string userId = Request.QueryString["userid"];
            string deviceId = ddlDevices.SelectedValue;
            string startDate = txtStartDate.Text;
            string endDate = txtEndDate.Text;
            string editId = hfEditUserDeviceID.Value;

            if (string.IsNullOrEmpty(deviceId) || string.IsNullOrEmpty(startDate))
            {
                ShowAlert("warning", "Missing Info", "Please select a device and a start date.");
                return;
            }

            try
            {
                string endDateValue = string.IsNullOrEmpty(endDate) ? "NULL" : "'" + endDate + "'";
                string sql = "";
                string successMessage = "";

                if (string.IsNullOrEmpty(editId))
                {
                    sql = "INSERT INTO UserDevice (UserID, DeviceID, StartDate, EndDate, Status) VALUES (" + userId + ", " + deviceId + ", '" + startDate + "', " + endDateValue + ", 1)";
                    successMessage = "Device assigned successfully.";
                }
                else
                {
                    sql = "UPDATE UserDevice SET DeviceID = " + deviceId + ", StartDate = '" + startDate + "', EndDate = " + endDateValue + ", UpdateDate = GETDATE() WHERE UserDeviceID = " + editId;
                    successMessage = "Assignment updated successfully.";
                }

                db.SqlToDt(sql);
                Session["AlertType"] = "success";
                Session["AlertMessage"] = successMessage;
                Response.Redirect("UserToDevice.aspx?userid=" + userId);
            }
            catch (Exception ex)
            {
                ShowAlert("error", "Database Error", ex.Message);
            }
        }

        protected void btnDeleteConfirm_Click(object sender, EventArgs e)
        {
            string deleteId = hfDeleteId.Value;
            if (!string.IsNullOrEmpty(deleteId))
            {
                try
                {
                    string sql = "UPDATE UserDevice SET isDeleted = '1' WHERE UserDeviceID = " + deleteId;
                    db.SqlToDt(sql);
                    Session["AlertType"] = "success";
                    Session["AlertMessage"] = "Assignment deleted successfully.";
                    Response.Redirect("UserToDevice.aspx?userid=" + Request.QueryString["userid"]);
                }
                catch (Exception ex)
                {
                    ShowAlert("error", "Error", "Error during deletion: " + ex.Message);
                }
            }
        }

        protected void rptUserDevices_ItemCommand(object source, RepeaterCommandEventArgs e)
        {

            if (e.CommandName == "EditDevice")
            {
                int deviceId = Convert.ToInt32(e.CommandArgument);
                string userDeviceId = e.CommandArgument.ToString();
                hfEditUserDeviceID.Value = userDeviceId;

                try
                {
                    string sql = @"SELECT ud.DeviceID, d.Brand + ' ' + d.Model + ' | ' + d.SerialNumber AS DeviceName, ud.StartDate, ud.EndDate
                           FROM UserDevice ud 
                           INNER JOIN Device d ON ud.DeviceID = d.DeviceID
                           WHERE ud.UserDeviceID = " + userDeviceId;

                    DataTable dt = db.SqlToDt(sql);
                    if (dt.Rows.Count > 0)
                    {
                        string dbId = dt.Rows[0]["DeviceID"].ToString();


                        if (ddlDevices.Items.FindByValue(dbId) == null)
                        {
                            ddlDevices.Items.Add(new ListItem(dt.Rows[0]["DeviceName"].ToString(), dbId));
                        }

                        ddlDevices.SelectedValue = dbId;
                        txtStartDate.Text = dt.Rows[0]["StartDate"] != DBNull.Value ? Convert.ToDateTime(dt.Rows[0]["StartDate"]).ToString("yyyy-MM-dd") : "";
                        txtEndDate.Text = dt.Rows[0]["EndDate"] != DBNull.Value ? Convert.ToDateTime(dt.Rows[0]["EndDate"]).ToString("yyyy-MM-dd") : "";


                        ScriptManager.RegisterStartupScript(this, GetType(), "reinitSelect2", "initUserDeviceSelect2();", true);
                    }
                }
                catch (Exception ex)
                {
                    ShowAlert("error", "Error", "Details could not be loaded: " + ex.Message);
                }
            }
        }

        private void GetUserInfo(string userId)
        {
            try
            {
                DataTable dt = db.SqlToDt("SELECT NameSurname FROM Users WHERE UserID = " + userId);
                if (dt.Rows.Count > 0) lblUserName.Text = dt.Rows[0]["NameSurname"].ToString() + " - Assigned Devices";
            }
            catch { lblUserName.Text = "User Devices"; }
        }
    }
}