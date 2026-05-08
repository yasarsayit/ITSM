using System;
using System.Data;
using System.Web.UI.WebControls;

namespace ITSM
{
    public partial class Software : System.Web.UI.Page
    {
        DBTools db = new DBTools();
        CookieTools cookie = new CookieTools();
        CheckTools chk = new CheckTools(); 

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

        public string FilterDesc { get { return ViewState["F_Desc"] as string ?? ""; } set { ViewState["F_Desc"] = value; } }
        public string FilterLicense { get { return ViewState["F_License"] as string ?? ""; } set { ViewState["F_License"] = value; } }
        public string FilterType { get { return ViewState["F_Type"] as string ?? ""; } set { ViewState["F_Type"] = value; } }
        public string FilterPurDate { get { return ViewState["F_PurDate"] as string ?? ""; } set { ViewState["F_PurDate"] = value; } }
        public string FilterExpDate { get { return ViewState["F_ExpDate"] as string ?? ""; } set { ViewState["F_ExpDate"] = value; } }
        public string FilterUser { get { return ViewState["F_User"] as string ?? ""; } set { ViewState["F_User"] = value; } }
        public string FilterDevice { get { return ViewState["F_Device"] as string ?? ""; } set { ViewState["F_Device"] = value; } }
        public string FilterAccCode { get { return ViewState["F_AccCode"] as string ?? ""; } set { ViewState["F_AccCode"] = value; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CurrentPage = 1;
                ResetFilters();
                LoadSoftwareTypes(); 
                FetchData();
            }
        }

        private void LoadSoftwareTypes()
        {
            try
            {
                
                string sql = "SELECT DISTINCT SoftwareType FROM Software WHERE DeleteDate IS NULL AND SoftwareType IS NOT NULL AND SoftwareType <> '' ORDER BY SoftwareType ASC";
                db.Sql2AddCombo(ddlFilterType, sql, "", "SoftwareType", "SoftwareType", true, "All");
            }
            catch (Exception) { }
        }

        private void ResetFilters()
        {
            FilterDesc = ""; FilterLicense = ""; FilterType = "";
            FilterPurDate = ""; FilterExpDate = ""; FilterUser = "";
            FilterDevice = ""; FilterAccCode = "";
        }

        private void ShowAlert(string type, string message)
        {
            hfAlertType.Value = type;
            hfAlertMessage.Value = message;
        }

        private void FetchData()
        {
            try
            {
                int pageSize = Convert.ToInt32(ddlPageSize.SelectedValue);
                int offset = (CurrentPage - 1) * pageSize;

                string filterCondition = "";

   
                if (!string.IsNullOrEmpty(FilterDesc)) filterCondition += " AND Description LIKE '%" + chk.temizle(FilterDesc) + "%' ";
                if (!string.IsNullOrEmpty(FilterLicense)) filterCondition += " AND LicenseNumber LIKE '%" + chk.temizle(FilterLicense) + "%' ";
                if (!string.IsNullOrEmpty(FilterType)) filterCondition += " AND SoftwareType = '" + chk.temizle(FilterType) + "' ";

             
                if (!string.IsNullOrEmpty(FilterPurDate)) filterCondition += " AND CONVERT(date, PurchaseDate) = '" + chk.temizle(FilterPurDate) + "' ";
                if (!string.IsNullOrEmpty(FilterExpDate)) filterCondition += " AND CONVERT(date, ExpiryDate) = '" + chk.temizle(FilterExpDate) + "' ";

                if (!string.IsNullOrEmpty(FilterUser)) filterCondition += " AND AssignedUser LIKE '%" + chk.temizle(FilterUser) + "%' ";
                if (!string.IsNullOrEmpty(FilterDevice)) filterCondition += " AND DeviceNo LIKE '%" + chk.temizle(FilterDevice) + "%' ";
                if (!string.IsNullOrEmpty(FilterAccCode)) filterCondition += " AND AccountingCode LIKE '%" + chk.temizle(FilterAccCode) + "%' ";

                string baseWhere = " WHERE (isDelete IS NULL OR isDelete != 'X') " + filterCondition;

               
                string countSql = "SELECT COUNT(SoftwareID) FROM Software " + baseWhere;
                DataTable dtCount = db.SqlToDt(countSql);
                int totalRecords = dtCount.Rows.Count > 0 ? Convert.ToInt32(dtCount.Rows[0][0]) : 0;

                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
                if (TotalPages == 0) TotalPages = 1;

                if (CurrentPage > TotalPages) CurrentPage = TotalPages;
                offset = (CurrentPage - 1) * pageSize;

                string sql = "SELECT " + Environment.NewLine;
                sql += " SoftwareID, " + Environment.NewLine;
                sql += " Description, " + Environment.NewLine;
                sql += " LicenseNumber, " + Environment.NewLine;
                sql += " LicenseType, " + Environment.NewLine;
                sql += " SoftwareType, " + Environment.NewLine;
                sql += " PurchaseDate, " + Environment.NewLine;
                sql += " ExpiryDate, " + Environment.NewLine;
                sql += " AssignedUser, " + Environment.NewLine;
                sql += " DeviceNo, " + Environment.NewLine;
                sql += " AccountingCode, " + Environment.NewLine;
                sql += " RecordUser " + Environment.NewLine;
                sql += " FROM Software " + Environment.NewLine;
                sql += baseWhere + Environment.NewLine;
                sql += " ORDER BY SoftwareID DESC " + Environment.NewLine;
                sql += " OFFSET " + offset + " ROWS FETCH NEXT " + pageSize + " ROWS ONLY";

                DataTable dt = db.SqlToDt(sql);

                if (dt != null && dt.Rows.Count > 0)
                {
                    rptSoftware.DataSource = dt;
                    rptSoftware.DataBind();
                    rptSoftware.Visible = true;
                    trNoData.Visible = false;
                }
                else
                {
                    rptSoftware.Visible = false;
                    trNoData.Visible = true;
                }

                SetupPagination(totalRecords, pageSize);
            }
            catch (Exception ex)
            {
                ShowAlert("error", "Data load error: " + chk.temizle(ex.Message));
            }
        }

     
        protected void lbAra_Click(object sender, EventArgs e)
        {
            FilterDesc = txtFilterDesc.Text.Trim();
            FilterLicense = txtFilterLicense.Text.Trim();
            FilterType = ddlFilterType.SelectedValue;
            FilterPurDate = txtFilterPurDate.Text.Trim();
            FilterExpDate = txtFilterExpDate.Text.Trim();
            FilterUser = txtFilterUser.Text.Trim();
            FilterDevice = txtFilterDevice.Text.Trim();
            FilterAccCode = txtFilterAccCode.Text.Trim();

            CurrentPage = 1;
            FetchData();
        }

        protected void lbAraTemizle_Click(object sender, EventArgs e)
        {
            txtFilterDesc.Text = "";
            txtFilterLicense.Text = "";
            ddlFilterType.SelectedIndex = 0;
            txtFilterPurDate.Text = "";
            txtFilterExpDate.Text = "";
            txtFilterUser.Text = "";
            txtFilterDevice.Text = "";
            txtFilterAccCode.Text = "";

            ResetFilters();

            CurrentPage = 1;
            FetchData();
        }

   
        private void SetupPagination(int totalRecords, int pageSize)
        {
            int startRecord = ((CurrentPage - 1) * pageSize) + 1;
            int endRecord = Math.Min(CurrentPage * pageSize, totalRecords);

            if (totalRecords == 0)
            {
                lblPageInfo.Text = "Showing 0 entries";
            }
            else
            {
                lblPageInfo.Text = $"Showing {startRecord} to {endRecord} of {totalRecords} entries";
            }

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
            FetchData();
        }

        protected void lbPrev_Click(object sender, EventArgs e)
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                FetchData();
            }
        }

        protected void lbNext_Click(object sender, EventArgs e)
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                FetchData();
            }
        }

        protected void rptPagination_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Page")
            {
                CurrentPage = Convert.ToInt32(e.CommandArgument);
                FetchData();
            }
        }

        protected void btnDeleteConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                string id = hfDeleteId.Value;
                if (string.IsNullOrEmpty(id)) return;

                string user = cookie.Oku("User") ?? "System";

                string sql = "UPDATE Software SET " + Environment.NewLine;
                sql += " DeleteDate = GETDATE(), " + Environment.NewLine;
                sql += " DeleteUser = '" + chk.temizle(user) + "', " + Environment.NewLine;
                sql += " isDelete = 'X' " + Environment.NewLine;
                sql += " WHERE SoftwareID = " + chk.temizle(id);

                int result = db.ExecStr(sql);
                if (result > 0)
                {
                    ShowAlert("success", "Record marked as deleted successfully.");
                }
                else
                {
                    ShowAlert("error", "Database error: " + chk.temizle(db.Hata));
                }
            }
            catch (Exception ex)
            {
                ShowAlert("error", "System error: " + chk.temizle(ex.Message));
            }
            finally
            {
                hfDeleteId.Value = "";
                FetchData();
            }
        }

        protected void rptSoftware_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Edit")
            {
                Response.Redirect("SoftwareForm.aspx?id=" + e.CommandArgument);
            }
            else if (e.CommandName == "ShowDevices")
            {
                Response.Redirect("SoftwareToDevice.aspx?softwareId=" + e.CommandArgument);
            }
        }
    }
}