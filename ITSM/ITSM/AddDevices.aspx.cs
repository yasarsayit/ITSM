using System;
using System.Data;
using System.Web.UI.WebControls;

namespace ITSM
{
    public partial class AddDevice : System.Web.UI.Page
    {
        DBTools db = new DBTools();
        CookieTools cookie = new CookieTools();
        CheckTools chk = new CheckTools();
        public int CurrentPage { get { return ViewState["CP"] != null ? (int)ViewState["CP"] : 1; } set { ViewState["CP"] = value; } }
        public int TotalPages { get { return ViewState["TP"] != null ? (int)ViewState["TP"] : 1; } set { ViewState["TP"] = value; } }
        public string FType { get { return ViewState["FT"] as string ?? ""; } set { ViewState["FT"] = value; } }
        public string FDesc { get { return ViewState["FD"] as string ?? ""; } set { ViewState["FD"] = value; } }
        public string FAcc { get { return ViewState["FA"] as string ?? ""; } set { ViewState["FA"] = value; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                VerileriGetir();
            }
        }

        private void VerileriGetir()
        {
            int pageSize = Convert.ToInt32(ddlPageSize.SelectedValue);
            int offset = (CurrentPage - 1) * pageSize;

            string filterCond = "";
            if (!string.IsNullOrEmpty(FType)) filterCond += " AND AD.DeviceType LIKE '%" + chk.temizle(FType) + "%' ";
            if (!string.IsNullOrEmpty(FDesc)) filterCond += " AND AD.Description LIKE '%" + chk.temizle(FDesc) + "%' ";
            if (!string.IsNullOrEmpty(FAcc)) filterCond += " AND AD.AccountingCode LIKE '%" + chk.temizle(FAcc) + "%' ";

            string baseWhere = " WHERE AD.IsDelete IS NULL " + filterCond;

            string countSql = "SELECT COUNT(AD.AD_ID) FROM AdditionalDevices AD " + baseWhere;
            DataTable dtCount = db.SqlToDt(countSql);
            int totalRecords = (dtCount != null && dtCount.Rows.Count > 0 && dtCount.Rows[0][0] != DBNull.Value) ? Convert.ToInt32(dtCount.Rows[0][0]) : 0;

            TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            if (TotalPages == 0) TotalPages = 1;
            if (CurrentPage > TotalPages) CurrentPage = TotalPages;
            offset = (CurrentPage - 1) * pageSize;

            string sql = "SELECT " + Environment.NewLine;
            sql += " AD.*, " + Environment.NewLine;
            sql += " ISNULL(STUFF((SELECT ', ' + (D.Brand + ' ' + D.Model + ' [' + D.SerialNumber + ']') " + Environment.NewLine;
            sql += "               FROM DeviceRel DR " + Environment.NewLine;
            sql += "               INNER JOIN device D ON DR.PrimaryDevice = D.DeviceID " + Environment.NewLine;
            sql += "               WHERE DR.SecondaryDevice = AD.AD_ID AND DR.Status = 1 " + Environment.NewLine;
            sql += "               FOR XML PATH('')), 1, 2, ''), 'Standalone') as MainDeviceName " + Environment.NewLine;
            sql += " FROM AdditionalDevices AD " + Environment.NewLine;
            sql += baseWhere + " " + Environment.NewLine;
            sql += " ORDER BY AD.AD_ID DESC " + Environment.NewLine;
            sql += " OFFSET " + offset + " ROWS FETCH NEXT " + pageSize + " ROWS ONLY";

            DataTable dt = db.SqlToDt(sql);
            rptAdditionalDevices.DataSource = dt;
            rptAdditionalDevices.DataBind();

            SetupPagination(totalRecords, pageSize, dt.Rows.Count);
        }

        private void SetupPagination(int totalRecords, int pageSize, int currentRows)
        {
            lblPageInfo.Text = $"Showing {(CurrentPage - 1) * pageSize + 1} to {(CurrentPage - 1) * pageSize + currentRows} of {totalRecords} entries";
            DataTable dtPages = new DataTable();
            dtPages.Columns.Add("PageNumber", typeof(int));
            dtPages.Columns.Add("IsActive", typeof(bool));
            int startPage = Math.Max(1, CurrentPage - 2);
            int endPage = Math.Min(TotalPages, CurrentPage + 2);
            for (int i = startPage; i <= endPage; i++) dtPages.Rows.Add(i, i == CurrentPage);
            rptPagination.DataSource = dtPages;
            rptPagination.DataBind();
            liPrev.Attributes["class"] = CurrentPage > 1 ? "page-item" : "page-item disabled";
            liNext.Attributes["class"] = CurrentPage < TotalPages ? "page-item" : "page-item disabled";
        }

        protected void lbAra_Click(object sender, EventArgs e)
        {
            RepeaterItem headerItem = rptAdditionalDevices.Controls[0] as RepeaterItem;
            if (headerItem != null)
            {
                FType = (headerItem.FindControl("txtFType") as TextBox)?.Text.Trim();
                FDesc = (headerItem.FindControl("txtFDesc") as TextBox)?.Text.Trim();
                FAcc = (headerItem.FindControl("txtFAcc") as TextBox)?.Text.Trim();
            }
            CurrentPage = 1;
            VerileriGetir();
        }

        protected void lbAraTemizle_Click(object sender, EventArgs e)
        {
            FType = ""; FDesc = ""; FAcc = "";
            CurrentPage = 1;
            VerileriGetir();
        }

        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentPage = 1;
            VerileriGetir();
        }

        protected void lbPrev_Click(object sender, EventArgs e)
        {
            if (CurrentPage > 1) { CurrentPage--; VerileriGetir(); }
        }

        protected void lbNext_Click(object sender, EventArgs e)
        {
            if (CurrentPage < TotalPages) { CurrentPage++; VerileriGetir(); }
        }

        protected void rptPagination_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Page")
            {
                CurrentPage = Convert.ToInt32(e.CommandArgument);
                VerileriGetir();
            }
        }

        protected void btnDeleteConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                string currentUser = cookie.Oku("User");
                if (string.IsNullOrEmpty(currentUser)) currentUser = "System";

                int delId = Convert.ToInt32(hfDeleteId.Value);


                string checkRelSql = "SELECT COUNT(*) FROM DeviceRel WHERE SecondaryDevice = " + delId + " AND Status = 1";
                int relCount = Convert.ToInt32(db.SqlToDt(checkRelSql).Rows[0][0]);

                if (relCount > 0)
                {
                    ShowAlert("error", "Cannot Delete!", "This additional device is currently linked to a main device. Please disconnect it first.");
                    return;
                }

                string sqlDelete = "UPDATE AdditionalDevices SET IsDelete = 'X', DeleteDate = GETDATE(), DeleteUser = '" + currentUser + "' WHERE AD_ID = " + delId;

                if (db.ExecStr(sqlDelete) > 0)
                {
                    ShowAlert("success", "Success", "Record deleted successfully.");
                }
                else
                {
                    ShowAlert("error", "Error", "Could not delete: " + db.Hata);
                }

                hfDeleteId.Value = "";
                VerileriGetir();
            }
            catch (Exception ex)
            {
                ShowAlert("error", "System Error", ex.Message);
            }
        }

        protected void rptAdditionalDevices_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Edit")
            {
                Response.Redirect("AddDeviceForm.aspx?id=" + Convert.ToInt32(e.CommandArgument));
            }
        }

        private void ShowAlert(string type, string title, string message)
        {
            hfAlertType.Value = type;
            hfAlertTitle.Value = title;
            hfAlertMessage.Value = message;
        }
    }
}