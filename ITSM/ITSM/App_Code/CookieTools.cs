using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace ITSM
{
    public class CookieTools
    {
        CrpTools CrpTool = new CrpTools();

        public void Yaz(string ckname, string ckval)
        {
            if (HttpContext.Current.Request.Cookies[ckname] == null)
            {
                HttpCookie ck = new HttpCookie(ckname);
                ck.Value = CrpTool.encrypt(ckval);
                ck.Expires = DateTime.Now.AddDays(30);
                HttpContext.Current.Response.Cookies.Add(ck);
            }
            else
            {
                HttpCookie ck = HttpContext.Current.Request.Cookies[ckname];
                ck.Value = CrpTool.encrypt(ckval);
                ck.Expires = DateTime.Now.AddDays(30);
                HttpContext.Current.Response.Cookies.Set(ck);
            }
        }

        public string Oku(string ckname)
        {
            string Val = "";
            if (HttpContext.Current.Request.Cookies[ckname] != null)
            {
                HttpCookie ck = HttpContext.Current.Request.Cookies[ckname];
                try
                {
                    Val = CrpTool.Decrypt(ck.Value);
                }
                catch
                {
                    Val = "";
                }
            }
            else
                return "";
            return Val;
        }

        public Boolean Sil(string ckname)
        {
            Boolean sonuc = false;
            try
            {
                if (HttpContext.Current.Request.Cookies[ckname] != null)
                {
                    HttpCookie ck = HttpContext.Current.Request.Cookies[ckname];
                    //ck.Expires=DateTime.Now.AddDays(-1);
                    var expiredCookie = new HttpCookie(ckname)
                    {
                        Expires = DateTime.Now.AddDays(-1),
                        Domain = ck.Domain
                    };
                    HttpContext.Current.Response.Cookies.Add(expiredCookie); // overwrite it

                    sonuc = true;
                }
            }
            catch (Exception)
            {
                sonuc = false;

            }
            return sonuc;
        }

        public void MultiYaz(string CkName, DataTable dt)
        {
            DataTable dt_ck = MultiOku(CkName);
            if (dt_ck.Rows.Count > 0)
            {

                for (var i = 0; i <= dt.Rows.Count - 1; i++)
                {
                    DataRow[] FRow = dt_ck.Select("CkName='" + dt.Rows[i]["CkName"] + "'");

                    if (FRow.Count() > 0)
                    {
                        // ESKİ KAYIT MEVCUT
                        // KAÇ TANE KAYIT BULDUYSAN HEPSİNİ GÜNCELLE !!!!!
                        foreach (DataRow row in FRow)
                        {
                            if (dt.Rows[i]["CkVal"] != DBNull.Value)
                                row["CkVal"] = dt.Rows[i]["CkVal"].ToString();
                        }
                    }
                    else
                        // KAYIT BULUNAMADI
                        if (dt.Rows[i]["CkName"] != DBNull.Value & dt.Rows[i]["CkVal"] != DBNull.Value)
                        dt_ck.Rows.Add(dt.Rows[i]["CkName"].ToString(), dt.Rows[i]["CkVal"].ToString());

                    dt_ck.AcceptChanges();
                }
            }

            if (HttpContext.Current.Request.Cookies[CkName] == null)
            {
                HttpCookie ck = new HttpCookie(CkName);
                for (var i = 0; i <= dt.Rows.Count - 1; i++)
                    ck.Values.Add(dt.Rows[i]["CkName"].ToString(), dt.Rows[i]["CkVal"].ToString());
                ck.Expires = DateTime.Now.AddDays(GetSesSure());
                ck.Value = CrpTool.encrypt(ck.Value);
                HttpContext.Current.Response.Cookies.Add(ck);
            }
            else
            {
                HttpCookie ck = new HttpCookie(CkName);
                for (var i = 0; i <= dt_ck.Rows.Count - 1; i++)
                    ck.Values.Add(dt_ck.Rows[i]["CkName"].ToString(), dt_ck.Rows[i]["CkVal"].ToString());
                ck.Expires = DateTime.Now.AddDays(GetSesSure());
                ck.Value = CrpTool.encrypt(ck.Value);

                HttpContext.Current.Response.SetCookie(ck);
                HttpContext.Current.Response.Cookies.Set(ck);
                HttpContext.Current.Request.Cookies.Set(ck);
                HttpContext.Current.Response.Cookies.Set(ck);
            }

            dt.Dispose();
        }

        public DataTable MultiOku(string ckname)
        {
            DataTable dt = new DataTable();
            try
            {
                dt.Columns.Add("CkName");
                dt.Columns.Add("CkVal");

                if (HttpContext.Current.Request.Cookies[ckname] != null)
                {
                    HttpCookie ck2 = HttpContext.Current.Request.Cookies[ckname];
                    //HttpCookie f = HttpContext.Current.Response.Cookies[ckname];


                    // Dim val1 As String = ck.Value
                    string val2 = ck2.Value;
                    // val2 = NormalizeUTF8TurkishCharacters(val2);

                    // If ck Is Nothing Then
                    // ck = HttpContext.Current.Request.Cookies[CkName]
                    // End If

                    if (ck2.Value != null && ck2.Value.ToString().Length > 0)
                    {
                        string CKTOKEN = CrpTool.Decrypt(val2);
                        string phrase = CKTOKEN;
                        string ITMPARSE = "&";
                        char[] ITM_chars = ITMPARSE.ToCharArray();
                        string[] ITM_STR = phrase.Split(ITM_chars);

                        string VALPARSE = "=";

                        for (int i = 0; i <= ITM_STR.Count() - 1; i++)
                        {
                            char[] VAL_chars = VALPARSE.ToCharArray();
                            string[] VAL_STR = ITM_STR[i].Split(VAL_chars);
                            dt.Rows.Add(VAL_STR[0], VAL_STR[1]);
                        }

                        dt.AcceptChanges();
                    }
                }

            }
            catch (Exception ex)
            {
                string errmsg = ex.ToString();
                throw;
            }

            return dt;
        }

        public string multione(string field, string cookie)
        {
            //Cookies ck = new CkieTools();
            DataTable sepet = MultiOku(cookie);
            string item = "";
            if (sepet.Select("CkName = '" + field + "'").Any())
            {
                DataRow[] dr = sepet.Select("CkName = '" + field + "'");
                item = dr[0]["CkVal"].ToString();
            }

            return item;
        }

        public void ckaddrow(string ckname, string par, string val)
        {
            DataTable dt = MultiOku(ckname);
            dt.Rows.Add(par, val);
            MultiYaz(ckname, dt);
            dt.Dispose();
        }

        public void refresh(string ckname)
        {
            DataTable dt = MultiOku(ckname);
            MultiYaz(ckname, dt);
            Yaz(ckname, Oku(ckname));
        }

        public static string NormalizeUTF8TurkishCharacters(string str)
        {
            string txt = str;
            var utfCh = new List<string>() { "Ä±", "Ã§", "ÅŸ", "Ã¶", "Ã¼", "ÄŸ", "Ä°", "Ã‡", "ÅŸ", "Å", "Ã–", "Ãœ", "ÄŸ", "Ä" };
            var utfChTR = new List<string>() { "ı", "ç", "ş", "ö", "ü", "ğ", "İ", "Ç", "Ş", "Ş", "Ö", "Ü", "Ğ", "Ğ" };

            for (int i = 0, loopTo = utfCh.Count - 1; i <= loopTo; i++)
                txt = txt.Replace(utfCh[i], utfChTR[i]);

            return txt;
        }

        public Boolean Logout()
        {
            Boolean sonuc = false;
            try
            {
                if (HttpContext.Current != null)
                {

                    // Clear Cookies Client Side
                    int cookieCount = HttpContext.Current.Request.Cookies.Count;
                    for (var i = 0; i < cookieCount; i++)
                    {
                        var cookie = HttpContext.Current.Request.Cookies[i];
                        if (cookie != null)
                        {
                            var expiredCookie = new HttpCookie(cookie.Name)
                            {
                                Expires = DateTime.Now.AddDays(-1),
                                Domain = cookie.Domain
                            };
                            HttpContext.Current.Response.Cookies.Add(expiredCookie); // overwrite it
                        }
                    }

                    // Clear Cookies Server Side
                    HttpContext.Current.Request.Cookies.Clear();

                    //Sessions Clear
                    HttpContext.Current.Session.RemoveAll();
                    //HttpContext.Current.Session.Clear();
                    //HttpContext.Current.Session.Abandon();

                }

                sonuc = true;
            }
            catch (Exception)
            {
                sonuc = false;

            }
            return sonuc;
        }

        public int GetSesSure()
        {
            int sure = 365;
            return sure;
        }
        public void ClearAuthCookies()
        {
            Sil("User");
            Sil("UserType");
            Sil("namesurname");
            Sil("useremail");
            Sil("Password");
        }
    }
}