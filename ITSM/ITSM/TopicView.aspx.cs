using System;
using System.Data;
using System.Web.UI;

namespace ITSM
{
    public partial class TopicView : Page
    {
        DBTools db = new DBTools();
        TranslationTools trn = new TranslationTools();
        CookieTools cookie = new CookieTools(); 

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                this.Title = gettext("topic_view_title", "VIEW TOPIC");

                if (Session["User"] == null && string.IsNullOrEmpty(cookie.Oku("User")))
                {
                    Response.Redirect("Login.aspx");
                    return;
                }

                if (!IsPostBack)
                {
                    string topicId = Request.QueryString["id"];
                    if (!string.IsNullOrEmpty(topicId))
                    {
                        LoadTopic(topicId);
                    }
                    else
                    {
                        Response.Redirect("TopicList.aspx");
                    }

                    this.DataBind();
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error",
                    "alert('System Error: " + ex.Message.Replace("'", "") + "');", true);
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


        private void LoadTopic(string id)
        {
            try
            {
                string sql = "SELECT Title, Summary, Content FROM KnowledgeBase WHERE KnowledgeID = " + id;
                DataTable dt = db.SqlToDt(sql);

                if (dt != null && dt.Rows.Count > 0)
                {
                    litTitle.Text = dt.Rows[0]["Title"].ToString();
                    litSummary.Text = dt.Rows[0]["Summary"].ToString();
                    litContent.Text = dt.Rows[0]["Content"].ToString();
                }
                else
                {
                    litTitle.Text = "Topic Not Found";
                    litContent.Text = "<p>The requested topic could not be found or has been deleted.</p>";
                }
            }
            catch (Exception ex)
            {
                litTitle.Text = "System Error";
                litContent.Text = "<p>An error occurred: " + ex.Message + "</p>";
            }
        }
    }
}