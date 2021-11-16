
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShareYourView.Models;

namespace ShareYourView.Controllers
{
    public class DetailController : Controller
    {
        private static GoogleDriveFile _DetailFile { get; set; }  

        // GET: Detail
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult DisplayView(GoogleDriveFile file)
        {
            //ViewBag.Image = DetailsHelper.getImage(file);
            //ImageMetadata imgData = new ImageMetadata();

            //string name = Path.GetFileNameWithoutExtension(file.Name) + "@" + HttpContext.User.Identity.Name + Path.GetExtension(file.Name);

            //using (shareYourView_DBEntities db = new shareYourView_DBEntities())
            //{                
            //    var img = db.ImageDetails.Where(a => a.image_Name == name).FirstOrDefault();

            //    if(img != null)
            //    {
            //        imgData = db.ImageMetadatas.Where(a => a.image_ID == img.image_ID).FirstOrDefault();
            //        //Debug.WriteLine("\n\n\n\n\n" + imgData.Address);
            //    }                
            //}
            _DetailFile = file;
            //Set the image to view
            ViewBag.Image = DetailsHelper.getImage(file);
            

            //Set mutiple data views to View
            ImageMetadata imgData = getImgData(file);
            List<UserDetail> userData = getUserData(file);

            imageSharedView shareView = new imageSharedView();
            shareView.imgData = imgData;
            shareView.userData = userData;

            //Debug.WriteLine("\n\n\n\n\n" + ViewBag.Image + "\n\n\n\n\n\n");

            return View(shareView);
        }

        private ImageMetadata getImgData(GoogleDriveFile file)
        {
            ImageMetadata imgData = new ImageMetadata();

            string name = Path.GetFileNameWithoutExtension(file.Name) + "@" + HttpContext.User.Identity.Name + Path.GetExtension(file.Name);

            using (shareYourView_DBEntities db = new shareYourView_DBEntities())
            {
                var img = db.ImageDetails.Where(a => a.image_Name == name).FirstOrDefault();

                if (img != null)
                {
                    imgData = db.ImageMetadatas.Where(a => a.image_ID == img.image_ID).FirstOrDefault();
                    //Debug.WriteLine("\n\n\n\n\n" + imgData.Address);
                }
            }

            return imgData;
        }

        private List<UserDetail> getUserData(GoogleDriveFile file)
        {
            List<UserDetail> listUser = new List<UserDetail>();
            string name = Path.GetFileNameWithoutExtension(file.Name) + "@" + HttpContext.User.Identity.Name + Path.GetExtension(file.Name);

            using (shareYourView_DBEntities db = new shareYourView_DBEntities())
            {
                var img = db.ImageDetails.Where(a => a.image_Name == name).FirstOrDefault();
                int imgID = img.image_ID;

                List<ImageShared> listShare = db.ImageShareds.Where(a => a.image_ID == imgID).ToList();

                foreach(var c in listShare)
                {
                    if(c != null)
                    {
                        var x = db.UserDetails.Where(a => a.user_ID == c.user_ID).FirstOrDefault();
                        listUser.Add(x);
                    }
                }

            }

            return listUser;
        }

        public ActionResult Unshare(int userId, int fileId)
        {
            using (shareYourView_DBEntities db = new shareYourView_DBEntities())
            {
                var x = db.ImageShareds.Where(a => a.image_ID == fileId && a.user_ID == userId).FirstOrDefault();

                db.ImageShareds.Remove(x);
                db.SaveChanges();
                //Debug.Write("\n\n\n\n\n" + x.user_ID + "=" + x.image_ID);
            }

                return RedirectToAction("DisplayView", "Detail", _DetailFile);
        }

        public ActionResult DisplayImageInfo()
        {

            return View();
        }
    }
}