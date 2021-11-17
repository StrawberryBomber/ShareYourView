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
    public class CreateController : Controller
    {
        // GET: Create
        [HttpGet]
        public ActionResult uploadNewImage()
        {
            return View();
        }
        [HttpPost]
        public ActionResult uploadNewImage(ImageMetadata metadata)
        {
            GoogleDriveAPIHelper.UplaodFileOnDrive(metadata.imageFile);
            

            using (shareYourView_DBEntities db = new shareYourView_DBEntities())
            {
                string name = Path.GetFileNameWithoutExtension(metadata.imageFile.FileName) + "@" + HttpContext.User.Identity.Name + Path.GetExtension(metadata.imageFile.FileName);

                Debug.WriteLine("\n\n\n\n" + name);
                var x = db.ImageDetails.Where(a => a.image_Name == name).FirstOrDefault();
                
                if(x != null)
                {
                    metadata.image_ID = x.image_ID;
                    Debug.WriteLine("\n\n\n\n\n" + metadata.image_ID);
                    db.ImageMetadatas.Add(metadata);
                    db.SaveChanges();                    
                }                
            }
            ModelState.Clear();

            ViewBag.Message = "File Uploaded Successfully";
            return View(metadata);
        }

    }
}