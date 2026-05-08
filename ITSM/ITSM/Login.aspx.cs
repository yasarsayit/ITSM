using System;
using System.Data;

namespace ITSM
{
    public partial class Login : System.Web.UI.Page
    {
        DBTools db = new DBTools();
        CookieTools cookie = new CookieTools();
        CheckTools ch = new CheckTools();
        TranslationTools trn = new TranslationTools();

        string lang = "tr";

        protected void Page_Load(object sender, EventArgs e)
        {


            if (!IsPostBack)
            {
                Session["Lang"] = trn.GetLangCode();

                if (Session["Lang"] == null)
                {
                    Session["Lang"] = trn.GetLangCode();
                    lang = Session["Lang"].ToString();
                }


                if (Session["DCUSER"] != null && !string.IsNullOrWhiteSpace(Session["DCUSER"].ToString()))
                {
                    OrtakLoginIleGirisYap();
                }
            }
        }

        public string gettext(string tag, string fallback)
        {
            if (Session["Lang"] == null)
            {
                Session["Lang"] = trn.GetLangCode();
            }
            return trn.GetText(tag, fallback, Session["Lang"].ToString()).ToString();
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                lblMessage.Text = gettext("loginnullwrn", "Please enter your username and password.");
                return;
            }

            GirisYap(txtUsername.Text, txtPassword.Text);
        }

        void GirisYap(string username, string password)
        {
            string username_ = ch.temizle(username.Trim());
            string password_ = ch.temizle(password.Trim());

            string sql = "SELECT * FROM Users WHERE UserName = '" + username_ + "' AND Pass = '" + password_ + "' AND Status = 1";
            DataTable dt = db.SqlToDt(sql);

            if (dt.Rows.Count > 0)
            {
                string uType = dt.Rows[0]["UserType"].ToString().Trim();

                Session["User"] = username_;
                Session["UserType"] = uType;

                cookie.Yaz("User", username_);
                cookie.Yaz("UserType", uType);
                cookie.Yaz("namesurname", dt.Rows[0]["NameSurname"].ToString());
                cookie.Yaz("useremail", dt.Rows[0]["Email"].ToString());

                Response.Redirect("Default.aspx", false);
            }
            else
            {
                lblMessage.Text = gettext("loginwrongwrn", "Username or password is incorrect.");
            }
        }

        void OrtakLoginIleGirisYap()
        {
            try
            {
                string dcUser = Session["DCUSER"] != null ? Session["DCUSER"].ToString().Trim() : "";
                string dcMail = Session["DCMAIL"] != null ? Session["DCMAIL"].ToString().Trim() : "";
                string dcAdSoyad = Session["DCADSOYAD"] != null ? Session["DCADSOYAD"].ToString().Trim() : "";
                string sicilNo = Session["SICILNO"] != null ? Session["SICILNO"].ToString().Trim() : "";

                if (string.IsNullOrWhiteSpace(dcUser))
                    return;

                // Portal mail/adsoyad boş gelirse AD'den tamamla
                if (string.IsNullOrWhiteSpace(dcMail) || string.IsNullOrWhiteSpace(dcAdSoyad))
                {
                    ADTools ad = new ADTools();
                    string[] info = ad.GetUserInfo(dcUser);

                    if (string.IsNullOrWhiteSpace(dcMail))
                        dcMail = info[0];

                    if (string.IsNullOrWhiteSpace(dcAdSoyad))
                        dcAdSoyad = info[1];

                    if (string.IsNullOrWhiteSpace(sicilNo))
                        sicilNo = info[2];
                }

                string userType = "user";

                string sqlYetki = "";
                sqlYetki += "SELECT TOP 1 LVL FROM HAVUZ.dbo.UYGULAMAYETKILERI ";
                sqlYetki += "WHERE DCUSER = '" + ch.temizle(dcUser) + "' AND UYGULAMA = 1 ";

                DataTable dtYetki = db.SqlToDt(sqlYetki);

                if (dtYetki.Rows.Count > 0)
                {
                    string lvl = dtYetki.Rows[0]["LVL"].ToString().Trim();

                    if (lvl == "99")
                        userType = "admin";
                }

                // Users tablosunda kullanıcıyı bul / yoksa oluştur
                string userId = EnsureUserExistsFromDC(dcUser, dcMail, dcAdSoyad, sicilNo, userType);


                Session["User"] = dcUser;
                Session["UserType"] = userType;
                Session["UserID"] = userId;

                cookie.Yaz("User", dcUser);
                cookie.Yaz("UserType", userType);
                cookie.Yaz("namesurname", dcAdSoyad);
                cookie.Yaz("useremail", dcMail);

                Response.Redirect("Default.aspx", false);
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Ortak login hatası: " + ex.Message;
            }
        }


        private string EnsureUserExistsFromDC(string dcUser, string dcMail, string dcAdSoyad, string sicilNo, string userType)
        {
            try
            {
                string cleanUser = ch.temizle(dcUser);
                string cleanMail = ch.temizle(dcMail);
                string cleanAdSoyad = ch.temizle(dcAdSoyad);
                string cleanSicil = ch.temizle(sicilNo);
                string cleanUserType = ch.temizle(userType);

                // 1) Önce mevcut kullanıcıyı bul
                string sqlFind = "";
                sqlFind += "SELECT TOP 1 UserID ";
                sqlFind += "FROM Users ";
                sqlFind += "WHERE isDeleted IS NULL AND (";
                sqlFind += "UserName = '" + cleanUser + "' ";

                if (!string.IsNullOrWhiteSpace(cleanMail))
                    sqlFind += "OR Email = '" + cleanMail + "' ";

                if (!string.IsNullOrWhiteSpace(cleanSicil))
                    sqlFind += "OR EmployeeID = '" + cleanSicil + "' ";

                sqlFind += ")";


                string title = "";

                if (userType == "admin")
                {
                    title = "Manager";

                }
                else
                {
                    title = "User";
                }

                DataTable dtFind = db.SqlToDt(sqlFind);

                if (dtFind.Rows.Count > 0)
                {
                    string userId = dtFind.Rows[0]["UserID"].ToString();

                    // İstersen burada eksik alanları güncelleyebilirsin
                    string sqlUpdate = "";
                    sqlUpdate += "UPDATE Users SET ";
                    sqlUpdate += "NameSurname = '" + cleanAdSoyad.Replace("'", "''") + "', ";
                    sqlUpdate += "UserName = '" + cleanUser.Replace("'", "''") + "', ";
                    sqlUpdate += "Email = '" + cleanMail.Replace("'", "''") + "', ";
                    sqlUpdate += "EmployeeID = '" + cleanSicil.Replace("'", "''") + "', ";
                    sqlUpdate += "UserType = '" + cleanUserType.Replace("'", "''") + "', ";
                    sqlUpdate += "Title = '" + title + "',";
                    sqlUpdate += "Status = 1 ";
                    sqlUpdate += "WHERE UserID = " + userId;

                    db.ExecStr(sqlUpdate);

                    return userId;
                }

                // 2) JHR'den bölüm bilgisini çek
                string department = "";


                if (!string.IsNullOrWhiteSpace(cleanSicil))
                {
                    string sqlJhr = "";
                    sqlJhr += "SELECT TOP 1 ";
                    sqlJhr += "ISNULL(Staff.Bolum,'') AS Bolum ";
                    sqlJhr += "FROM JHR.dbo.Perlist AS Staff ";
                    sqlJhr += "WHERE Staff.SicilNo = '" + cleanSicil + "'";

                    DataTable dtJhr = db.SqlToDt(sqlJhr);

                    if (dtJhr.Rows.Count > 0)
                    {
                        department = dtJhr.Rows[0]["Bolum"].ToString();
                    }
                }

                // 3) Yoksa insert et
                string defaultPass = "Pass123";

                string sqlInsert = "";
                sqlInsert += "INSERT INTO Users ";
                sqlInsert += "(NameSurname, UserName, Email, Department, Title, Status, UserType, Pass, isDeleted, EmployeeID) ";
                sqlInsert += "VALUES (";
                sqlInsert += "'" + cleanAdSoyad.Replace("'", "''") + "', ";
                sqlInsert += "'" + cleanUser.Replace("'", "''") + "', ";
                sqlInsert += "'" + cleanMail.Replace("'", "''") + "', ";
                sqlInsert += "'" + ch.temizle(department).Replace("'", "''") + "', ";
                sqlInsert += "'" + ch.temizle(title).Replace("'", "''") + "', ";
                sqlInsert += "1, ";
                sqlInsert += "'" + cleanUserType.Replace("'", "''") + "', ";
                sqlInsert += "'" + defaultPass + "', ";
                sqlInsert += "NULL, ";
                sqlInsert += "'" + cleanSicil.Replace("'", "''") + "'";
                sqlInsert += "); ";
                sqlInsert += "SELECT SCOPE_IDENTITY() AS NewUserID;";

                DataTable dtInsert = db.SqlToDt(sqlInsert);

                if (dtInsert.Rows.Count > 0)
                    return dtInsert.Rows[0]["NewUserID"].ToString();

                return "0";
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Users senkronizasyon hatası: " + ex.Message;
                return "0";
            }
        }
    }
}