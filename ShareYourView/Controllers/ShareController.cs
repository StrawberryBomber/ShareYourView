using ShareYourView.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        [Authorize]
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

        [Authorize]
        [HttpGet]
        public ActionResult viewDetails(GoogleDriveFile file)
        {
            ViewBag.Image = DetailsHelper.getSharedImage(file);

            //Set mutiple data views to View
            ImageMetadata imgData = getImgData(file);

            return View(imgData);
        }

        private ImageMetadata getImgData(GoogleDriveFile file)
        {
            ImageMetadata imgData = new ImageMetadata();

            string name = Path.GetFileNameWithoutExtension(file.Name) + "@";

            using (shareYourView_DBEntities db = new shareYourView_DBEntities())
            {
                var img = db.ImageDetails.Where(a => a.image_Name.Contains(name)).FirstOrDefault();

                var user = db.UserDetails.Where(a => a.user_Username == HttpContext.User.Identity.Name).FirstOrDefault();

                var share = db.ImageShareds.Where(a => a.image_ID == img.image_ID && a.user_ID == user.user_ID).FirstOrDefault();

                if(share != null)
                {
                    imgData = db.ImageMetadatas.Where(a => a.image_ID == share.image_ID).FirstOrDefault();


                    var sharedImage = db.ImageDetails.Where(a => a.image_ID == share.image_ID).FirstOrDefault();
                    var sharedUser = db.UserDetails.Where(a => a.user_ID == sharedImage.user_ID).FirstOrDefault();

                    imgData.sharedBy = sharedUser.user_FName + " " + sharedUser.user_LName + "  -  " + sharedUser.user_Email;

                    Debug.WriteLine("\n\n\n" + sharedUser.user_Username + "\n\n\n");
                }

            }

                //string name = Path.GetFileNameWithoutExtension(file.Name) + "@" + HttpContext.User.Identity.Name + Path.GetExtension(file.Name);

                //using (shareYourView_DBEntities db = new shareYourView_DBEntities())
                //{
                //    var img = db.ImageDetails.Where(a => a.image_Name == name).FirstOrDefault();

                //    if (img != null)
                //    {
                //        imgData = db.ImageMetadatas.Where(a => a.image_ID == img.image_ID).FirstOrDefault();
                //    }
                //}

                return imgData;
        }

        [HttpPost]
        public ActionResult returnToHome()
        {
            return RedirectToAction("Index", "Home");
        }

    }
}