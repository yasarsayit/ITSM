using System;
using System.IO;
using System.Web;

namespace ITSM
{
    public class ImageUpload : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";

            try
            {
                if (context.Request.Files.Count > 0)
                {
                    HttpPostedFile file = context.Request.Files[0];

                    // Dosyanın asıl ismini ve uzantısını alıyoruz
                    string originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
                    string extension = Path.GetExtension(file.FileName);
                    string fileName = originalFileName + extension;

                    string folderPath = context.Server.MapPath("~/Uploads/");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    string savePath = Path.Combine(folderPath, fileName);

                    // EĞER AYNI İSİMDE DOSYA VARSA, SONUNA _1, _2 GİBİ SAYILAR EKLE
                    int counter = 1;
                    while (File.Exists(savePath))
                    {
                        fileName = originalFileName + "_" + counter + extension;
                        savePath = Path.Combine(folderPath, fileName);
                        counter++;
                    }

                    // Dosyayı temiz ismiyle klasöre kaydet
                    file.SaveAs(savePath);

                    string fileUrl = "/Uploads/" + fileName;
                    context.Response.Write("{\"location\": \"" + fileUrl + "\"}");
                }
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                context.Response.Write("{\"error\": \"" + ex.Message + "\"}");
            }
        }

        public bool IsReusable => false;
    }
}