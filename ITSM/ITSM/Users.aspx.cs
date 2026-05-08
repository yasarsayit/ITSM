using System;
using System.Data;
using System.Web.UI.WebControls;

namespace ITSM
{
    public partial class Users : System.Web.UI.Page
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

        public string FilterEmployeeID { get { return ViewState["F_EmployeeID"] as string ?? ""; } set { ViewState["F_EmployeeID"] = value; } }
        public string FilterName { get { return ViewState["F_Name"] as string ?? ""; } set { ViewState["F_Name"] = value; } }
        public string FilterUser { get { return ViewState["F_User"] as string ?? ""; } set { ViewState["F_User"] = value; } }
        public string FilterEmail { get { return ViewState["F_Email"] as string ?? ""; } set { ViewState["F_Email"] = value; } }
        public string FilterDept { get { return ViewState["F_Dept"] as string ?? ""; } set { ViewState["F_Dept"] = value; } }
        public string FilterTitle { get { return ViewState["F_Title"] as string ?? ""; } set { ViewState["F_Title"] = value; } }
        public string FilterStatus { get { return ViewState["F_Status"] as string ?? ""; } set { ViewState["F_Status"] = value; } }
        public string FilterType { get { return ViewState["F_Type"] as string ?? ""; } set { ViewState["F_Type"] = value; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CurrentPage = 1;
                ResetFilters();
                LoadDepartments();
                LoadUserTypes();
                FetchData();
            }
        }

        private void LoadDepartments()
        {
            try
            {
                string sql = "SELECT DISTINCT Department FROM Users WHERE Department IS NOT NULL AND Department <> '' ORDER BY Department ASC";
                db.Sql2AddCombo(ddlFilterDept, sql, "", "Department", "Department", true, "All");
            }
            catch (Exception) { }
        }

        private void LoadUserTypes()
        {
            try
            {
                string sql = "SELECT DISTINCT UserType FROM Users WHERE UserType IS NOT NULL AND UserType <> '' ORDER BY UserType ASC";
                db.Sql2AddCombo(ddlFilterType, sql, "", "UserType", "UserType", true, "All");
            }
            catch (Exception) { }
        }

        private void ResetFilters()
        {
            FilterEmployeeID = ""; FilterName = ""; FilterUser = ""; FilterEmail = "";
            FilterDept = ""; FilterTitle = ""; FilterStatus = ""; FilterType = "";
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

              
                if (!string.IsNullOrEmpty(FilterEmployeeID)) filterCondition += " AND EmployeeID LIKE '%" + chk.temizle(FilterEmployeeID) + "%' ";
                if (!string.IsNullOrEmpty(FilterName)) filterCondition += " AND NameSurname LIKE '%" + chk.temizle(FilterName) + "%' ";
                if (!string.IsNullOrEmpty(FilterUser)) filterCondition += " AND UserName LIKE '%" + chk.temizle(FilterUser) + "%' ";
                if (!string.IsNullOrEmpty(FilterEmail)) filterCondition += " AND Email LIKE '%" + chk.temizle(FilterEmail) + "%' ";
                if (!string.IsNullOrEmpty(FilterTitle)) filterCondition += " AND Title LIKE '%" + chk.temizle(FilterTitle) + "%' ";
                if (!string.IsNullOrEmpty(FilterStatus)) filterCondition += " AND Status = " + chk.temizle(FilterStatus) + " ";
                if (!string.IsNullOrEmpty(FilterDept)) filterCondition += " AND Department = '" + chk.temizle(FilterDept) + "' ";
                if (!string.IsNullOrEmpty(FilterType)) filterCondition += " AND UserType = '" + chk.temizle(FilterType) + "' ";

                string countSql = "SELECT COUNT(UserID) FROM Users WHERE (isDeleted IS NULL OR isDeleted != 'x')" + filterCondition;
                DataTable dtCount = db.SqlToDt(countSql);
                int totalRecords = dtCount.Rows.Count > 0 ? Convert.ToInt32(dtCount.Rows[0][0]) : 0;

                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
                if (TotalPages == 0) TotalPages = 1;

                if (CurrentPage > TotalPages) CurrentPage = TotalPages;
                offset = (CurrentPage - 1) * pageSize;

                string sql = "SELECT " + Environment.NewLine;
                sql += " UserID, " + Environment.NewLine;
                sql += " EmployeeID, " + Environment.NewLine;
                sql += " NameSurname, " + Environment.NewLine;
                sql += " UserName, " + Environment.NewLine;
                sql += " Email, " + Environment.NewLine;
                sql += " Department, " + Environment.NewLine;
                sql += " Title, " + Environment.NewLine;
                sql += " CASE Status WHEN 1 THEN 'Active' ELSE 'Inactive' END AS StatusText, " + Environment.NewLine;
                sql += " UserType " + Environment.NewLine;
                sql += " FROM Users " + Environment.NewLine;
                sql += " WHERE (isDeleted IS NULL OR isDeleted != 'x') " + filterCondition + Environment.NewLine;
                sql += " ORDER BY UserID DESC " + Environment.NewLine;
                sql += " OFFSET " + offset + " ROWS FETCH NEXT " + pageSize + " ROWS ONLY";

                DataTable dt = db.SqlToDt(sql);

                if (dt != null && dt.Rows.Count > 0)
                {
                    rptUsers.DataSource = dt;
                    rptUsers.DataBind();

                    rptUsers.Visible = true;
                    trNoData.Visible = false;
                }
                else
                {
                    rptUsers.Visible = false;
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
            FilterEmployeeID = txtFilterEmployeeID.Text.Trim();
            FilterName = txtFilterName.Text.Trim();
            FilterUser = txtFilterUser.Text.Trim();
            FilterEmail = txtFilterEmail.Text.Trim();
            FilterTitle = txtFilterTitle.Text.Trim();
            FilterStatus = ddlFilterStatus.SelectedValue;
            FilterDept = ddlFilterDept.SelectedValue;
            FilterType = ddlFilterType.SelectedValue;

            CurrentPage = 1;
            FetchData();
        }

        protected void lbAraTemizle_Click(object sender, EventArgs e)
        {
            txtFilterEmployeeID.Text = "";
            txtFilterName.Text = "";
            txtFilterUser.Text = "";
            txtFilterEmail.Text = "";
            txtFilterTitle.Text = "";
            ddlFilterStatus.SelectedIndex = 0;
            ddlFilterDept.SelectedIndex = 0;
            ddlFilterType.SelectedIndex = 0;

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

                string cleanId = chk.temizle(id);

            
                string checkSql = "SELECT count(*) as say FROM UserDevice WHERE UserID = " + cleanId + " AND (isDeleted IS NULL OR isDeleted != '1')";
                DataTable dtCheck = db.SqlToDt(checkSql);

                if (dtCheck != null && dtCheck.Rows.Count > 0)
                {
                    int deviceCount = Convert.ToInt32(dtCheck.Rows[0]["say"]);

                    if (deviceCount > 0)
                    {

                        ShowAlert("warning", "first remove the device assignments.");
                        return;
                    }
                }


                string sql = "UPDATE Users SET isDeleted = 'X' WHERE UserID = " + cleanId;
                int result = db.ExecStr(sql);

                if (result > 0)
                {
                    ShowAlert("success", "The user has been successfully deleted.");
                    FetchData();
                }
                else
                {
                    string errorDetail = !string.IsNullOrEmpty(db.Hata) ? db.Hata : "Kayıt bulunamadı.";
                    ShowAlert("error", "Veritabanı Hatası: " + chk.temizle(errorDetail));
                }
            }
            catch (Exception ex)
            {
                ShowAlert("error", "System Error: " + chk.temizle(ex.Message));
            }
            finally
            {
                hfDeleteId.Value = "";
            }
        }

        protected void rptUsers_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "Edit":
                    int editId = Convert.ToInt32(e.CommandArgument);
                    Response.Redirect("UserForm.aspx?id=" + editId);
                    break;

                case "ViewDevices":
                    int userIdForDevices = Convert.ToInt32(e.CommandArgument);
                    Response.Redirect("UserToDevice.aspx?userid=" + userIdForDevices);
                    break;

                default:
                    break;
            }
        }
    }
}