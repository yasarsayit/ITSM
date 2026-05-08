using System;
using System.Data;
using System.Web.UI.WebControls;

namespace ITSM
{
    public partial class AddDeviceDeviceReport : System.Web.UI.Page
    {
        DBTools db = new DBTools();


        public int CurrentPage { get { return ViewState["CP"] != null ? (int)ViewState["CP"] : 1; } set { ViewState["CP"] = value; } }
        public int TotalPages { get { return ViewState["TP"] != null ? (int)ViewState["TP"] : 1; } set { ViewState["TP"] = value; } }
        public string FilterPrimary { get { return ViewState["FPri"] as string ?? ""; } set { ViewState["FPri"] = value; } }
        public string FilterSecondary { get { return ViewState["FSec"] as string ?? ""; } set { ViewState["FSec"] = value; } }
        public string FilterDate { get { return ViewState["FDate"] as string ?? ""; } set { ViewState["FDate"] = value; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetRelations();
            }
        }

        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentPage = 1;
            GetRelations();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            CurrentPage = 1;
            FilterPrimary = txtSearchPrimary.Text.Trim();
            FilterSecondary = txtSearchSecondary.Text.Trim();
            FilterDate = txtSearchDate.Text.Trim();
            GetRelations();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            CurrentPage = 1;
            txtSearchPrimary.Text = "";
            txtSearchSecondary.Text = "";
            txtSearchDate.Text = "";
            FilterPrimary = "";
            FilterSecondary = "";
            FilterDate = "";
            GetRelations();
        }

        private void GetRelations()
        {
            try
            {
                int pageSize = Convert.ToInt32(ddlPageSize.SelectedValue);
                int offset = (CurrentPage - 1) * pageSize;

                string baseWhere = @" WHERE dr.Status = 1 
                                      AND (d.IsDeleted IS NULL OR d.IsDeleted <> 'X') 
                                      AND (ad.IsDelete IS NULL OR ad.IsDelete <> 'X')";


                if (!string.IsNullOrEmpty(FilterPrimary))
                    baseWhere += $" AND (ISNULL(d.Brand, '') + ' ' + ISNULL(d.Model, '') + ' - SN: ' + ISNULL(d.SerialNumber, '')) LIKE '%{FilterPrimary.Replace("'", "''")}%'";

                if (!string.IsNullOrEmpty(FilterSecondary))
                    baseWhere += $" AND (ISNULL(ad.DeviceType, '') + ' - ' + ISNULL(ad.Description, '')) LIKE '%{FilterSecondary.Replace("'", "''")}%'";

                if (!string.IsNullOrEmpty(FilterDate))
                    baseWhere += $" AND CONVERT(varchar, dr.RecordDate, 104) LIKE '%{FilterDate.Replace("'", "''")}%'";


                string countSql = @"
                    SELECT COUNT(*) 
                    FROM DeviceRel dr
                    LEFT JOIN Device d ON dr.PrimaryDevice = d.DeviceID
                    LEFT JOIN AdditionalDevices ad ON dr.SecondaryDevice = ad.AD_ID" + baseWhere;

                DataTable dtCount = db.SqlToDt(countSql);
                int totalRecords = (dtCount != null && dtCount.Rows.Count > 0 && dtCount.Rows[0][0] != DBNull.Value) ? Convert.ToInt32(dtCount.Rows[0][0]) : 0;

                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
                if (TotalPages == 0) TotalPages = 1;
                if (CurrentPage > TotalPages) CurrentPage = TotalPages;
                offset = (CurrentPage - 1) * pageSize;

                string sql = $@"
                    SELECT 
                        (ISNULL(d.Brand, '') + ' ' + ISNULL(d.Model, '') + ' - SN: ' + ISNULL(d.SerialNumber, 'N/A')) AS PrimaryDeviceName,
                        (ISNULL(ad.DeviceType, '') + ' - ' + ISNULL(ad.Description, 'Undefined')) AS SecondaryDeviceName,
                        ISNULL(CONVERT(varchar, dr.RecordDate, 104), '-') AS RecordDateText
                    FROM DeviceRel dr
                    LEFT JOIN Device d ON dr.PrimaryDevice = d.DeviceID
                    LEFT JOIN AdditionalDevices ad ON dr.SecondaryDevice = ad.AD_ID
                    {baseWhere}
                    ORDER BY dr.RecordDate DESC
                    OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY";

                DataTable dt = db.SqlToDt(sql);

                if (dt != null && dt.Rows.Count > 0)
                {
                    rptRelations.DataSource = dt;
                    rptRelations.DataBind();
                    trNoData.Visible = false;
                    divPagination.Visible = true;

                    SetupPagination(totalRecords, pageSize, dt.Rows.Count);
                }
                else
                {
                    rptRelations.DataSource = null;
                    rptRelations.DataBind();
                    trNoData.Visible = true;
                    divPagination.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Response.Write("<div style='color:red; font-weight:bold;'>C# Error: " + ex.Message + "</div>");
            }
        }

        private void SetupPagination(int totalRecords, int pageSize, int rowCount)
        {

            int startRecord = ((CurrentPage - 1) * pageSize) + 1;
            int endRecord = startRecord + rowCount - 1;
            lblPageInfo.Text = totalRecords == 0 ? "No records to display" : $"Showing {startRecord} - {endRecord} of {totalRecords} records.";

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
                GetRelations();
            }
        }

        protected void lbNext_Click(object sender, EventArgs e)
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                GetRelations();
            }
        }

        protected void rptPagination_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Page")
            {
                CurrentPage = Convert.ToInt32(e.CommandArgument);
                GetRelations();
            }
        }
    }
}