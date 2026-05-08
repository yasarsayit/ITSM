using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace ITSM
{
    public class TranslationTools
    {
        private string _hata = string.Empty;

        public string Hata
        {
            get { return _hata; }
            set { _hata = value; }
        }

        private readonly string ConnStr = ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;

        public readonly Dictionary<string, string> translations =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public string Translate(string tag, string lang = "EN")
        {
            string r = "";

            try
            {
                string columnName = "EN";

                if (!string.IsNullOrEmpty(lang))
                {
                    switch (lang.ToUpperInvariant())
                    {
                        case "TR":
                        case "EN":
                        case "RU":
                            columnName = lang.ToUpperInvariant();
                            break;
                    }
                }

                using (SqlConnection conn = new SqlConnection(ConnStr))
                using (SqlCommand cmd = new SqlCommand(
                    string.Format("SELECT ISNULL({0}, EN) AS lng FROM Lang WHERE Tag = @Tag", columnName), conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@Tag", SqlDbType.NVarChar, 100) { Value = tag });

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader.IsDBNull(0))
                                r = "0";
                            else
                                r = reader.GetString(0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                r = "";
                _hata = ex.Message;
            }

            return r;
        }

        public string GetText(string tag, string fallback, string langCode = "EN")
        {
            string cacheKey = (langCode ?? "EN").ToUpperInvariant() + "|" + tag;

            if (translations.ContainsKey(cacheKey))
                return translations[cacheKey];

            string value = Translate(tag, langCode);

            if (string.IsNullOrWhiteSpace(value) || value == "0")
                value = fallback;

            translations[cacheKey] = value;
            return value;
        }

        public string GetLangCode()
        {
            try
            {
                string uiLang = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToUpperInvariant();

                switch (uiLang)
                {
                    case "TR":
                    case "EN":
                    case "RU":
                        return uiLang;
                }

                var langs = HttpContext.Current?.Request?.UserLanguages;

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
                            case "RU":
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

    }


}