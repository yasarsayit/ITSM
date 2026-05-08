using ITSM;
using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ITSM
{
    public partial class Admin : System.Web.UI.Page
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

        public string FilterReqID { get { return ViewState["F_ReqID"] as string ?? ""; } set { ViewState["F_ReqID"] = value; } }
        public string FilterTopic { get { return ViewState["F_Topic"] as string ?? ""; } set { ViewState["F_Topic"] = value; } }
        public string FilterType { get { return ViewState["F_Type"] as string ?? ""; } set { ViewState["F_Type"] = value; } }
        public string FilterUser { get { return ViewState["F_User"] as string ?? ""; } set { ViewState["F_User"] = value; } }
        public string FilterStatus { get { return ViewState["F_Status"] as string ?? ""; } set { ViewState["F_Status"] = value; } }
        public string FilterReqDate { get { return ViewState["F_ReqDate"] as string ?? ""; } set { ViewState["F_ReqDate"] = value; } }
        public string FilterDueDate { get { return ViewState["F_DueDate"] as string ?? ""; } set { ViewState["F_DueDate"] = value; } }
        public string FilterPriority { get { return ViewState["F_Priority"] as string ?? ""; } set { ViewState["F_Priority"] = value; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["User"] == null && string.IsNullOrEmpty(cookie.Oku("User")))
            {
                Response.Redirect("Login.aspx");
                return;
            }

            string uType = cookie.Oku("UserType")?.Trim().ToLower();
            if (uType != "admin")
            {
                Response.Redirect("Default.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CurrentPage = 1;
                ResetFilters();
                LoadRequestSubTypes();
                FetchData();
            }
        }

        private void LoadRequestSubTypes()
        {
            try
            {
                ddlFilterType.Items.Clear();
                ddlFilterType.Items.Add(new ListItem("All", ""));
                string sql = "SELECT rst.SubTypeID, (rt.Description + ' - ' + rst.Description) as FullName FROM RequestSubTypes rst LEFT JOIN RequestTypes rt ON rst.ReqTypeId = rt.TypeID ORDER BY rt.Description, rst.Description ASC";
                DataTable dt = db.SqlToDt(sql);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                        ddlFilterType.Items.Add(new ListItem(row["FullName"].ToString(), row["SubTypeID"].ToString()));
                }
            }
            catch (Exception ex) { ScriptManager.RegisterStartupScript(this, GetType(), "showError", $"alert('Kategori Yükleme Hatası: {chk.temizle(ex.Message)}');", true); }
        }

        protected string GetScoreBadge(object score, object isConfirmed)
        {

            if (score == null || score == DBNull.Value || string.IsNullOrEmpty(score.ToString()))
            {
                return "<span class='text-muted' style='font-size:11px;'>-</span>";
            }

            string s = score.ToString();
            if (s == "0") return "<span class='badge bg-danger' title='0 - Far below expectations'><i class='fas fa-star text-white'></i> 0</span>";
            if (s == "1") return "<span class='badge bg-warning text-dark' title='1 - Below expectations'><i class='fas fa-star text-dark'></i> 1</span>";
            if (s == "2") return "<span class='badge bg-primary' title='2 - Met expectations'><i class='fas fa-star text-white'></i> 2</span>";
            if (s == "3") return "<span class='badge bg-success' title='3 - Above expectations'><i class='fas fa-star text-white'></i> 3</span>";

            return "<span class='badge bg-secondary'>" + s + "</span>";
        }
        private void ResetFilters()
        {
            FilterReqID = ""; FilterTopic = ""; FilterType = ""; FilterUser = ""; FilterStatus = "";
            FilterReqDate = ""; FilterDueDate = ""; FilterPriority = "";
        }

        private void FetchData()
        {
            try
            {
                int pageSize = Convert.ToInt32(ddlPageSize.SelectedValue);
                int offset = (CurrentPage - 1) * pageSize;
                string filterCondition = "";

                if (!string.IsNullOrEmpty(FilterReqID)) { if (int.TryParse(chk.temizle(FilterReqID), out int reqIdParsed)) filterCondition += " AND r.RequestId = " + reqIdParsed + " "; }
                if (!string.IsNullOrEmpty(FilterTopic)) filterCondition += " AND r.Topic LIKE '%" + chk.temizle(FilterTopic) + "%' ";
                if (!string.IsNullOrEmpty(FilterType)) filterCondition += " AND r.RequestSubtype = " + chk.temizle(FilterType) + " ";
                if (!string.IsNullOrEmpty(FilterUser)) filterCondition += " AND (r.RecordUser LIKE '%" + chk.temizle(FilterUser) + "%' OR u.NameSurname LIKE '%" + chk.temizle(FilterUser) + "%') ";

                if (!string.IsNullOrEmpty(FilterReqDate))
                    filterCondition += " AND CONVERT(varchar, r.RequestDate, 23) = '" + chk.temizle(FilterReqDate) + "' ";

                if (!string.IsNullOrEmpty(FilterDueDate))
                    filterCondition += " AND CONVERT(varchar, r.DueDate, 23) = '" + chk.temizle(FilterDueDate) + "' ";

                if (!string.IsNullOrEmpty(FilterPriority))
                {
                    if (FilterPriority == "Urgent")
                        filterCondition += " AND r.IsUrgent = 1 ";
                    else if (FilterPriority == "Important")
                        filterCondition += " AND r.IsImportant = 1 ";
                    else if (FilterPriority == "Standard")
                        filterCondition += " AND (r.IsUrgent IS NULL OR r.IsUrgent = 0) AND (r.IsImportant IS NULL OR r.IsImportant = 0) ";
                }


                if (!string.IsNullOrEmpty(FilterStatus))
                {
                    string safeStatus = chk.temizle(FilterStatus);

                    if (safeStatus == "ONGOING")
                    {

                        filterCondition += " AND (r.IsConfirmed IS NULL OR r.IsConfirmed = '0' OR r.IsConfirmed = 0 OR r.IsConfirmed = '1') ";
                    }
                    else if (safeStatus == "ENDED")
                    {

                        filterCondition += " AND (r.IsConfirmed = '2' OR r.IsConfirmed = '3') ";
                    }
                    else if (safeStatus == "0")
                    {

                        filterCondition += " AND (r.IsConfirmed IS NULL OR r.IsConfirmed = '0' OR r.IsConfirmed = 0) ";
                    }
                    else
                    {

                        filterCondition += " AND r.IsConfirmed = '" + safeStatus + "' ";
                    }
                }


                string countSql = "SELECT COUNT(r.RequestId) FROM Requests r LEFT JOIN Users u ON r.UserID = u.UserID WHERE (r.isDeleted IS NULL OR r.isDeleted != 'X')" + filterCondition;
                DataTable dtCount = db.SqlToDt(countSql);
                int totalRecords = dtCount.Rows.Count > 0 ? Convert.ToInt32(dtCount.Rows[0][0]) : 0;


                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
                if (TotalPages == 0) TotalPages = 1;
                if (CurrentPage > TotalPages) CurrentPage = TotalPages;
                offset = (CurrentPage - 1) * pageSize;


                string sql =
 "SELECT" + Environment.NewLine +
 "    r.RequestId," + Environment.NewLine +
 "    r.Topic," + Environment.NewLine +
 "    rt.Description AS TypeName," + Environment.NewLine +
 "    rst.Description AS SubTypeName," + Environment.NewLine +
 "    r.RequestDate," + Environment.NewLine +
 "    r.RecordUser AS CreatedBy," + Environment.NewLine +
 "    u.NameSurname AS RequestOwner," + Environment.NewLine +
 "    r.DueDate," + Environment.NewLine +
 "    r.IsImportant," + Environment.NewLine +
 "    r.IsUrgent," + Environment.NewLine +
 "    r.IsConfirmed," + Environment.NewLine +
 "    r.Score," + Environment.NewLine +
 "    CASE " + Environment.NewLine +
 "        WHEN NOT EXISTS ( " + Environment.NewLine +
 "            SELECT 1 FROM Tasks t WHERE t.RequestID = r.RequestId " + Environment.NewLine +
 "        ) " + Environment.NewLine +
 "        AND DATEDIFF(MINUTE, r.RequestDate, GETDATE()) > ( " + Environment.NewLine +
 "            SELECT TOP 1 CAST(ParameterValue AS INT) " + Environment.NewLine +
 "            FROM Parameters " + Environment.NewLine +
 "            WHERE ParameterName = 'SLA' " + Environment.NewLine +
 "        ) " + Environment.NewLine +
 "        THEN 1 " + Environment.NewLine +
 "        ELSE 0 " + Environment.NewLine +
 "    END AS IsSlaExceeded " + Environment.NewLine +
 "FROM Requests r " + Environment.NewLine +
 "LEFT JOIN RequestTypes rt ON r.RequestType = rt.TypeID " + Environment.NewLine +
 "LEFT JOIN RequestSubTypes rst ON r.RequestSubtype = rst.SubTypeID " + Environment.NewLine +
 "LEFT JOIN Users u ON r.UserID = u.UserID " + Environment.NewLine +
 "WHERE (r.isDeleted IS NULL OR r.isDeleted <> 'X') " + Environment.NewLine +
 filterCondition + Environment.NewLine +
 "ORDER BY " + Environment.NewLine +
 "    CASE " + Environment.NewLine +
 "        WHEN ISNULL(CAST(r.IsUrgent AS INT),0) = 1 AND ISNULL(CAST(r.IsImportant AS INT),0) = 1 THEN 1 " + Environment.NewLine +
 "        WHEN ISNULL(CAST(r.IsUrgent AS INT),0) = 1 THEN 2 " + Environment.NewLine +
 "        WHEN ISNULL(CAST(r.IsImportant AS INT),0) = 1 THEN 3 " + Environment.NewLine +
 "        ELSE 4 " + Environment.NewLine +
 "    END, " + Environment.NewLine +
 "    ISNULL(r.IsConfirmed, 0), " + Environment.NewLine +
 "    r.RequestId DESC " + Environment.NewLine +
 "OFFSET " + offset + " ROWS " + Environment.NewLine +
 "FETCH NEXT " + pageSize + " ROWS ONLY";

                DataTable dt = db.SqlToDt(sql);
                rptAdminRequests.DataSource = dt;
                rptAdminRequests.DataBind();
                trNoData.Visible = (dt == null || dt.Rows.Count == 0);
                SetupPagination(totalRecords, pageSize);
            }
            catch (Exception) { }
        }
        protected string GetRowColorClass(object isConfirmed, object isSlaExceeded)
        {

            string status = isConfirmed != DBNull.Value ? isConfirmed.ToString() : "0";
            bool slaAsimi = isSlaExceeded != DBNull.Value && isSlaExceeded.ToString() == "1";

            if (status == "0" && slaAsimi)
            {
                return "table-danger fw-bold";
            }

            if (status == "1") return "table-success";
            if (status == "2") return "opacity-50 text-muted";
            if (status == "3") return "";
            if (status == "0") return "table-warning";

            return "table-warning";
        }

        protected string GetSlaTextColor(object isConfirmed, object isSlaExceeded)
        {
            if (isSlaExceeded != DBNull.Value && isSlaExceeded.ToString() == "1") return "color: #721c24;";
            return "color: #0F406B;";
        }
        protected string GetStatusBadge(object isConfirmed)
        {
            if (isConfirmed != DBNull.Value)
            {
                if (isConfirmed.ToString() == "1") return "<span class='badge bg-success' title='APPROVED' style='font-size: 13px; padding: 6px 8px;'><i class='fal fa-check'></i></span>";
                if (isConfirmed.ToString() == "2") return "<span class='badge bg-danger' title='REJECTED' style='font-size: 13px; padding: 6px 8px;'><i class='fal fa-times'></i></span>";
                if (isConfirmed.ToString() == "3") return "<span class='badge bg-info' title='COMPLETED' style='font-size: 13px; padding: 6px 8px;'><i class='fal fa-check-double'></i></span>";
            }
            return "<span class='badge bg-secondary' title='PENDING' style='font-size: 13px; padding: 6px 8px;'><i class='fal fa-clock'></i></span>";
        }

        protected string GetPriorityBadges(object isImportant, object isUrgent)
        {
            bool urgent = isUrgent != DBNull.Value && (Convert.ToBoolean(isUrgent) || isUrgent.ToString() == "1");
            bool important = isImportant != DBNull.Value && (Convert.ToBoolean(isImportant) || isImportant.ToString() == "1");
            string html = "";
            html += urgent ? $"<span class='badge bg-danger me-1' title='URGENT'><i class=\"far fa-exclamation\"></i></span>" : "";
            html += important ? $"<span class='badge bg-warning text-dark me-1' title='IMPORTANT'><i class=\"fal fa-user-crown\"></i></span>" : "";
            return html;
        }

        protected void lbAra_Click(object sender, EventArgs e) { FilterReqID = txtFilterReqID.Text.Trim(); FilterTopic = txtFilterTopic.Text.Trim(); FilterType = ddlFilterType.SelectedValue; FilterUser = txtFilterUser.Text.Trim(); FilterStatus = ddlFilterStatus.SelectedValue; FilterReqDate = txtFilterReqDate.Text.Trim(); FilterDueDate = txtFilterDueDate.Text.Trim(); FilterPriority = ddlFilterPriority.SelectedValue; CurrentPage = 1; FetchData(); }
        protected void lbAraTemizle_Click(object sender, EventArgs e) { Response.Redirect("Admin.aspx"); }
        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e) { CurrentPage = 1; FetchData(); }
        protected void lbPrev_Click(object sender, EventArgs e) { if (CurrentPage > 1) { CurrentPage--; FetchData(); } }
        protected void lbNext_Click(object sender, EventArgs e) { if (CurrentPage < TotalPages) { CurrentPage++; FetchData(); } }
        protected void rptPagination_ItemCommand(object source, RepeaterCommandEventArgs e) { if (e.CommandName == "Page") { CurrentPage = Convert.ToInt32(e.CommandArgument); FetchData(); } }
        protected void rptAdminRequests_ItemCommand(object source, RepeaterCommandEventArgs e) { if (e.CommandName == "Review") Response.Redirect("AdminForm.aspx?id=" + chk.temizle(e.CommandArgument.ToString())); }

        private void SetupPagination(int totalRecords, int pageSize)
        {
            int startRecord = ((CurrentPage - 1) * pageSize) + 1;
            int endRecord = Math.Min(CurrentPage * pageSize, totalRecords);
            lblPageInfo.Text = totalRecords == 0 ? "Showing 0 entries" : $"Showing {startRecord} to {endRecord} of {totalRecords} entries";
            DataTable dtPages = new DataTable(); dtPages.Columns.Add("PageNumber"); dtPages.Columns.Add("IsActive", typeof(bool));
            int startPage = Math.Max(1, CurrentPage - 3); int endPage = Math.Min(TotalPages, CurrentPage + 3);
            for (int i = startPage; i <= endPage; i++) dtPages.Rows.Add(i, i == CurrentPage);
            rptPagination.DataSource = dtPages; rptPagination.DataBind();
            liPrev.Attributes["class"] = CurrentPage > 1 ? "page-item" : "page-item disabled";
            liNext.Attributes["class"] = CurrentPage < TotalPages ? "page-item" : "page-item disabled";
        }
    }
}