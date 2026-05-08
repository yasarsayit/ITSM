using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ITSM
{
    public partial class TopicList : Page
    {

        public string FStatus { get { return ViewState["FS"] as string ?? ""; } set { ViewState["FS"] = value; } }
        public string FDate { get { return ViewState["FD"] as string ?? ""; } set { ViewState["FD"] = value; } }
        public int CurrentPage { get { return ViewState["CP"] != null ? (int)ViewState["CP"] : 1; } set { ViewState["CP"] = value; } }
        public int TotalPages { get { return ViewState["TP"] != null ? (int)ViewState["TP"] : 1; } set { ViewState["TP"] = value; } }
        public string FTitle { get { return ViewState["FT"] as string ?? ""; } set { ViewState["FT"] = value; } }
        public string FID { get { return ViewState["FID"] as string ?? ""; } set { ViewState["FID"] = value; } }
        CheckTools chk = new CheckTools();
        DBTools db = new DBTools();
        CookieTools cookie = new CookieTools();
        TranslationTools trn = new TranslationTools();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                this.Title = gettext("topiclisttitle", "TOPIC LIST");

                if (Session["User"] == null && string.IsNullOrEmpty(cookie.Oku("User")))
                {
                    Response.Redirect("Login.aspx");
                    return;
                }

                if (!IsPostBack)
                {
                    LoadTopics();
                    this.DataBind();
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error",
                    "Swal.fire('System Error', 'An error occurred while loading the page: " + ex.Message.Replace("'", "") + "', 'error');", true);
            }
        }
        public string gettext(string tag, string fallback)
        {
            try
            {
                string lang = (Session != null && Session["Lang"] != null) ? Session["Lang"].ToString() : "TR";
                var result = trn.GetText(tag, fallback, lang);
                return result != null ? result.ToString() : fallback;
            }
            catch { return fallback; }
        }
        private void LoadTopics()
        {
            int pageSize = Convert.ToInt32(ddlPageSize.SelectedValue);
            string filter = " WHERE (IsDeleted IS NULL OR IsDeleted != 'X') ";

            if (!string.IsNullOrEmpty(FTitle)) filter += $" AND Title LIKE '%{chk.temizle(FTitle)}%' ";
            if (!string.IsNullOrEmpty(FID)) filter += $" AND KnowledgeID LIKE '%{chk.temizle(FID)}%' ";

            if (!string.IsNullOrEmpty(FStatus)) filter += $" AND IsVisible = {FStatus} ";

            if (!string.IsNullOrEmpty(FDate)) filter += $" AND CAST(RecordDate AS DATE) = '{chk.temizle(FDate)}' ";

            int totalRecords = Convert.ToInt32(db.SqlToDt("SELECT COUNT(*) FROM KnowledgeBase " + filter).Rows[0][0]);
            TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            if (TotalPages == 0) TotalPages = 1;
            int offset = (CurrentPage - 1) * pageSize;

            string sql = $@"SELECT KnowledgeID, Title, IsVisible, RecordDate FROM KnowledgeBase {filter} 
                    ORDER BY KnowledgeID DESC OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY";

            DataTable dt = db.SqlToDt(sql);
            rptTopicList.DataSource = dt;
            rptTopicList.DataBind();

            if (rptTopicList.Controls.Count > 0)
            {
                var header = rptTopicList.Controls[0].FindControl("ddlFStatus") as DropDownList;
                if (header != null && header.Items.Count >= 3)
                {
                    header.Items[0].Text = gettext("all_selection", "All");
                    header.Items[1].Text = gettext("status_visible", "Visible");
                    header.Items[2].Text = gettext("status_hidden", "Hidden");
                }
            }

            string showing = gettext("txt_showing", "Showing");
            string records = gettext("txt_records", "records");
            string between = gettext("txt_between", "between");

            int startRecord = totalRecords == 0 ? 0 : offset + 1;
            int endRecord = Math.Min(offset + pageSize, totalRecords);
            lblPageInfo.Text = $"{showing} {totalRecords} {records} {startRecord}-{endRecord} {between}.";

            lbPrev.DataBind();
            lbNext.DataBind();

            DataTable dtPages = new DataTable();
            dtPages.Columns.Add("PageNumber");
            dtPages.Columns.Add("IsActive", typeof(bool));
            for (int i = 1; i <= TotalPages; i++) dtPages.Rows.Add(i, i == CurrentPage);
            rptPagination.DataSource = dtPages;
            rptPagination.DataBind();

            liPrev.Attributes["class"] = CurrentPage > 1 ? "page-item" : "page-item disabled";
            liNext.Attributes["class"] = CurrentPage < TotalPages ? "page-item" : "page-item disabled";
        }

        protected void lbSearch_Click(object sender, EventArgs e)
        {
            var header = rptTopicList.Controls[0];
            FTitle = ((TextBox)header.FindControl("txtFTitle")).Text;
            FID = ((TextBox)header.FindControl("txtFID")).Text;

            FStatus = ((DropDownList)header.FindControl("ddlFStatus")).SelectedValue;
            FDate = ((TextBox)header.FindControl("txtFDate")).Text;

            CurrentPage = 1;
            LoadTopics();
        }
        protected void lbClear_Click(object sender, EventArgs e)
        {
            FTitle = ""; FID = ""; FStatus = ""; FDate = ""; 
            LoadTopics();
        }
        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e) { CurrentPage = 1; LoadTopics(); }
        protected void lbPrev_Click(object sender, EventArgs e) { CurrentPage--; LoadTopics(); }
        protected void lbNext_Click(object sender, EventArgs e) { CurrentPage++; LoadTopics(); }
        protected void rptPagination_ItemCommand(object source, RepeaterCommandEventArgs e) { CurrentPage = Convert.ToInt32(e.CommandArgument); LoadTopics(); }
    }
}