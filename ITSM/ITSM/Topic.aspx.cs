using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;

namespace ITSM
{
    public partial class Konu : System.Web.UI.Page
    {
        CookieTools cookie = new CookieTools();
        DBTools db = new DBTools();
        TranslationTools trn = new TranslationTools();

        protected void Page_Load(object sender, EventArgs e)
        {

            if (Session["User"] == null && string.IsNullOrEmpty(cookie.Oku("User")))
            {
                Response.Redirect("Login.aspx");
                return;
            }


            string userType = cookie.Oku("UserType")?.Trim().ToLower();
            if (userType != "admin")
            {

                Response.Redirect("Requests.aspx");
                return;
            }


            if (!IsPostBack)
            {

                this.Title = gettext("page_title_create", "Yeni Bilgi Bankası Kaydı Oluştur");

                if (Request.QueryString["id"] != null)
                {
                    LoadTopicForEdit(Request.QueryString["id"]);

                    this.Title = gettext("page_title_edit", "Kaydı Düzenle");
                }

                this.DataBind();
            }
        }

        public string gettext(string tag, string fallback)
        {
            try
            {
                string lang = (Session["Lang"] != null) ? Session["Lang"].ToString().ToLower() : "tr";

                if (tag == "tinymce_lang_code")
                {
                    return lang == "en" ? "en" : "tr";
                }

                if (trn != null)
                {
                    var result = trn.GetText(tag, fallback, lang);
                    return result != null ? result.ToString() : fallback;
                }
            }
            catch { }

            return fallback;
        }

        private void LoadTopicForEdit(string id)
        {
            string sql = "SELECT Title, TitleEng, Content, Summary, IsVisible FROM KnowledgeBase WHERE KnowledgeID = " + id;
            DataTable dt = db.SqlToDt(sql);

            if (dt != null && dt.Rows.Count > 0)
            {
                txtKonuBaslik.Text = dt.Rows[0]["Title"].ToString();
                txtKonuBaslikEng.Text = dt.Rows[0]["TitleEng"].ToString();
                txtOzet.Text = dt.Rows[0]["Summary"].ToString();
                chkIsVisible.Checked = Convert.ToBoolean(dt.Rows[0]["IsVisible"]);
                hfEditorIcerik.Value = dt.Rows[0]["Content"].ToString();

                ScriptManager.RegisterStartupScript(this, GetType(), "loadEditorContent",
                    "setTimeout(function() { if(tinymce.get('proeditor')) { tinymce.get('proeditor').setContent(document.getElementById('" + hfEditorIcerik.ClientID + "').value); } }, 800);", true);
            }
        }

        protected void btnKaydet_Click(object sender, EventArgs e)
        {
            try
            {
                string htmlContent = hfEditorIcerik.Value;
                string title = txtKonuBaslik.Text.Trim();
                string titleEng = txtKonuBaslikEng.Text.Trim();
                string summary = txtOzet.Text.Trim();
                bool isVisible = chkIsVisible.Checked;

                string user = cookie.Oku("User") ?? "System";
                string userIDStr = cookie.Oku("UserID") ?? "0";
                int userID = int.TryParse(userIDStr, out int id) ? id : 0;

                string recordId = Request.QueryString["id"];

                if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(htmlContent) || htmlContent == "<p></p>")
                {
                    string warnTitle = gettext("warn_title", "Warning");
                    string warnBody = gettext("warn_body", "Please enter both a title and content!");
                    ScriptManager.RegisterStartupScript(this, GetType(), "warn", $"Swal.fire('{warnTitle}', '{warnBody}', 'warning');", true);
                    return;
                }

                Dictionary<string, object> prms = new Dictionary<string, object>();
                prms["@Title"] = title;
                prms["@TitleEng"] = titleEng;
                prms["@Content"] = htmlContent;
                prms["@Summary"] = summary;
                prms["@IsVisible"] = isVisible;
                prms["@UserID"] = userID;
                prms["@User"] = user;


                if (string.IsNullOrEmpty(recordId))
                {
                    string query = @"INSERT INTO KnowledgeBase (Title, TitleEng, Content, Summary, IsVisible, RecordUser, RecordDate) 
                             VALUES (@Title, @TitleEng, @Content, @Summary, @IsVisible, @User, GETDATE())";

                    if (db.ExecStrWithParams(query, prms))
                    {
                        string successTitle = gettext("success_title", "Successful");
                        string successBody = gettext("msg_added", "New topic added!");
                        ScriptManager.RegisterStartupScript(this, GetType(), "ok", $"Swal.fire('{successTitle}', '{successBody}', 'success').then(() => {{ window.location.href='Requests.aspx'; }});", true);
                    }
                    else
                    {
                        string errorTitle = gettext("error_sql", "SQL Error (Insert)");
                        string sqlHatasi = db.Hata != null ? db.Hata.Replace("'", "") : "Unknown SQL Error";
                        ScriptManager.RegisterStartupScript(this, GetType(), "hata", $"Swal.fire('{errorTitle}', '{sqlHatasi}', 'error');", true);
                    }
                }
                else
                {
                    prms["@KnowledgeID"] = recordId;
                    string query = @"UPDATE KnowledgeBase SET 
                        Title = @Title,
                        TitleEng = @TitleEng,
                        Content = @Content, 
                        Summary = @Summary, 
                        IsVisible = @IsVisible, 
                        UpdateUser = @User, 
                        UpdateDate = GETDATE() 
                      WHERE KnowledgeID = @KnowledgeID";

                    if (db.ExecStrWithParams(query, prms))
                    {
                        string successTitle = gettext("success_title", "Successful");
                        string successBody = gettext("msg_updated", "Topic updated successfully!");
                        ScriptManager.RegisterStartupScript(this, GetType(), "ok", $"Swal.fire('{successTitle}', '{successBody}', 'success').then(() => {{ window.location.href='Requests.aspx'; }});", true);
                    }
                    else
                    {
                        string errorTitle = gettext("error_sql_upd", "SQL Error (Update)");
                        string sqlHatasi = db.Hata != null ? db.Hata.Replace("'", "") : "Unknown SQL Error";
                        ScriptManager.RegisterStartupScript(this, GetType(), "hata", $"Swal.fire('{errorTitle}', '{sqlHatasi}', 'error');", true);
                    }
                }
            }
            catch (Exception ex)
            {
                string sysError = gettext("sys_error", "System Error");
                ScriptManager.RegisterStartupScript(this, GetType(), "hata", $"Swal.fire('{sysError}', '{ex.Message.Replace("'", "")}', 'error');", true);
            }

        }
    }
}