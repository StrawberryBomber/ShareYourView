using ShareYourView.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShareYourView.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        [Authorize]
        public ActionResult Index()
        {            
            return View();
        }


        //viewSharedImages
        [Authorize]
        [HttpPost]
        public ActionResult viewImages()
        {
            return RedirectToAction("GetGoogleDriveFiles", "Image");
        }

        [Authorize]
        [HttpPost]
        public ActionResult viewSharedImages()
        {
            return RedirectToAction("GetGoogleShareDriveFiles", "Share");
        }

    }
}