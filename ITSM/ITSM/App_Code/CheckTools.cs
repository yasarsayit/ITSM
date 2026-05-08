using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using System.Dynamic;
using System.IO;

namespace ITSM
{
    public class CheckTools
    {

        CookieTools ck = new CookieTools();
        DBTools DBTool = new DBTools();
        CrpTools Crypt = new CrpTools();
        //ParamTools ParamTool = new ParamTools();

        public bool IsInteger(string strTmp)
        {
            System.Text.RegularExpressions.Regex objIntPattern = new System.Text.RegularExpressions.Regex("^[0-9]+[0-9]*$");
            return objIntPattern.IsMatch(strTmp);
        }

        public bool IsNumeric(string strTmp)
        {
            System.Text.RegularExpressions.Regex objIntPattern = new System.Text.RegularExpressions.Regex(@"^-?[0-9][0-9,\.]+$");
            bool val = objIntPattern.IsMatch(strTmp);
            if (val == true || IsInteger(strTmp))
            {
                val = true;
            }
            return val;
        }

        public int LoginCheck(string
           USRNAME, string PASS)
        {
            USRNAME = temizle(USRNAME);
            PASS = temizle(PASS);

            SqlCommand komut;
            string sql;

            sql = "SELECT ID,USR,PASS,SATLEVEL,KADEMELEVEL FROM KULLANICILAR WHERE USR=@USR AND PASS=@PASS AND AKTIF=1";

            SqlConnection dbCon = DBTool.Con();
            dbCon.Open();
            komut = new SqlCommand(sql, dbCon);
            komut.Parameters.AddWithValue("USR", USRNAME);
            komut.Parameters.AddWithValue("PASS", PASS);

            SqlDataReader reader = komut.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(reader);
            reader.Close();
            dbCon.Close();

            int USERID = 0;
            if (dt.Rows.Count > 0)
            {

                HttpContext.Current.Session["SatLevel"] = dt.Rows[0]["SATLEVEL"].ToString();
                HttpContext.Current.Session["KademeUserLevel"] = dt.Rows[0]["KADEMELEVEL"].ToString();
                int.TryParse(dt.Rows[0]["ID"].ToString(), out USERID);
                dt.Dispose();
                return USERID;
            }
            dt.Dispose();
            return USERID;
        }

        public DataTable FindValInTables(string ColumnName, string Val)
        {
            string sql = "EXEC KayitKontrol '" + ColumnName + "','" + Val + "'";
            DataTable dt = DBTool.SqlToDt(sql);

            return dt;
        }


        public static string BuildFullTextSearch(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            var ignoredWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "ve", "veya", "ile", "ama", "fakat", "icin", "için",
                    "bir", "bu", "şu", "su", "çok", "cok", "az", "gibi",
                    "daha", "talep", "sorun", "problem, şey, talebi"
                    ,"gerek, bekliyorum, kendi, acil, rica, ediyorum",
                    ", saygılarımla, saygilarimla"
                };

            var words = input
                .Split(new[] { ' ', ',', '.', ';', ':', '\t', '\r', '\n', '-', '/', '\\', '(', ')', '[', ']' },
                       StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => x.Length > 2 && !ignoredWords.Contains(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (words.Count == 0)
                return string.Empty;

            return string.Join(" OR ", words.Select(x => "\"" + x.Replace("\"", "") + "*\""));
        }


        public string temizle(string kelime)
        {
            kelime = kelime.Replace(">", "");
            kelime = kelime.Replace("<", "");
            kelime = kelime.Replace("%", "");
            kelime = kelime.Replace("*", "");
            kelime = kelime.Replace("'", "");
            kelime = kelime.Replace(";", "");
            kelime = kelime.Replace("from", "");
            kelime = kelime.Replace("delete", "");
            kelime = kelime.Replace("update", "");
            kelime = kelime.Replace("drop", "");
            kelime = kelime.Replace("insert", "");
            kelime = kelime.Replace("select", "");
            kelime = kelime.Replace("$", "");
            kelime = kelime.Replace("session", "");

            return (kelime);
        }
        public bool IsDate(String date)
        {
            try
            {
                DateTime dt = DateTime.Parse(date);
                int MinYear = 0;
                int.TryParse(System.Configuration.ConfigurationManager.AppSettings["MINYEAR"], out MinYear);
                if (dt.Year < MinYear)
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public String FormatDatetime(String date)
        {
            try
            {
                if (date == "") return "NULL";

                DateTime dt = DateTime.Now;
                DateTime.TryParse(date, out dt);
                return "'" + dt.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'";
            }
            catch //(Exception ex)
            {
                //return ex.Message;
                return "NULL";
            }
        }
        public DateTime AyinBaslangici(DateTime tarih)
        {
            return new DateTime(tarih.Year, tarih.Month, 1);
        }
        public DateTime AyinBitisi(DateTime tarih)
        {
            tarih = new DateTime(tarih.Year, tarih.Month, 1);
            return tarih.AddMonths(1).AddDays(-1);
        }



        public string ChkComboDBNull(DropDownList combo)
        {
            if (combo.SelectedValue == "")
            {
                return "NULL";
            }
            return combo.SelectedValue.ToString();
        }

        public string ChkTxtDBNull(TextBox txt)
        {
            if (temizle(txt.Text) == "")
            {
                return "NULL";
            }
            return "'" + temizle(txt.Text) + "'";
        }

        public decimal ChkDecimalDBNull(TextBox txt)
        {
            decimal num = 0;
            decimal.TryParse(txt.Text, out num);
            return num;
        }



        public string ClearNull(string str)
        {
            return str.Replace(" ", "");
        }


        public bool UsrAuthCokie()
        {
            bool stat = false;

            DataTable dt = ck.MultiOku("TKN");
            if (dt.Rows.Count > 0)
            {
                string usr = "";
                string pass = "";

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    switch (dt.Rows[i]["CkName"].ToString())
                    {
                        case "usr":
                            usr = dt.Rows[i]["CkVal"].ToString();
                            break;

                        case "pass":
                            pass = dt.Rows[i]["CkVal"].ToString();
                            break;
                    }
                }

                if (dt.Rows.Count > 0)
                {
                    //GirisYap(usr, pass, 0);
                    int PersonID = LoginCheck(usr, pass);
                    if (PersonID > 0)
                    {
                        HttpContext.Current.Session["PersonID"] = PersonID;
                        stat = true;

                        //DataTable dtt = CKTool.ModulYetki(Session["PersonID"].ToString());
                        //if (dtt.Rows.Count > 0)
                        //{
                        //    Response.Redirect("Default.aspx", false);
                        //}
                    }
                }
                //GirisYap(dt.Rows[0]["usr"].ToString(), dt.Rows[0]["pass"].ToString());
            }

            return stat;
        }

        public int HarfSayac(char harf, string cumle)
        {
            int sayac = 0;
            for (int i = 0; i < cumle.Length; i++)
            {
                if (cumle[i] == harf)
                {
                    sayac++;
                }
            }
            return sayac;
        }

        public double Get_FolderSize()
        {
            string folderPath = AppDomain.CurrentDomain.BaseDirectory + "/dosyalar";
            long folderSize = GetFolderSize(folderPath);
            double folderSizeInMB = folderSize / (1024d * 1024d);

            return folderSizeInMB;
        }

        //public int Get_MaxDiskSize()
        //{
        //    int MaxDiskSize = 0;
        //    int.TryParse(ParamTool.ReadParam("MaxDiskSize"), out MaxDiskSize);

        //    return MaxDiskSize;
        //}

        public static long GetFolderSize(string folderPath)
        {
            long folderSize = Directory.GetFiles(folderPath).Sum(file => new FileInfo(file).Length);

            string[] subDirectories = Directory.GetDirectories(folderPath);

            foreach (string subDir in subDirectories)
            {
                folderSize += GetFolderSize(subDir);
            }

            return folderSize;
        }


        public bool KabulEdilenUzanti(string extension)
        {
            string[] allowedExtensions = { ".jpeg", ".jpg", ".png", ".docx", ".pdf", ".txt", ".exe", ".xlsx" };
            return Array.Exists(allowedExtensions, x => x.Equals(extension.ToLower()));
        }

        //public bool GetDiskKapasite(double UploadFile)
        //{
        //    int MaxDiskSize = Get_MaxDiskSize();
        //    double folderSizeInMB = Get_FolderSize();
        //    double kalan = MaxDiskSize - (folderSizeInMB + UploadFile);
        //    bool sonuc = true;
        //    if (kalan <= 0)
        //    {
        //        sonuc = false;
        //    }

        //    return sonuc;
        //}

    }
}