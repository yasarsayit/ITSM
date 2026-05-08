using System;
using System.Data;
using System.Web.UI.WebControls;

namespace ITSM
{
    public partial class UserDeviceReport : System.Web.UI.Page
    {
        DBTools db = new DBTools();
        CheckTools chk = new CheckTools();

        public int CurrentPage { get { return ViewState["CP"] != null ? (int)ViewState["CP"] : 1; } set { ViewState["CP"] = value; } }
        public int TotalPages { get { return ViewState["TP"] != null ? (int)ViewState["TP"] : 1; } set { ViewState["TP"] = value; } }
        public string FilterUser { get { return ViewState["FUser"] as string ?? ""; } set { ViewState["FUser"] = value; } }
        public string FilterDevice { get { return ViewState["FDevice"] as string ?? ""; } set { ViewState["FDevice"] = value; } }
        public string FilterDate { get { return ViewState["FDate"] as string ?? ""; } set { ViewState["FDate"] = value; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetAssignedDevices();
            }
        }

        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentPage = 1;
            GetAssignedDevices();
        }

        private void GetAssignedDevices()
        {
            int pageSize = Convert.ToInt32(ddlPageSize.SelectedValue);
            int offset = (CurrentPage - 1) * pageSize;

            string filterCond = "";
            if (!string.IsNullOrEmpty(FilterUser)) filterCond += " AND U.NameSurname LIKE '%" + chk.temizle(FilterUser) + "%' ";
            if (!string.IsNullOrEmpty(FilterDevice)) filterCond += " AND (D.Brand LIKE '%" + chk.temizle(FilterDevice) + "%' OR D.Model LIKE '%" + chk.temizle(FilterDevice) + "%' OR D.SerialNumber LIKE '%" + chk.temizle(FilterDevice) + "%') ";
            if (!string.IsNullOrEmpty(FilterDate)) filterCond += " AND CONVERT(varchar, UD.StartDate, 104) LIKE '%" + chk.temizle(FilterDate) + "%' ";

            string baseWhere = " WHERE UD.EndDate IS NULL AND (UD.isDeleted IS NULL OR LTRIM(RTRIM(UD.isDeleted)) = '') " + filterCond;

            string countSql = "SELECT COUNT(UD.UserDeviceID) FROM UserDevice UD INNER JOIN Users U ON UD.UserID = U.UserID INNER JOIN device D ON UD.DeviceID = D.DeviceID " + baseWhere;
            DataTable dtCount = db.SqlToDt(countSql);
            int totalRecords = (dtCount != null && dtCount.Rows.Count > 0 && dtCount.Rows[0][0] != DBNull.Value) ? Convert.ToInt32(dtCount.Rows[0][0]) : 0;

            TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            if (TotalPages == 0) TotalPages = 1;
            if (CurrentPage > TotalPages) CurrentPage = TotalPages;
            offset = (CurrentPage - 1) * pageSize;

            string sql = "SELECT UD.UserDeviceID, U.NameSurname, D.Brand, D.Model, D.SerialNumber, UD.StartDate ";
            sql += " FROM UserDevice UD ";
            sql += " INNER JOIN Users U ON UD.UserID = U.UserID ";
            sql += " INNER JOIN device D ON UD.DeviceID = D.DeviceID ";
            sql += baseWhere;
            sql += " ORDER BY UD.StartDate DESC ";
            sql += " OFFSET " + offset + " ROWS FETCH NEXT " + pageSize + " ROWS ONLY";

            DataTable dt = db.SqlToDt(sql);
            rptAssignedDevices.DataSource = dt;
            rptAssignedDevices.DataBind();

            if (rptAssignedDevices.Items.Count > 0)
            {
                RepeaterItem headerItem = rptAssignedDevices.Controls[0] as RepeaterItem;
                if (headerItem != null)
                {
                    TextBox txtFUser = headerItem.FindControl("txtFUser") as TextBox;
                    TextBox txtFDevice = headerItem.FindControl("txtFDevice") as TextBox;
                    TextBox txtFDate = headerItem.FindControl("txtFDate") as TextBox;

                    if (txtFUser != null) txtFUser.Text = FilterUser;
                    if (txtFDevice != null) txtFDevice.Text = FilterDevice;
                    if (txtFDate != null) txtFDate.Text = FilterDate;
                }
            }

            SetupPagination(totalRecords, pageSize, dt.Rows.Count);
        }

        protected void lbAra_Click(object sender, EventArgs e)
        {
            RepeaterItem headerItem = rptAssignedDevices.Controls[0] as RepeaterItem;
            if (headerItem != null)
            {
                FilterUser = (headerItem.FindControl("txtFUser") as TextBox)?.Text;
                FilterDevice = (headerItem.FindControl("txtFDevice") as TextBox)?.Text;
                FilterDate = (headerItem.FindControl("txtFDate") as TextBox)?.Text;
            }
            CurrentPage = 1;
            GetAssignedDevices();
        }

        protected void lbAraTemizle_Click(object sender, EventArgs e)
        {
            FilterUser = "";
            FilterDevice = "";
            FilterDate = "";
            CurrentPage = 1;
            GetAssignedDevices();
        }

        private void SetupPagination(int totalRecords, int pageSize, int rowCount)
        {
            int startRecord = ((CurrentPage - 1) * pageSize) + 1;
            int endRecord = startRecord + rowCount - 1;
            lblPageInfo.Text = totalRecords == 0 ? "Showing 0 entries" : $"Showing {startRecord} to {endRecord} of {totalRecords} entries";

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

            liPrev.Attributes["class"] = CurrentPage == 1 ? "page-item disabled" : "page-item";
            liNext.Attributes["class"] = CurrentPage == TotalPages ? "page-item disabled" : "page-item";
        }

        protected void lbPrev_Click(object sender, EventArgs e)
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                GetAssignedDevices();
            }
        }

        protected void lbNext_Click(object sender, EventArgs e)
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                GetAssignedDevices();
            }
        }

        protected void rptPagination_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Page")
            {
                CurrentPage = Convert.ToInt32(e.CommandArgument);
                GetAssignedDevices();
            }
        }
    }
}