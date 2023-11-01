using PlaceBooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PlaceBooking.Controllers
{
    public class HomeController : Controller
    {
        PlaceBookingDbContext db = new PlaceBookingDbContext();
        // GET: Home
        public ActionResult Index()
        {
            var listPost = db.Posts.Where(m => m.Status == 1 && m.Topid == 20).Take(3).ToList();
            return View(listPost);
        }
      
    }
}