using ShareYourView.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShareYourView.Controllers
{
    public class ImageController : Controller
    {
        // GET: Image
        public ActionResult Index()
        {
            return View();
        }

        //[HttpPost]
        //public ActionResult Index(HttpPostedFileBase file)
        //{
        //    GoogleDriveAPIHelper.UplaodFileOnDrive(file);
        //    ViewBag.Success = "File Uploaded on Google Drive";
        //    //GoogleDriveAPIHelper.GetDriveFiles();
        //    return View();
        //}

        [HttpGet]
        public ActionResult GetGoogleDriveFiles()
        {
            return View(GoogleDriveAPIHelper.GetDriveFiles());
        }

        [HttpPost]
        public ActionResult DeleteFile(GoogleDriveFile file)
        {
            GoogleDriveAPIHelper.DeleteFile(file);
            return RedirectToAction("GetGoogleDriveFiles");
        }

        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            GoogleDriveAPIHelper.UplaodFileOnDrive(file);
            ViewBag.Success = "File Uploaded on Google Drive";
            return RedirectToAction("GetGoogleDriveFiles");
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