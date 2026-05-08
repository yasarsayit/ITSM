using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
 


namespace ITSM
{
    public class DBTools
    {

        CookieTools ck = new CookieTools();
        CrpTools Crypt = new CrpTools();
        LogTools LogTool = new LogTools();

        SqlConnection dbCon = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString);

        string _Hata;
        public string Hata
        {
            get
            {
                return _Hata;
            }
            set
            {
                _Hata = value;
            }
        }

        public SqlConnection Con()
        {
            //SqlConnection dbCon = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString);
            return dbCon;
        }


        public DataTable SqlToDt(string sql)
        {
            DataTable dt = new DataTable();

            try
            {
                Con().Open();
                using (SqlDataAdapter dad = new SqlDataAdapter(sql, Con()))// köprü görevi görür
                {
                    dad.Fill(dt);
                }
                Con().Close();
                return dt;
            }
            catch (Exception ex)
            {
                _Hata = ex.Message;
                Con().Close();

                LogTool.TxtLogYaz("ERR. SqlToDt : " + sql + Environment.NewLine + Environment.NewLine + "MSG: " + ex.Message);
                return dt;
            }

        }

        public void Sql2AddCombo(DropDownList combo, string sql, string SelItemVal, string TextField, string ValField, bool SelFirst = false, string bosmsg = "")
        {
            try
            {
                combo.Items.Clear();

                SqlCommand komut = new SqlCommand(sql, Con());

                Con().Open();
                SqlDataReader oku;
                oku = komut.ExecuteReader();

                if (bosmsg != "")
                {
                    ListItem Itemim1 = new ListItem();
                    Itemim1.Text = bosmsg;
                    Itemim1.Value = "";
                    combo.Items.Add(Itemim1);
                }

                if (oku.HasRows)
                {
                    while (oku.Read())
                    {
                        if (oku[TextField].ToString() != "")
                        {
                            ListItem Itemim = new ListItem();
                            Itemim.Text = oku[TextField].ToString().ToUpper();
                            Itemim.Value = oku[ValField].ToString().ToUpper();
                            if (SelItemVal != "")
                            {
                                if (oku[ValField].ToString() == SelItemVal)
                                {
                                    Itemim.Selected = true;
                                }

                            }
                            combo.Items.Add(Itemim);
                        }
                    }

                    if (SelFirst == true)
                    {
                        combo.SelectedIndex = 0;
                    }
                }

                oku.Close();
                Con().Close();

            }
            catch (Exception ex)
            {
                _Hata = ex.Message;
                Con().Close();

                LogTool.TxtLogYaz("ERR. Sql2AddCombo : " + sql + Environment.NewLine + Environment.NewLine + "MSG: " + ex.Message);
            }
        }

        public int ExecStr(string sql)
        {
            int don;
            try
            {
                SqlCommand komut = new SqlCommand(sql, Con());
                Con().Open();
                SqlCommand cmdf = Con().CreateCommand();
                cmdf.Connection = Con();
                cmdf.CommandText = sql;
                don = cmdf.ExecuteNonQuery();
                Con().Close();
            }
            catch (Exception ex)
            {
                don = -1;
                _Hata = ex.Message;
                Con().Close();

                LogTool.TxtLogYaz("ERR. ExecStr : " + sql + Environment.NewLine + Environment.NewLine + "MSG: " + ex.Message);
            }
            return don;
        }


        public DataTable GetDataTableWithParams(string sql, Dictionary<string, object> prms)
        {
            DataTable dt = new DataTable();

            using (dbCon)
            {
                try
                {
                    using (SqlCommand cmd = new SqlCommand(sql, dbCon))
                    {
                        if (prms != null)
                        {
                            foreach (var kvp in prms)
                            {
                                cmd.Parameters.AddWithValue(kvp.Key, kvp.Value);
                            }
                        }

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _Hata = ex.Message;
                }
            }

            return dt;
        }

        public bool ExecStrWithParams(string sql, Dictionary<string, object> prms)
        {
            bool success = false;

            using (dbCon)
            {
                try
                {
                    dbCon.Open();

                    using (SqlCommand cmd = new SqlCommand(sql, dbCon))
                    {
                        if (prms != null)
                        {
                            foreach (var kvp in prms)
                            {
                                cmd.Parameters.AddWithValue(kvp.Key, kvp.Value);
                            }
                        }

                        cmd.ExecuteNonQuery();
                        success = true;
                    }
                }
                catch (Exception ex)
                {
                    _Hata = ex.Message;
                    success = false;
                }
            }

            return success;
        }

        public int ExecInsertAndReturnId(string sql, Dictionary<string, object> prms)
        {
            int insertedId = 0;

            using (dbCon)
            {
                try
                {
                    dbCon.Open();

                    sql += "; SELECT SCOPE_IDENTITY();";

                    using (SqlCommand cmd = new SqlCommand(sql, dbCon))
                    {
                        if (prms != null)
                        {
                            foreach (var kvp in prms)
                            {
                                cmd.Parameters.AddWithValue(kvp.Key, kvp.Value);
                            }
                        }

                        var result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            insertedId = Convert.ToInt32(result);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _Hata = ex.Message;
                    insertedId = 0;
                }
            }

            return insertedId;
        }

 

    }

}