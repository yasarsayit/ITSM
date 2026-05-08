using System;
using System.Configuration;
using System.Reflection;
using System.Web;
using System.Web.SessionState;

namespace CommonWeb
{
    public class SharedSessionModule : IHttpModule
    {
        protected static string applicationName = ConfigurationManager.AppSettings["ApplicationName"];
        protected static string rootDomain = ConfigurationManager.AppSettings["RootDomain"];

        public void Init(HttpApplication context)
        {
            if (string.IsNullOrEmpty(applicationName) || string.IsNullOrEmpty(rootDomain))
                return;

            FieldInfo runtimeInfo = typeof(HttpRuntime).GetField("_theRuntime", BindingFlags.Static | BindingFlags.NonPublic);
            HttpRuntime theRuntime = (HttpRuntime)runtimeInfo.GetValue(null);

            FieldInfo appNameInfo = typeof(HttpRuntime).GetField("_appDomainAppId", BindingFlags.Instance | BindingFlags.NonPublic);
            appNameInfo.SetValue(theRuntime, applicationName);

            context.PostRequestHandlerExecute += context_PostRequestHandlerExecute;
        }

        public void Dispose()
        {
        }

        private void context_PostRequestHandlerExecute(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;

            try
            {
                if (app == null || app.Context == null)
                    return;

                string path = app.Request.AppRelativeCurrentExecutionFilePath ?? "";

                // Statik dosyalarda hiç uğraşma
                if (path.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                    path.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                    path.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                    path.EndsWith(".gif", StringComparison.OrdinalIgnoreCase) ||
                    path.EndsWith(".svg", StringComparison.OrdinalIgnoreCase) ||
                    path.EndsWith(".css", StringComparison.OrdinalIgnoreCase) ||
                    path.EndsWith(".js", StringComparison.OrdinalIgnoreCase) ||
                    path.EndsWith(".woff", StringComparison.OrdinalIgnoreCase) ||
                    path.EndsWith(".woff2", StringComparison.OrdinalIgnoreCase) ||
                    path.EndsWith(".ttf", StringComparison.OrdinalIgnoreCase) ||
                    path.EndsWith(".eot", StringComparison.OrdinalIgnoreCase) ||
                    path.EndsWith(".ico", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                // Session burada yoksa hiç kurcalama
                HttpSessionState session = null;
                try
                {
                    session = app.Session;
                }
                catch
                {
                    return;
                }

                if (session == null || string.IsNullOrEmpty(session.SessionID))
                    return;

                HttpCookie cookie = app.Response.Cookies["ASP.NET_SessionId"];

                if (cookie == null)
                {
                    cookie = new HttpCookie("ASP.NET_SessionId");
                    app.Response.Cookies.Add(cookie);
                }

                cookie.Value = session.SessionID;

                if (!string.Equals(rootDomain, "", StringComparison.OrdinalIgnoreCase))
                    cookie.Domain = rootDomain;

                cookie.Path = "/";
            }
            catch
            {
                // burada sessiz geçiyoruz, çünkü modül yüzünden statik dosya patlamasın
            }
        }
    }
}