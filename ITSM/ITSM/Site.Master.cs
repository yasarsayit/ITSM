using Antlr.Runtime;
using System;
using System.Web.UI;

namespace ITSM
{
    public partial class SiteMaster : MasterPage
    {
        DBTools db = new DBTools();
        CookieTools cookie = new CookieTools();
        CheckTools ch = new CheckTools();
        TranslationTools trn = new TranslationTools();
        protected void Page_Load(object sender, EventArgs e)
        {

         
            if (Session["User"] == null && string.IsNullOrEmpty(cookie.Oku("User")))
            {
                logout();
                return;
            }

     
            ltName.Text = cookie.Oku("namesurname");
            ltUser.Text = cookie.Oku("User");
            lbEmail.Text = cookie.Oku("useremail");

            string userType = cookie.Oku("UserType");
            if (userType != null && userType.ToLower() == "admin")
            {
                liTopicParent.Visible = true;
                liInventoryParent.Visible = true;
                liAdminMenu.Visible = true;
                liReports.Visible = true;
                liCategoryMenu.Visible = true;
            }
            else
            {
                liTopicParent.Visible = false;
                liInventoryParent.Visible = false;
                liAdminMenu.Visible = false;
                liReports.Visible = false;
                liCategoryMenu.Visible = false;
            }


            if (!IsPostBack)
            {
                LoadLanguageDropdown();
            }
        }

        public string gettext(string tag, string fallback)
        {
            return trn.GetText(tag, fallback, Session["Lang"].ToString()).ToString();
        }
        private void LoadLanguageDropdown()
        {
            ddlLang.Items.Clear();
            ddlLang.Items.Add(new System.Web.UI.WebControls.ListItem("TR", "TR"));
            ddlLang.Items.Add(new System.Web.UI.WebControls.ListItem("EN", "EN"));
 

            string currentLang = GetCurrentLang();

            if (ddlLang.Items.FindByValue(currentLang) != null)
                ddlLang.SelectedValue = currentLang;
            else
                ddlLang.SelectedValue = "EN";
        }

        public string GetCurrentLang()
        {
            if (Session["Lang"] != null && !string.IsNullOrWhiteSpace(Session["Lang"].ToString()))
                return Session["Lang"].ToString().ToUpperInvariant();

            string detectedLang = GetBrowserOrSystemLang();
            Session["Lang"] = detectedLang;
            return detectedLang;
        }

        private string GetBrowserOrSystemLang()
        {
            try
            {
                string uiLang = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToUpperInvariant();

                switch (uiLang)
                {
                    case "TR":
                    case "EN":
                
                        return uiLang;
                }

                var langs = Request.UserLanguages;

                if (langs != null)
                {
                    foreach (var l in langs)
                    {
                        if (string.IsNullOrWhiteSpace(l) || l.Length < 2)
                            continue;

                        string lang = l.Substring(0, 2).ToUpperInvariant();

                        switch (lang)
                        {
                            case "TR":
                            case "EN":
                           
                                return lang;
                        }
                    }
                }
            }
            catch
            {
            }

            return "EN";
        }

        protected void ddlLang_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["Lang"] = ddlLang.SelectedValue;

            // İstersen cookie'ye de yazabilirsin:
            // cookie.Yaz("Lang", ddlLang.SelectedValue);

            Response.Redirect(Request.RawUrl, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        public void logout()
        {
            Session.Abandon();
            Session.Clear();
            cookie.ClearAuthCookies();
            cookie.Sil("User");
            cookie.Sil("Password");
            Response.Redirect("Login.aspx");
        }

        public void lbLogOut_Click(object sender, EventArgs e)
        {
            logout();
        }
    }
}