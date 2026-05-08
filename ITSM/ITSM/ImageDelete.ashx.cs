using System;
using System.IO;
using System.Web;

namespace ITSM
{
    public class ImageDelete : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            string fileUrl = context.Request.QueryString["fileUrl"];

            if (string.IsNullOrEmpty(fileUrl))
            {
                context.Response.Write("{\"success\": false, \"message\": \"File path not found.\"}");
                return;
            }

            try
            {
                string filePath = context.Server.MapPath("~" + fileUrl);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    context.Response.Write("{\"success\": true}");
                }
                else
                {
                    context.Response.Write("{\"success\": false, \"message\": \"The file does not exist in the system.\"}");
                }
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                context.Response.Write("{\"success\": false, \"message\": \"" + ex.Message + "\"}");
            }
        }

        public bool IsReusable => false;
    }
}