using System;
using System.Data;
using System.Web.UI.WebControls;

namespace ITSM
{
    public partial class DeviceSoftwareReport : System.Web.UI.Page
    {
        DBTools db = new DBTools();
        CheckTools chk = new CheckTools();
        public int CurrentPage { get { return ViewState["CP"] != null ? (int)ViewState["CP"] : 1; } set { ViewState["CP"] = value; } }
        public int TotalPages { get { return ViewState["TP"] != null ? (int)ViewState["TP"] : 1; } set { ViewState["TP"] = value; } }
        public string FilterDevice { get { return ViewState["FDevice"] as string ?? ""; } set { ViewState["FDevice"] = value; } }
        public string FilterSoftware { get { return ViewState["FSoftware"] as string ?? ""; } set { ViewState["FSoftware"] = value; } }
        public string FilterDate { get { return ViewState["FDate"] as string ?? ""; } set { ViewState["FDate"] = value; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetAssignedSoftware();
            }
        }

        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentPage = 1;
            GetAssignedSoftware();
        }

        private void GetAssignedSoftware()
        {
            int pageSize = Convert.ToInt32(ddlPageSize.SelectedValue);
            int offset = (CurrentPage - 1) * pageSize;

            string filterCond = "";
            if (!string.IsNullOrEmpty(FilterDevice)) filterCond += " AND (D.Model LIKE '%" + chk.temizle(FilterDevice) + "%' OR D.SerialNumber LIKE '%" + chk.temizle(FilterDevice) + "%') ";
            if (!string.IsNullOrEmpty(FilterSoftware)) filterCond += " AND S.Description LIKE '%" + chk.temizle(FilterSoftware) + "%' ";
            if (!string.IsNullOrEmpty(FilterDate)) filterCond += " AND CONVERT(varchar, DS.StartDate, 104) LIKE '%" + chk.temizle(FilterDate) + "%' ";

            string baseWhere = " WHERE (DS.EndDate IS NULL) " + filterCond;

            string countSql = "SELECT COUNT(*) FROM DeviceSoftware DS INNER JOIN Device D ON DS.DeviceID = D.DeviceID INNER JOIN Software S ON DS.SoftwareID = S.SoftwareID " + baseWhere;
            DataTable dtCount = db.SqlToDt(countSql);
            int totalRecords = (dtCount != null && dtCount.Rows.Count > 0 && dtCount.Rows[0][0] != DBNull.Value) ? Convert.ToInt32(dtCount.Rows[0][0]) : 0;

            TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            if (TotalPages == 0) TotalPages = 1;
            if (CurrentPage > TotalPages) CurrentPage = TotalPages;
            offset = (CurrentPage - 1) * pageSize;

            string sql = "SELECT D.Model, D.SerialNumber, S.Description AS SoftwareDesc, DS.StartDate ";
            sql += " FROM DeviceSoftware DS ";
            sql += " INNER JOIN Device D ON DS.DeviceID = D.DeviceID ";
            sql += " INNER JOIN Software S ON DS.SoftwareID = S.SoftwareID ";
            sql += baseWhere;
            sql += " ORDER BY DS.StartDate DESC ";
            sql += " OFFSET " + offset + " ROWS FETCH NEXT " + pageSize + " ROWS ONLY";

            DataTable dt = db.SqlToDt(sql);
            rptAssignedSoftware.DataSource = dt;
            rptAssignedSoftware.DataBind();

            if (rptAssignedSoftware.Items.Count > 0)
            {
                RepeaterItem headerItem = rptAssignedSoftware.Controls[0] as RepeaterItem;
                if (headerItem != null)
                {
                    TextBox txtFDevice = headerItem.FindControl("txtFDevice") as TextBox;
                    TextBox txtFSoftware = headerItem.FindControl("txtFSoftware") as TextBox;
                    TextBox txtFDate = headerItem.FindControl("txtFDate") as TextBox;

                    if (txtFDevice != null) txtFDevice.Text = FilterDevice;
                    if (txtFSoftware != null) txtFSoftware.Text = FilterSoftware;
                    if (txtFDate != null) txtFDate.Text = FilterDate;
                }
            }

            SetupPagination(totalRecords, pageSize, dt.Rows.Count);
        }

        protected void lbAra_Click(object sender, EventArgs e)
        {
            RepeaterItem headerItem = rptAssignedSoftware.Controls[0] as RepeaterItem;
            if (headerItem != null)
            {
                FilterDevice = (headerItem.FindControl("txtFDevice") as TextBox)?.Text;
                FilterSoftware = (headerItem.FindControl("txtFSoftware") as TextBox)?.Text;
                FilterDate = (headerItem.FindControl("txtFDate") as TextBox)?.Text;
            }
            CurrentPage = 1;
            GetAssignedSoftware();
        }

        protected void lbAraTemizle_Click(object sender, EventArgs e)
        {
            FilterDevice = "";
            FilterSoftware = "";
            FilterDate = "";
            CurrentPage = 1;
            GetAssignedSoftware();
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
                GetAssignedSoftware();
            }
        }

        protected void lbNext_Click(object sender, EventArgs e)
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                GetAssignedSoftware();
            }
        }

        protected void rptPagination_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Page")
            {
                CurrentPage = Convert.ToInt32(e.CommandArgument);
                GetAssignedSoftware();
            }
        }
    }
}