using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ITSM
{
    public partial class Device : System.Web.UI.Page
    {
        DBTools db = new DBTools();
        CookieTools cookie = new CookieTools();
        CheckTools chk = new CheckTools();

        public int CurrentPage1 { get { return ViewState["CP1"] != null ? (int)ViewState["CP1"] : 1; } set { ViewState["CP1"] = value; } }
        public int TotalPages1 { get { return ViewState["TP1"] != null ? (int)ViewState["TP1"] : 1; } set { ViewState["TP1"] = value; } }
        public string F1Brand { get { return ViewState["F1B"] as string ?? ""; } set { ViewState["F1B"] = value; } }
        public string F1Model { get { return ViewState["F1M"] as string ?? ""; } set { ViewState["F1M"] = value; } }
        public string F1Serial { get { return ViewState["F1S"] as string ?? ""; } set { ViewState["F1S"] = value; } }
        public string F1Type { get { return ViewState["F1T"] as string ?? ""; } set { ViewState["F1T"] = value; } }
        public string F1ItemName { get { return ViewState["F1IN"] as string ?? ""; } set { ViewState["F1IN"] = value; } }





        public int CurrentPage3 { get { return ViewState["CP3"] != null ? (int)ViewState["CP3"] : 1; } set { ViewState["CP3"] = value; } }
        public int TotalPages3 { get { return ViewState["TP3"] != null ? (int)ViewState["TP3"] : 1; } set { ViewState["TP3"] = value; } }
        public string F3User { get { return ViewState["F3U"] as string ?? ""; } set { ViewState["F3U"] = value; } }
        public string F3Device { get { return ViewState["F3D"] as string ?? ""; } set { ViewState["F3D"] = value; } }
        public string F3Date { get { return ViewState["F3Dt"] as string ?? ""; } set { ViewState["F3Dt"] = value; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                VerileriGetir();

                GetAssignedDevices();
            }
        }


        private void VerileriGetir()
        {
            int pageSize = Convert.ToInt32(ddlPageSize1.SelectedValue);
            int offset = (CurrentPage1 - 1) * pageSize;

            string filterCond = "";
            if (!string.IsNullOrEmpty(F1Brand)) filterCond += " AND D.Brand LIKE '%" + chk.temizle(F1Brand) + "%' ";
            if (!string.IsNullOrEmpty(F1Model)) filterCond += " AND D.Model LIKE '%" + chk.temizle(F1Model) + "%' ";
            if (!string.IsNullOrEmpty(F1Serial)) filterCond += " AND D.SerialNumber LIKE '%" + chk.temizle(F1Serial) + "%' ";
            if (!string.IsNullOrEmpty(F1Type)) filterCond += " AND (CASE D.DeviceTypeID WHEN 1 THEN 'Laptop PC' ELSE 'Desktop PC' END) LIKE '%" + chk.temizle(F1Type) + "%' ";
            if (!string.IsNullOrEmpty(F1ItemName)) filterCond += " AND ISNULL(D.ItemName, '') LIKE '%" + chk.temizle(F1ItemName) + "%' ";

            string baseWhere = " WHERE D.IsDeleted IS NULL " + filterCond;

            string countSql = "SELECT COUNT(D.DeviceID) FROM device D " + baseWhere;
            DataTable dtCount = db.SqlToDt(countSql);
            int totalRecords = (dtCount != null && dtCount.Rows.Count > 0 && dtCount.Rows[0][0] != DBNull.Value) ? Convert.ToInt32(dtCount.Rows[0][0]) : 0;

            TotalPages1 = (int)Math.Ceiling((double)totalRecords / pageSize);
            if (TotalPages1 == 0) TotalPages1 = 1;
            if (CurrentPage1 > TotalPages1) CurrentPage1 = TotalPages1;
            offset = (CurrentPage1 - 1) * pageSize;


            string sql = "SELECT " + Environment.NewLine;
            sql += " D.DeviceID, " + Environment.NewLine;
            sql += " D.Brand, " + Environment.NewLine;
            sql += " D.Model, " + Environment.NewLine;
            sql += " D.SerialNumber, " + Environment.NewLine;
            sql += " (SELECT COUNT(*) FROM DeviceRel DR WHERE DR.PrimaryDevice = D.DeviceID AND DR.Status = 1) as AdditionalCount, " + Environment.NewLine;
            sql += " ISNULL(STUFF((SELECT ', ' + (AD.DeviceType + ' (' + AD.Description + ')') " + Environment.NewLine;
            sql += "               FROM DeviceRel DR " + Environment.NewLine;
            sql += "               INNER JOIN AdditionalDevices AD ON DR.SecondaryDevice = AD.AD_ID " + Environment.NewLine;
            sql += "               WHERE DR.PrimaryDevice = D.DeviceID AND DR.Status = 1 AND AD.IsDelete IS NULL " + Environment.NewLine;
            sql += "               FOR XML PATH('')), 1, 2, ''), 'No Devices') as LinkedAdditionalDevices, " + Environment.NewLine;
            sql += " CASE D.DeviceTypeID WHEN 1 THEN 'Laptop PC' ELSE 'Desktop PC' END AS DeviceTypeText, " + Environment.NewLine;
            sql += " ISNULL(D.ItemName, '') AS ItemName " + Environment.NewLine;
            sql += " FROM device D " + Environment.NewLine;
            sql += baseWhere + " " + Environment.NewLine;
            sql += " ORDER BY D.DeviceID DESC " + Environment.NewLine;
            sql += " OFFSET " + offset + " ROWS FETCH NEXT " + pageSize + " ROWS ONLY";

            DataTable dt = db.SqlToDt(sql);
            rptDevices.DataSource = dt;
            rptDevices.DataBind();
            SetupPagination1(totalRecords, pageSize, dt.Rows.Count);
        }

        protected void lbAra1_Click(object sender, EventArgs e)
        {
            RepeaterItem headerItem = rptDevices.Controls[0] as RepeaterItem;
            if (headerItem != null)
            {
                F1Brand = (headerItem.FindControl("txtF1Brand") as TextBox)?.Text.Trim();
                F1Model = (headerItem.FindControl("txtF1Model") as TextBox)?.Text.Trim();
                F1Serial = (headerItem.FindControl("txtF1Serial") as TextBox)?.Text.Trim();
                F1Type = (headerItem.FindControl("txtF1Type") as TextBox)?.Text.Trim();
                F1ItemName = (headerItem.FindControl("txtF1ItemName") as TextBox)?.Text.Trim();
            }
            CurrentPage1 = 1;
            VerileriGetir();
        }

        protected void lbAraTemizle1_Click(object sender, EventArgs e)
        {
            F1Brand = ""; F1Model = ""; F1Serial = ""; F1Type = ""; F1ItemName = "";
            CurrentPage1 = 1;
            VerileriGetir();
        }
        protected void ddlPageSize1_SelectedIndexChanged(object sender, EventArgs e) { CurrentPage1 = 1; VerileriGetir(); }
        protected void lbPrev1_Click(object sender, EventArgs e) { if (CurrentPage1 > 1) { CurrentPage1--; VerileriGetir(); } }
        protected void lbNext1_Click(object sender, EventArgs e) { if (CurrentPage1 < TotalPages1) { CurrentPage1++; VerileriGetir(); } }
        protected void rptPagination1_ItemCommand(object source, RepeaterCommandEventArgs e) { if (e.CommandName == "Page") { CurrentPage1 = Convert.ToInt32(e.CommandArgument); VerileriGetir(); } }

        private void SetupPagination1(int totalRecords, int pageSize, int rowCount)
        {
            int startRecord = ((CurrentPage1 - 1) * pageSize) + 1;
            int endRecord = startRecord + rowCount - 1;
            lblPageInfo1.Text = totalRecords == 0 ? "Showing 0 entries" : $"Showing {startRecord} to {endRecord} of {totalRecords} entries";

            DataTable dtPages = new DataTable();
            dtPages.Columns.Add("PageNumber");
            dtPages.Columns.Add("IsActive", typeof(bool));
            int startPage = Math.Max(1, CurrentPage1 - 3);
            int endPage = Math.Min(TotalPages1, CurrentPage1 + 3);
            for (int i = startPage; i <= endPage; i++) dtPages.Rows.Add(i, i == CurrentPage1);
            rptPagination1.DataSource = dtPages;
            rptPagination1.DataBind();
            liPrev1.Attributes["class"] = CurrentPage1 > 1 ? "page-item" : "page-item disabled";
            liNext1.Attributes["class"] = CurrentPage1 < TotalPages1 ? "page-item" : "page-item disabled";
        }







        private void GetAssignedDevices()
        {
            int pageSize = Convert.ToInt32(ddlPageSize3.SelectedValue);
            int offset = (CurrentPage3 - 1) * pageSize;

            string filterCond = "";
            if (!string.IsNullOrEmpty(F3User)) filterCond += " AND U.NameSurname LIKE '%" + chk.temizle(F3User) + "%' ";
            if (!string.IsNullOrEmpty(F3Device)) filterCond += " AND (D.Brand + ' ' + D.Model + ' (' + D.SerialNumber + ')') LIKE '%" + chk.temizle(F3Device) + "%' ";
            if (!string.IsNullOrEmpty(F3Date)) filterCond += " AND CONVERT(varchar, UD.StartDate, 104) LIKE '%" + chk.temizle(F3Date) + "%' ";

            string baseWhere = " WHERE UD.EndDate IS NULL AND (UD.isDeleted IS NULL OR LTRIM(RTRIM(UD.isDeleted)) = '') " + filterCond;


            string countSql = "SELECT COUNT(UD.UD_ID) FROM UserDevice UD INNER JOIN Users U ON UD.UserID = U.UserID INNER JOIN device D ON UD.DeviceID = D.DeviceID " + baseWhere;
            DataTable dtCount3 = db.SqlToDt(countSql);
            int totalRecords = (dtCount3 != null && dtCount3.Rows.Count > 0 && dtCount3.Rows[0][0] != DBNull.Value) ? Convert.ToInt32(dtCount3.Rows[0][0]) : 0;

            TotalPages3 = (int)Math.Ceiling((double)totalRecords / pageSize);
            if (TotalPages3 == 0) TotalPages3 = 1;
            if (CurrentPage3 > TotalPages3) CurrentPage3 = TotalPages3;
            offset = (CurrentPage3 - 1) * pageSize;


            string sql = "SELECT " + Environment.NewLine;
            sql += " U.NameSurname, " + Environment.NewLine;
            sql += " (D.Brand + ' ' + D.Model + ' (' + D.SerialNumber + ')') AS DeviceFullInfo, " + Environment.NewLine;
            sql += " UD.StartDate " + Environment.NewLine;
            sql += " FROM UserDevice UD " + Environment.NewLine;
            sql += " INNER JOIN Users U ON UD.UserID = U.UserID " + Environment.NewLine;
            sql += " INNER JOIN device D ON UD.DeviceID = D.DeviceID " + Environment.NewLine;
            sql += baseWhere + " " + Environment.NewLine;
            sql += " ORDER BY UD.StartDate DESC " + Environment.NewLine;
            sql += " OFFSET " + offset + " ROWS FETCH NEXT " + pageSize + " ROWS ONLY";

            DataTable dt = db.SqlToDt(sql);
            rptAssignedDevices.DataSource = dt;
            rptAssignedDevices.DataBind();
            SetupPagination3(totalRecords, pageSize, dt.Rows.Count);
        }

        protected void lbAra3_Click(object sender, EventArgs e)
        {
            RepeaterItem headerItem = rptAssignedDevices.Controls[0] as RepeaterItem;
            if (headerItem != null)
            {
                F3User = (headerItem.FindControl("txtF3User") as TextBox)?.Text.Trim();
                F3Device = (headerItem.FindControl("txtF3Device") as TextBox)?.Text.Trim();
                F3Date = (headerItem.FindControl("txtF3Date") as TextBox)?.Text.Trim();
            }
            CurrentPage3 = 1;
            GetAssignedDevices();
        }

        protected void lbAraTemizle3_Click(object sender, EventArgs e)
        {
            F3User = ""; F3Device = ""; F3Date = "";
            CurrentPage3 = 1;
            GetAssignedDevices();
        }
        protected void ddlPageSize3_SelectedIndexChanged(object sender, EventArgs e) { CurrentPage3 = 1; GetAssignedDevices(); }
        protected void lbPrev3_Click(object sender, EventArgs e) { if (CurrentPage3 > 1) { CurrentPage3--; GetAssignedDevices(); } }
        protected void lbNext3_Click(object sender, EventArgs e) { if (CurrentPage3 < TotalPages3) { CurrentPage3++; GetAssignedDevices(); } }
        protected void rptPagination3_ItemCommand(object source, RepeaterCommandEventArgs e) { if (e.CommandName == "Page") { CurrentPage3 = Convert.ToInt32(e.CommandArgument); GetAssignedDevices(); } }

        private void SetupPagination3(int totalRecords, int pageSize, int rowCount)
        {
            int startRecord = ((CurrentPage3 - 1) * pageSize) + 1;
            int endRecord = startRecord + rowCount - 1;
            lblPageInfo3.Text = totalRecords == 0 ? "Showing 0 entries" : $"Showing {startRecord} to {endRecord} of {totalRecords} entries";

            DataTable dtPages = new DataTable();
            dtPages.Columns.Add("PageNumber");
            dtPages.Columns.Add("IsActive", typeof(bool));
            int startPage = Math.Max(1, CurrentPage3 - 3);
            int endPage = Math.Min(TotalPages3, CurrentPage3 + 3);
            for (int i = startPage; i <= endPage; i++) dtPages.Rows.Add(i, i == CurrentPage3);
            rptPagination3.DataSource = dtPages;
            rptPagination3.DataBind();
            liPrev3.Attributes["class"] = CurrentPage3 > 1 ? "page-item" : "page-item disabled";
            liNext3.Attributes["class"] = CurrentPage3 < TotalPages3 ? "page-item" : "page-item disabled";
        }



        protected void rptDevices_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Edit") Response.Redirect("DeviceForm.aspx?id=" + e.CommandArgument);
        }

        protected void rptAdditionalDevices_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Edit") Response.Redirect("AddDeviceForm.aspx?id=" + e.CommandArgument);
        }

        protected void btnDeleteConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                string deviceId = hfDeleteId.Value;


                string checkSql = "SELECT COUNT(*) FROM DeviceRel WHERE PrimaryDevice = " + deviceId + " AND Status = 1";
                int linkedCount = Convert.ToInt32(db.SqlToDt(checkSql).Rows[0][0]);

                if (linkedCount > 0)
                {
                    ShowAlert("error", "Cannot Delete!", $"This device has active additional device(s) linked to it. Please disconnect them first.");
                    return;
                }

                string checkUserSql = "SELECT COUNT(*) FROM UserDevice WHERE DeviceID = " + deviceId + " AND IsDeleted IS NULL AND EndDate IS NULL";
                int userCount = Convert.ToInt32(db.SqlToDt(checkUserSql).Rows[0][0]);

                if (userCount > 0)
                {
                    ShowAlert("error", "Cannot Delete!", "This device is currently assigned to a personnel. Please end the assignment first.");
                    return;
                }

                string currentUser = cookie.Oku("User") ?? "Admin";
                string sql = $"UPDATE device SET IsDeleted = 'X', DeleteDate = GETDATE(), DeleteUser = '{currentUser}' WHERE DeviceID = {deviceId}";

                if (db.ExecStr(sql) > 0)
                {
                    ShowAlert("success", "Success", "Device deleted successfully.");
                    VerileriGetir();
                }
                else
                {
                    ShowAlert("error", "Error", "Could not delete the device.");
                }
            }
            catch (Exception ex)
            {
                ShowAlert("error", "System Error", ex.Message);
            }
        }



        private void ShowAlert(string type, string title, string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "showError", $"Swal.fire({{ icon: '{type}', title: '{title}', text: '{message}' }});", true);
        }
    }
}