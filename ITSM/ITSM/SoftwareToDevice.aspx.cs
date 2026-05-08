using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ITSM
{
    public partial class SoftwareToDevice : System.Web.UI.Page
    {
        DBTools db = new DBTools();
        protected global::System.Web.UI.WebControls.HiddenField hfDeleteId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string softwareId = Request.QueryString["id"];
                if (!string.IsNullOrEmpty(softwareId))
                {
                    LoadDevices();
                    LoadSoftwareInfo(softwareId);
                    FetchData(softwareId);
                }
                else
                {
                    Response.Redirect("Software.aspx");
                }
            }
        }

        private void LoadDevices()
        {
            // URL'den mevcut yazılım ID'sini alıyoruz
            string softwareId = Request.QueryString["id"];
            if (string.IsNullOrEmpty(softwareId)) return;

            // Sorgu: Cihazları getir ama DeviceSoftware tablosunda bu yazılım ile eşleşenleri hariç tut
            string sql = string.Format(@"
        SELECT DeviceID, (Model + ' | ' + ISNULL(SerialNumber, 'No SN')) AS DeviceInfo 
        FROM Device 
        WHERE DeleteDate IS NULL 
        AND DeviceID NOT IN (
            SELECT DeviceID FROM DeviceSoftware WHERE SoftwareID = {0}
        )
        ORDER BY Model", softwareId);

            DataTable dt = db.SqlToDt(sql);

            ddlModels.Items.Clear();
            ddlModels.Items.Add(new ListItem("-- Select Device --", "0"));

            if (dt != null && dt.Rows.Count > 0)
            {
                ddlModels.DataSource = dt;
                ddlModels.DataTextField = "DeviceInfo";
                ddlModels.DataValueField = "DeviceID";
                ddlModels.DataBind();
            }
        }
        protected void rptDeviceSoftware_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Edit")
            {
                string deviceId = e.CommandArgument.ToString();
                string softwareId = Request.QueryString["id"];

                string sql = string.Format("SELECT * FROM DeviceSoftware WHERE DeviceID={0} AND SoftwareID={1}", deviceId, softwareId);
                DataTable dt = db.SqlToDt(sql);

                if (dt != null && dt.Rows.Count > 0)
                {
                    hfIsEdit.Value = "1";
                    hfSelectedDeviceID.Value = deviceId;
                    ListItem item = ddlModels.Items.FindByValue(deviceId);
                    if (item != null)
                    {
                        ddlModels.ClearSelection();
                        item.Selected = true;
                        ScriptManager.RegisterStartupScript(this, GetType(), "updateSelect2",
                            "setTimeout(function() { initMySelect2(); $('#ddlModels').trigger('change'); }, 150);", true);
                    }

                    ddlModels.Enabled = false;
                    txtStartDate.Text = dt.Rows[0]["StartDate"] != DBNull.Value ? Convert.ToDateTime(dt.Rows[0]["StartDate"]).ToString("yyyy-MM-dd") : "";
                    txtEndDate.Text = dt.Rows[0]["EndDate"] != DBNull.Value ? Convert.ToDateTime(dt.Rows[0]["EndDate"]).ToString("yyyy-MM-dd") : "";
                    btnSave.Text = "UPDATE";
                    ScriptManager.RegisterStartupScript(this, GetType(), "scrollUp", "window.scrollTo({ top: 0, behavior: 'smooth' });", true);
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            
            if (ddlModels.SelectedValue == "0" && hfIsEdit.Value == "0")
            {
                ShowAlert("warning", "Please select a device.");
                return;
            }

            string softwareId = Request.QueryString["id"];
            if (string.IsNullOrEmpty(softwareId))
            {
                ShowAlert("error", "Software ID not found.");
                return;
            }

            if (hfIsEdit.Value == "0")
            {
                string checkSql = string.Format(@"
            SELECT 
                (SELECT ISNULL(UserCount, 0) FROM Software WHERE SoftwareID = {0}) as MaxCount,
                (SELECT COUNT(*) FROM DeviceSoftware WHERE SoftwareID = {0}) as CurrentCount", softwareId);

                DataTable dtCheck = db.SqlToDt(checkSql);

                if (dtCheck != null && dtCheck.Rows.Count > 0)
                {
                    int maxCount = 0;
                    int currentCount = 0;
                    int.TryParse(dtCheck.Rows[0]["MaxCount"].ToString(), out maxCount);
                    int.TryParse(dtCheck.Rows[0]["CurrentCount"].ToString(), out currentCount);

                    if (maxCount > 0 && currentCount >= maxCount)
                    {
                        ShowAlert("warning", "License limit reached! Maximum capacity: " + maxCount);
                        return;
                    }
                }
            }

            string deviceId = (hfIsEdit.Value == "0") ? ddlModels.SelectedValue : hfSelectedDeviceID.Value;
            string sDate = string.IsNullOrEmpty(txtStartDate.Text) ? "NULL" : string.Format("'{0}'", txtStartDate.Text);
            string eDate = string.IsNullOrEmpty(txtEndDate.Text) ? "NULL" : string.Format("'{0}'", txtEndDate.Text);
            string currentUserId = "1";
            string sql;

            if (hfIsEdit.Value == "0")
            {
                sql = string.Format(@"INSERT INTO DeviceSoftware (DeviceID, SoftwareID, StartDate, EndDate, RecordDate, RecordUser) 
                            VALUES ({0}, {1}, {2}, {3}, GETDATE(), '{4}')",
                                            deviceId, softwareId, sDate, eDate, currentUserId);
            }
            else
            {
                sql = string.Format(@"UPDATE DeviceSoftware SET StartDate={0}, EndDate={1}, UpdateDate=GETDATE(), UpdateUser='{2}' 
                            WHERE DeviceID={3} AND SoftwareID={4}",
                                            sDate, eDate, currentUserId, deviceId, softwareId);
            }

            try
            {
                if (db.ExecStr(sql) > 0)
                {
                    FetchData(softwareId);
                    ResetForm();
                    LoadDevices(); 
                    ShowAlert("success", "Record saved successfully.");
                }
                else
                {
                    ShowAlert("error", "Database error: " + db.Hata.Replace("'", ""));
                }
            }
            catch (Exception ex)
            {
                ShowAlert("error", "System error: " + ex.Message.Replace("'", ""));
            }
        }
        private void FetchData(string softwareId)
        {
            string sql = string.Format(@"SELECT DS.DeviceID, D.Model, D.SerialNumber as DeviceNo, DS.StartDate, DS.EndDate 
                                 FROM DeviceSoftware DS
                                 LEFT JOIN Device D ON DS.DeviceID = D.DeviceID
                                 WHERE DS.SoftwareID = {0} 
                                 AND (DS.isDeleted IS NULL OR DS.isDeleted = 0)", softwareId);

            DataTable dt = db.SqlToDt(sql);
            rptDeviceSoftware.DataSource = dt;
            rptDeviceSoftware.DataBind();

            if (dt != null && dt.Rows.Count > 0)
            {
                pnlEmpty.Visible = false;
                pnlTable.Visible = true;
            }
            else
            {
                pnlEmpty.Visible = true;
                pnlTable.Visible = false;
            }
        }
        protected void btnDeleteConfirm_Click(object sender, EventArgs e)
        {
            string deviceId = hfDeleteId.Value;
            string softwareId = Request.QueryString["id"];

            if (string.IsNullOrEmpty(deviceId)) return;

            string sql = string.Format("DELETE FROM DeviceSoftware WHERE DeviceID={0} AND SoftwareID={1}", deviceId, softwareId);
            if (db.ExecStr(sql) > 0)
            {
                FetchData(softwareId);
                LoadDevices(); 
                ShowAlert("success", "Deleted successfully.");
            }
            else
            {
                ShowAlert("error", "Error: " + db.Hata.Replace("'", ""));
            }
            hfDeleteId.Value = "";
        }

        private void ResetForm()
        {
            hfIsEdit.Value = "0";
            hfSelectedDeviceID.Value = "";
            ddlModels.SelectedIndex = 0;
            ddlModels.Enabled = true;
            txtStartDate.Text = "";
            txtEndDate.Text = "";
            btnSave.Text = "SAVE";
        }

        private void ShowAlert(string type, string msg)
        {
            hfAlertType.Value = type;
            hfAlertMessage.Value = msg;
            ScriptManager.RegisterStartupScript(this, GetType(), "alertScript", "showAlert();", true);
        }

        private void LoadSoftwareInfo(string softwareId)
        {
            DataTable dt = db.SqlToDt("SELECT SoftwareID, Description FROM Software WHERE SoftwareID = " + softwareId);
            if (dt != null && dt.Rows.Count > 0)
            {
                litSoftwareName.Text = dt.Rows[0]["Description"].ToString();
            }
        }


    }
}