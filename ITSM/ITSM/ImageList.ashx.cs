using System;
using System.IO;
using System.Text;
using System.Web;

namespace ITSM
{
    public class ImageList : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            string folderPath = context.Server.MapPath("~/uploads/");

            if (!Directory.Exists(folderPath))
            {
                context.Response.Write("[]");
                return;
            }

            string[] files = Directory.GetFiles(folderPath);
            StringBuilder json = new StringBuilder();
            json.Append("[");

            for (int i = 0; i < files.Length; i++)
            {
                string fileName = Path.GetFileName(files[i]);
                string fileUrl = "/uploads/" + fileName;

                // TinyMCE'nin Resim Listesi için beklediği format: {"title": "Resim Adı", "value": "/yol/resim.jpg"}
                json.Append($"{{\"title\": \"{fileName}\", \"value\": \"{fileUrl}\"}}");

                if (i < files.Length - 1)
                {
                    json.Append(",");
                }
            }
            json.Append("]");

            context.Response.Write(json.ToString());
        }

        public bool IsReusable => false;
    }
}