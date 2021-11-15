
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShareYourView.Models;

namespace ShareYourView.Controllers
{
    public class DetailController : Controller
    {
        // GET: Detail
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult DisplayView(GoogleDriveFile file)
        {
            ViewBag.Image = DetailsHelper.getImage(file);
            ImageMetadata imgData = new ImageMetadata();
            string name = Path.GetFileNameWithoutExtension(file.Name) + "@" + HttpContext.User.Identity.Name + Path.GetExtension(file.Name);
            using (shareYourView_DBEntities db = new shareYourView_DBEntities())
            {                
                var img = db.ImageDetails.Where(a => a.image_Name == name).FirstOrDefault();

                if(img != null)
                {
                    imgData = db.ImageMetadatas.Where(a => a.image_ID == img.image_ID).FirstOrDefault();
                    Debug.WriteLine("\n\n\n\n\n" + imgData.Address);
                }                
            }

            return View(imgData);
        }

        public ActionResult DisplayImageInfo()
        {

            return View();
        }
    }
}