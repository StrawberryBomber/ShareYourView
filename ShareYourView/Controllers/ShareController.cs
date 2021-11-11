using ShareYourView.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShareYourView.Controllers
{
    public class ShareController : Controller
    {
        // GET: Share
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetGoogleShareDriveFiles()
        {
            return View(GoogleDriveAPIHelper.GetShareDriveFiles());
        }

        public void DownloadFile(string id)
        {
            string FilePath = GoogleDriveAPIHelper.DownloadGoogleFile(id);

            Response.ContentType = "application/zip";
            Response.AddHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(FilePath));
            Response.WriteFile(System.Web.HttpContext.Current.Server.MapPath("~/GoogleDriveFiles/" + Path.GetFileName(FilePath)));
            Response.End();
            Response.Flush();
        }

        [HttpPost]
        public ActionResult returnToHome()
        {
            return RedirectToAction("Index", "Home");
        }

    }
}