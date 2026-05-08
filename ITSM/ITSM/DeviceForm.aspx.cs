using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

namespace ITSM
{
    public partial class DeviceForm : System.Web.UI.Page
    {

        [Serializable]
        public class SelectedAddDevice
        {
            public string AD_ID { get; set; }
            public string DeviceInfo { get; set; }
        }

        private List<SelectedAddDevice> CurrentSelectedAddDevices
        {
            get
            {
                if (ViewState["SelectedAddDevices"] == null)
                    ViewState["SelectedAddDevices"] = new List<SelectedAddDevice>();
                return (List<SelectedAddDevice>)ViewState["SelectedAddDevices"];
            }
            set { ViewState["SelectedAddDevices"] = value; }
        }



        [Serializable]
        public class SelectedAssignment
        {
            public string UserDeviceID { get; set; }
            public string UserID { get; set; }
            public string NameSurname { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime? EndDate { get; set; }
        }

        private List<SelectedAssignment> CurrentAssignments
        {
            get
            {
                if (ViewState["CurrentAssignments"] == null)
                    ViewState["CurrentAssignments"] = new List<SelectedAssignment>();
                return (List<SelectedAssignment>)ViewState["CurrentAssignments"];
            }
            set { ViewState["CurrentAssignments"] = value; }
        }


        private void BindAssignments()
        {
            rptAssignments.DataSource = CurrentAssignments;
            rptAssignments.DataBind();
        }

        DBTools db = new DBTools();
        CookieTools cookie = new CookieTools();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                LoadAdditionalDevices();


                LoadUsers();

                string id = Request.QueryString["id"];
                if (!string.IsNullOrEmpty(id))
                {

                    VeriGetir(id);


                    string relSql = "SELECT AD.AD_ID, (AD.DeviceType + ' - ' + AD.Description + ' [SN: ' + ISNULL(AD.AccountingCode, 'N/A') + ']') as Info " +
                                    "FROM DeviceRel DR " +
                                    "INNER JOIN AdditionalDevices AD ON DR.SecondaryDevice = AD.AD_ID " +
                                    "WHERE DR.PrimaryDevice = " + id + " AND DR.Status = 1 AND AD.IsDelete IS NULL";

                    DataTable dtRel = db.SqlToDt(relSql);
                    List<SelectedAddDevice> loadedDevices = new List<SelectedAddDevice>();

                    if (dtRel != null && dtRel.Rows.Count > 0)
                    {
                        foreach (DataRow row in dtRel.Rows)
                        {
                            loadedDevices.Add(new SelectedAddDevice
                            {
                                AD_ID = row["AD_ID"].ToString(),
                                DeviceInfo = row["Info"].ToString()
                            });
                        }
                    }


                    CurrentSelectedAddDevices = loadedDevices;
                    BindLinkedDevices();


                    LoadAssignments(id);
                }
            }
        }

        private void LoadUsers()
        {

            string sql = "SELECT UserID, NameSurname FROM Users WHERE isDeleted IS NULL ORDER BY NameSurname";
            DataTable dtUsers = db.SqlToDt(sql);

            ddlUsers.DataSource = dtUsers;
            ddlUsers.DataTextField = "NameSurname";
            ddlUsers.DataValueField = "UserID";
            ddlUsers.DataBind();
            ddlUsers.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Select Personel --", "0"));
        }

        private void LoadAssignments(string deviceId)
        {

            string sql = @"SELECT ud.UserDeviceID, ud.UserID, u.NameSurname, ud.StartDate, ud.EndDate 
                   FROM UserDevice ud 
                   INNER JOIN Users u ON ud.UserID = u.UserID 
                   WHERE ud.DeviceID = " + deviceId + " AND ud.isDeleted IS NULL " +
                         " ORDER BY ud.StartDate DESC";

            DataTable dtAssignments = db.SqlToDt(sql);
            List<SelectedAssignment> loaded = new List<SelectedAssignment>();

            foreach (DataRow row in dtAssignments.Rows)
            {
                loaded.Add(new SelectedAssignment
                {
                    UserDeviceID = row["UserDeviceID"].ToString(),
                    UserID = row["UserID"].ToString(),
                    NameSurname = row["NameSurname"].ToString(),
                    StartDate = Convert.ToDateTime(row["StartDate"]),
                    EndDate = row["EndDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["EndDate"]) : null
                });
            }

            CurrentAssignments = loaded;
            BindAssignments();
        }
        protected void btnAddAssignment_Click(object sender, EventArgs e)
        {
            try
            {
                string userId = ddlUsers.SelectedValue;
                if (userId == "0" || string.IsNullOrEmpty(userId))
                {
                    ShowAlert("warning", "Warning", "Please select a personnel.");
                    return;
                }

                string userDeviceId = hfSelectedUserDeviceID.Value;
                var list = CurrentAssignments;

                DateTime startDate = string.IsNullOrEmpty(txtAssignStartDate.Text) ? DateTime.Now : Convert.ToDateTime(txtAssignStartDate.Text);
                DateTime? endDate = string.IsNullOrEmpty(txtAssignEndDate.Text) ? (DateTime?)null : Convert.ToDateTime(txtAssignEndDate.Text);

                if (string.IsNullOrEmpty(userDeviceId))
                {

                    list.Add(new SelectedAssignment
                    {
                        UserDeviceID = "TEMP_" + Guid.NewGuid().ToString("N").Substring(0, 8),
                        UserID = userId,
                        NameSurname = ddlUsers.SelectedItem.Text,
                        StartDate = startDate,
                        EndDate = endDate
                    });
                }
                else
                {

                    var item = list.FirstOrDefault(x => x.UserDeviceID == userDeviceId);
                    if (item != null)
                    {
                        item.UserID = userId;
                        item.NameSurname = ddlUsers.SelectedItem.Text;
                        item.StartDate = startDate;
                        item.EndDate = endDate;
                    }
                }

                CurrentAssignments = list;
                BindAssignments();


                hfSelectedUserDeviceID.Value = "";
                txtAssignStartDate.Text = "";
                txtAssignEndDate.Text = "";
                ddlUsers.SelectedIndex = 0;
                btnAddAssignment.Text = "<i class='fal fa-plus'></i> Confirm";
                btnAddAssignment.CssClass = "btn btn-outline-primary w-100";
            }
            catch (Exception ex)
            {
                ShowAlert("error", "Error", ex.Message);
            }
        }

        protected void rptAssignments_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string userDeviceId = e.CommandArgument.ToString();
            var list = CurrentAssignments;

            if (e.CommandName == "DeleteAssign")
            {

                list.RemoveAll(x => x.UserDeviceID == userDeviceId);
                CurrentAssignments = list;
                BindAssignments();
            }
            else if (e.CommandName == "EditAssign")
            {
                var item = list.FirstOrDefault(x => x.UserDeviceID == userDeviceId);
                if (item != null)
                {
                    ddlUsers.SelectedValue = item.UserID;
                    txtAssignStartDate.Text = item.StartDate.ToString("yyyy-MM-dd");
                    txtAssignEndDate.Text = item.EndDate.HasValue ? item.EndDate.Value.ToString("yyyy-MM-dd") : "";

                    hfSelectedUserDeviceID.Value = item.UserDeviceID;
                    btnAddAssignment.Text = "<i class='fal fa-sync'></i> UPDATE";
                    btnAddAssignment.CssClass = "btn btn-warning w-100";
                }
            }
        }


        private void LoadAdditionalDevices()
        {
            string sql = "SELECT AD_ID, (DeviceType + ' - ' + Description + ' [SN: ' + ISNULL(AccountingCode, 'N/A') + ']') as Info FROM AdditionalDevices WHERE IsDelete IS NULL";
            DataTable dt = db.SqlToDt(sql);
            ddlAdditionalDevices.DataSource = dt;
            ddlAdditionalDevices.DataTextField = "Info";
            ddlAdditionalDevices.DataValueField = "AD_ID";
            ddlAdditionalDevices.DataBind();
            ddlAdditionalDevices.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Select Additional Device...", ""));
        }


        private void VeriGetir(string id)
        {
            try
            {
                string sql = "SELECT * FROM device WHERE DeviceID = " + id;
                DataTable dt = db.SqlToDt(sql);
                if (dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    txtBrand.Text = dr["Brand"].ToString();
                    txtModel.Text = dr["Model"].ToString();
                    txtSerial.Text = dr["SerialNumber"].ToString();


                    if (dr["DeviceTypeID"] != DBNull.Value)
                        ddlDeviceType.SelectedValue = dr["DeviceTypeID"].ToString();

                    txtCPU.Text = dr["CPU"].ToString();
                    txtRAM.Text = dr["RAM"].ToString();
                    txtDiskCapacity.Text = dr["DiskCapacity"].ToString();
                    string diskType = "";
                    if (dt.Columns.Contains("Disk1Type") && dr["Disk1Type"] != DBNull.Value)
                        diskType = dr["Disk1Type"].ToString();
                    if (!string.IsNullOrEmpty(diskType) && ddlDiskType.Items.FindByValue(diskType) != null)
                        ddlDiskType.SelectedValue = diskType;

                    txtDisk2Capacity.Text = (dt.Columns.Contains("Disk2Capacity") && dr["Disk2Capacity"] != DBNull.Value) ? dr["Disk2Capacity"].ToString() : "";

                    string disk2Type = (dt.Columns.Contains("Disk2Type") && dr["Disk2Type"] != DBNull.Value) ? dr["Disk2Type"].ToString() : "";
                    if (!string.IsNullOrEmpty(disk2Type) && ddlDisk2Type.Items.FindByValue(disk2Type) != null)
                        ddlDisk2Type.SelectedValue = disk2Type;

                    if (dr["PurchaseDate"] != DBNull.Value)
                        txtPurchaseDate.Text = Convert.ToDateTime(dr["PurchaseDate"]).ToString("yyyy-MM-dd");
                    if (dr["WarrantyEnd"] != DBNull.Value)
                        txtWarrantyEnd.Text = Convert.ToDateTime(dr["WarrantyEnd"]).ToString("yyyy-MM-dd");
                    if (dr["ScrapDescription"] != DBNull.Value)
                        txtScrapDescription.Text = dr["ScrapDescription"].ToString();

                    if (dr["ScrapDate"] != DBNull.Value)
                        txtScrapDate.Text = Convert.ToDateTime(dr["ScrapDate"]).ToString("yyyy-MM-dd");

                    txtPurchaseNumber.Text = dr["PurchaseNumber"].ToString();
                    ddlStatus.SelectedValue = dr["Status"].ToString();
                    txtAccountingCode.Text = dr["AccountingCode"].ToString();
                    if (dt.Columns.Contains("ItemName"))
                        txtItemName.Text = dr["ItemName"] == DBNull.Value ? "" : dr["ItemName"].ToString();
                    else
                        txtItemName.Text = "";
                }
            }
            catch (Exception ex)
            {
                lblErrorMessage.Text = "Data fetch error: " + ex.Message;
                pnlError.Visible = true;
            }

            string relSql = "SELECT AD.AD_ID, (AD.DeviceType + ' - ' + AD.Description + ' [SN: ' + ISNULL(AD.AccountingCode, 'N/A') + ']') as Info " +
                            "FROM DeviceRel DR INNER JOIN AdditionalDevices AD ON DR.SecondaryDevice = AD.AD_ID " +
                            "WHERE DR.PrimaryDevice = " + id + " AND DR.Status = 1 AND AD.IsDelete IS NULL";
            DataTable dtRel = db.SqlToDt(relSql);
            List<SelectedAddDevice> loaded = new List<SelectedAddDevice>();
            foreach (DataRow row in dtRel.Rows)
                loaded.Add(new SelectedAddDevice { AD_ID = row["AD_ID"].ToString(), DeviceInfo = row["Info"].ToString() });
            CurrentSelectedAddDevices = loaded;
            BindLinkedDevices();
        }
        private void BindLinkedDevices()
        {
            rptLinkedAddDevices.DataSource = CurrentSelectedAddDevices;
            rptLinkedAddDevices.DataBind();
        }

        protected void btnAddAddLink_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlAdditionalDevices.SelectedValue)) return;
            var list = CurrentSelectedAddDevices;
            if (!list.Any(x => x.AD_ID == ddlAdditionalDevices.SelectedValue))
            {
                list.Add(new SelectedAddDevice { AD_ID = ddlAdditionalDevices.SelectedValue, DeviceInfo = ddlAdditionalDevices.SelectedItem.Text });
                CurrentSelectedAddDevices = list;
                BindLinkedDevices();
            }
        }

        protected void rptLinkedAddDevices_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Remove")
            {
                var list = CurrentSelectedAddDevices;
                list.RemoveAll(x => x.AD_ID == e.CommandArgument.ToString());
                CurrentSelectedAddDevices = list;
                BindLinkedDevices();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtBrand.Text) || string.IsNullOrEmpty(ddlDeviceType.SelectedValue))
            {
                lblErrorMessage.Text = "Please fill in all required (*) fields!";
                pnlError.Visible = true;
                return;
            }

            try
            {
                string id = Request.QueryString["id"];
                string sql = "";
                string typeID = ddlDeviceType.SelectedValue;
                string currentUser = cookie.Oku("User") ?? "Admin";
                string sDate = string.IsNullOrEmpty(txtScrapDate.Text) ? "NULL" : "'" + txtScrapDate.Text + "'";
                string sDesc = string.IsNullOrEmpty(txtScrapDescription.Text) ? "NULL" : "'" + txtScrapDescription.Text.Trim().Replace("'", "''") + "'";
                string isDeletedValue = string.IsNullOrEmpty(txtScrapDate.Text) ? "NULL" : "'X'";

   
                if (string.IsNullOrEmpty(id))
                {
                    sql = $@"INSERT INTO device (Brand, Model, SerialNumber, DeviceTypeID, CPU, RAM, DiskCapacity, Disk1Type, Disk2Capacity, Disk2Type, PurchaseDate, WarrantyEnd, PurchaseNumber, Status, ScrapDate, ScrapDescription, isDeleted, RecordDate, RecordUser, AccountingCode, ItemName) 
             VALUES ('{txtBrand.Text.Trim().Replace("'", "''")}', '{txtModel.Text.Trim().Replace("'", "''")}', '{txtSerial.Text.Trim().Replace("'", "''")}', {typeID}, 
             {(string.IsNullOrEmpty(txtCPU.Text) ? "NULL" : "'" + txtCPU.Text.Trim() + "'")}, {(string.IsNullOrEmpty(txtRAM.Text) ? "NULL" : "'" + txtRAM.Text.Trim() + "'")}, 
             {(string.IsNullOrEmpty(txtDiskCapacity.Text) ? "NULL" : txtDiskCapacity.Text.Trim())}, {(string.IsNullOrEmpty(ddlDiskType.SelectedValue) ? "NULL" : ddlDiskType.SelectedValue)}, 
             {(string.IsNullOrEmpty(txtDisk2Capacity.Text) ? "NULL" : txtDisk2Capacity.Text.Trim())}, {(string.IsNullOrEmpty(ddlDisk2Type.SelectedValue) ? "NULL" : ddlDisk2Type.SelectedValue)}, 
             {(string.IsNullOrEmpty(txtPurchaseDate.Text) ? "NULL" : "'" + txtPurchaseDate.Text + "'")}, {(string.IsNullOrEmpty(txtWarrantyEnd.Text) ? "NULL" : "'" + txtWarrantyEnd.Text + "'")}, 
             {(string.IsNullOrEmpty(txtPurchaseNumber.Text) ? "NULL" : "'" + txtPurchaseNumber.Text.Trim() + "'")}, {ddlStatus.SelectedValue}, {sDate}, {sDesc}, {isDeletedValue}, GETDATE(), '{currentUser}', 
             {(string.IsNullOrEmpty(txtAccountingCode.Text) ? "NULL" : "'" + txtAccountingCode.Text.Trim().Replace("'", "''") + "'")}, {(string.IsNullOrEmpty(txtItemName.Text) ? "NULL" : "'" + txtItemName.Text.Trim().Replace("'", "''") + "'")})";
                }
                else
                {
                    sql = $@"UPDATE device SET Brand = '{txtBrand.Text.Trim().Replace("'", "''")}', Model = '{txtModel.Text.Trim().Replace("'", "''")}', SerialNumber = '{txtSerial.Text.Trim().Replace("'", "''")}', DeviceTypeID = {typeID}, Status = {ddlStatus.SelectedValue}, 
             ScrapDate = {sDate}, ScrapDescription = {sDesc}, isDeleted = {isDeletedValue}, UpdateDate = GETDATE(), UpdateUser = '{currentUser}', 
             AccountingCode = {(string.IsNullOrEmpty(txtAccountingCode.Text) ? "NULL" : "'" + txtAccountingCode.Text.Trim().Replace("'", "''") + "'")}, 
             ItemName = {(string.IsNullOrEmpty(txtItemName.Text) ? "NULL" : "'" + txtItemName.Text.Trim().Replace("'", "''") + "'")} WHERE DeviceID = {id}";
                }

                if (db.ExecStr(sql) > 0)
                {
                    string currentDeviceId = id ?? db.SqlToDt("SELECT IDENT_CURRENT('device')").Rows[0][0].ToString();

                    DataTable dtActive = db.SqlToDt($"SELECT SecondaryDevice FROM DeviceRel WHERE PrimaryDevice = {currentDeviceId} AND Status = 1");
                    var activeInDb = dtActive.AsEnumerable().Select(r => r["SecondaryDevice"].ToString()).ToList();
                    var selectedInUi = CurrentSelectedAddDevices.Select(x => x.AD_ID).ToList();

                    foreach (var dbId in activeInDb)
                    {
                        if (!selectedInUi.Contains(dbId))
                        {
                            db.ExecStr($"UPDATE DeviceRel SET Status = 0, EndDate = GETDATE() WHERE PrimaryDevice = {currentDeviceId} AND SecondaryDevice = {dbId} AND Status = 1");
                        }
                    }


                    foreach (var uiId in selectedInUi)
                    {
                        if (!activeInDb.Contains(uiId))
                        {

                            DataTable dtHistory = db.SqlToDt($"SELECT RelID FROM DeviceRel WHERE PrimaryDevice = {currentDeviceId} AND SecondaryDevice = {uiId} AND Status = 0");

                            if (dtHistory.Rows.Count > 0)
                            {
                              
                                db.ExecStr($"UPDATE DeviceRel SET Status = 1, EndDate = NULL, UpdateDate = GETDATE(), UpdateUser = '{currentUser}' WHERE RelID = {dtHistory.Rows[0]["RelID"]}");
                            }
                            else
                            {

                                db.ExecStr($@"INSERT INTO DeviceRel (PrimaryDevice, SecondaryDevice, Status, StartDate, RecordDate, RecordUser) 
                             VALUES ({currentDeviceId}, {uiId}, 1, GETDATE(), GETDATE(), '{currentUser}')");
                            }
                        }
                    }
                    string userDevSql = $"SELECT UserDeviceID FROM UserDevice WHERE DeviceID = {currentDeviceId} AND isDeleted IS NULL";
                    DataTable dtUserDevices = db.SqlToDt(userDevSql);

                    foreach (DataRow row in dtUserDevices.Rows)
                    {
                        if (!CurrentAssignments.Any(x => x.UserDeviceID == row["UserDeviceID"].ToString()))
                        {
                            db.ExecStr($"UPDATE UserDevice SET isDeleted = 'X', DeleteDate = GETDATE(), DeleteUser = '{currentUser}' WHERE UserDeviceID = {row["UserDeviceID"]}");
                        }
                    }

                    foreach (var item in CurrentAssignments)
                    {
                        string start = "'" + item.StartDate.ToString("yyyy-MM-dd") + "'";
                        string end = item.EndDate.HasValue ? "'" + item.EndDate.Value.ToString("yyyy-MM-dd") + "'" : "NULL";

                        if (item.UserDeviceID.StartsWith("TEMP_"))
                        {
                            db.ExecStr($@"INSERT INTO UserDevice (UserID, DeviceID, StartDate, EndDate, RecordDate, RecordUser) 
                         VALUES ({item.UserID}, {currentDeviceId}, {start}, {end}, GETDATE(), '{currentUser}')");
                        }
                        else
                        {
                            db.ExecStr($@"UPDATE UserDevice SET UserID = {item.UserID}, StartDate = {start}, EndDate = {end}, UpdateDate = GETDATE(), UpdateUser = '{currentUser}' 
                         WHERE UserDeviceID = {item.UserDeviceID}");
                        }
                    }

                    ShowAlert("success", "Successful", "Record saved.", "Device.aspx");
                }
                else
                {
                    ShowAlert("error", "Error", "Database error: " + db.Hata);
                }
            }
            catch (Exception ex)
            {
                lblErrorMessage.Text = "System Error: " + ex.Message;
                pnlError.Visible = true;
            }
        }
        void ShowAlert(string type, string title, string message, string redirectUrl = "")
        {
            hfAlertType.Value = type;
            hfAlertTitle.Value = title;
            hfAlertMessage.Value = message;
            hfRedirectUrl.Value = redirectUrl;
        }
    }
}