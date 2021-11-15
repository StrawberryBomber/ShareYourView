
using System;
using System.Collections.Generic;
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
            return View();
        }
    }
}