using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace ITSM
{
    public class LogTools
    {
        //DBTools DBTool = new DBTools();

        public string TxtLogYaz(string yazi)
        {
            try
            {
                string PersonID = "";
                string ADSOYAD = "";
                if (HttpContext.Current.Session["ADSOYAD"] != null)
                {
                    ADSOYAD = HttpContext.Current.Session["ADSOYAD"].ToString();
                }

                if (HttpContext.Current.Session["PersonID"] != null)
                {
                    PersonID = HttpContext.Current.Session["PersonID"].ToString();
                }

                string path_str = HttpContext.Current.Server.MapPath("~/logs/");
                using (var w = File.AppendText(path_str + DateTime.Today.ToString("ddMMyyyy") + ".txt"))
                {
                    w.WriteLine("[" + DateTime.Now.ToString() + "] :  " + yazi);
                    w.WriteLine("[PersonID] :  " + PersonID);
                    w.WriteLine("[ADSOYAD] :  " + ADSOYAD);
                    w.WriteLine(yazi);

                    w.Close();
                    w.Dispose();
                }
                yazi = null;
            }
            catch (IOException ex)
            {
                yazi = null;
                return ex.Message.ToString();
            }

            Collect();
            return "1";
        }

        public string DbLogYaz(int tip, string yazi)
        {
            try
            {
                string sql = "INSERT INTO LOGS VALUES (" + Environment.NewLine;
                sql += "GETDATE()" + Environment.NewLine;
                sql += ",1" + Environment.NewLine;
                sql += ",'" + HttpContext.Current.Request.ServerVariables["REMOTE_HOST"] + "'" + Environment.NewLine;
                sql += "," + HttpContext.Current.Session["PersonID"].ToString() + "" + Environment.NewLine;
                sql += ",'" + yazi;
                DBTools DBTool = new DBTools();
                int stat = DBTool.ExecStr(sql);
                if (stat > 0)
                {
                    //BAŞARILI DÖNÜŞ
                    Collect();
                    return "1";
                }
                else
                {
                    //DB HATA DURUMU
                    Collect();
                    return DBTool.Hata;
                }

            }
            catch (Exception ex)
            {
                //GENEL HATA DURUMU
                yazi = null;
                TxtLogYaz(ex.ToString());
                return ex.Message.ToString();
            }
        }

        private void Collect()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

    }
}