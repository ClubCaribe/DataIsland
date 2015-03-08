using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace DataIsland.Areas.filemanager.CustomActions
{
    public class ZipDownloadResult : ActionResult
    {
        
        public ZipDownloadResult() { }


        public ZipDownloadResult(string filename)
        {
            this.PhysicalPath = filename;
        }

        public string PhysicalPath { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "application/octet-stream";
            context.HttpContext.Response.AddHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(this.PhysicalPath));
            context.HttpContext.Response.BufferOutput = false;
            context.HttpContext.Response.Buffer = false;
            

            byte[] buffer = new byte[100000];
            using (FileStream fs = new FileStream(this.PhysicalPath, FileMode.Open, FileAccess.Read))
            {
                //context.HttpContext.Response.AddHeader("Content-Length", fs.Length.ToString());
                int bytesRead = 0;
                while ((bytesRead = fs.Read(buffer, 0, 100000)) > 0)
                {
                    context.HttpContext.Response.OutputStream.Write(buffer, 0, bytesRead);
                    context.HttpContext.Response.Flush();
                }
                fs.Close();
            }

            context.HttpContext.Response.Close();
        }
    }
}