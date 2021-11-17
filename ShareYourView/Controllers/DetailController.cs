
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
            
            _DetailFile = file;

            //Set the image to view
            ViewBag.Image = DetailsHelper.getImage(file);
            

            //Set mutiple data views to View
            ImageMetadata imgData = getImgData(file);
            List<UserDetail> userData = getUserData(file);

            imageSharedView shareView = new imageSharedView();
            shareView.imgData = imgData;
            shareView.userData = userData;


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
            }

                return RedirectToAction("DisplayView", "Detail", _DetailFile);
        }

        [HttpPost]
        public ActionResult addNewShareUser(string email, int fileId)
        {
            //add user to shared with the image
            using(shareYourView_DBEntities db = new shareYourView_DBEntities())
            {
                var x = db.ImageDetails.Where(a => a.image_ID == fileId).FirstOrDefault();
                var y = db.UserDetails.Where(a => a.user_Email == email).FirstOrDefault();


                if (x != null && y != null)
                {
                    ImageShared imgShare = new ImageShared();
                    imgShare.image_ID = x.image_ID;
                    imgShare.user_ID = y.user_ID;

                    db.ImageShareds.Add(imgShare);
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Image was shared successfully";
                }else
                {
                    TempData["ErrorMessage"] = "Email Address could not be found. Please make sure you entered it correctly, or ask the user to sign up to share Images";
                }
            }

            return RedirectToAction("DisplayView", "Detail", _DetailFile);
        }

        [HttpGet]
        public ActionResult updateMetaDataDetails(int fileId)
        {
            ImageMetadata data = new ImageMetadata();

            using (shareYourView_DBEntities db = new shareYourView_DBEntities())
            {
                data = db.ImageMetadatas.Where(a => a.image_ID == fileId).FirstOrDefault();
            }

            return View(data);
        }

        [HttpPost]
        public ActionResult updateMetaDataDetails(ImageMetadata imgData, int fileId)
        {

            using (shareYourView_DBEntities db = new shareYourView_DBEntities())
            {
                var x = db.ImageDetails.Where(a => a.image_ID == fileId).FirstOrDefault();

                if(x != null)
                {
                    ImageMetadata dataToUpdate = db.ImageMetadatas.Where(a => a.image_ID == x.image_ID).FirstOrDefault();

                    dataToUpdate.Longitude = imgData.Longitude;
                    dataToUpdate.Latitude = imgData.Latitude;
                    dataToUpdate.City = imgData.City;
                    dataToUpdate.Address = imgData.Address;
                    dataToUpdate.capturedBy = imgData.capturedBy;
                    dataToUpdate.capturedData = imgData.capturedData;

                    db.SaveChanges();
                    TempData["UpdatedSuccess"] = "The data has been updated successfully.";
                }

            }
                       

            return View(imgData);
        }

        [HttpPost]
        public ActionResult navigateToView(int fileId)
        {
            GoogleDriveFile file = new GoogleDriveFile();

            using (shareYourView_DBEntities db = new shareYourView_DBEntities())
            {
                var x = db.ImageDetails.Where(a => a.image_ID == fileId).FirstOrDefault();

                file = GoogleDriveAPIHelper.getSingleDriveFile(x.image_Name);
            }

            return RedirectToAction("DisplayView", "Detail", file);
        }
    }
}