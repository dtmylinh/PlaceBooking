
using PlaceBooking.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PlaceBooking.Common;

namespace PlaceBooking.Areas.Admin.Controllers
{
    public class AuthController : Controller
    {
        // GET: Admin/Auth
        private PlaceBookingDbContext db = new PlaceBookingDbContext();
        public ActionResult login()
        {
            return View("_login");
        }
        [HttpPost]
        public ActionResult login(FormCollection fc)
        {
            String Username = fc["username"];
            string Pass = Mystring.ToMD5(fc["password"]);
            var user_account = db.Users.Where(m => m.Access != 1 && m.Status == 1 && (m.Username == Username));
            var userC = db.Users.Where(m => m.Username == Username && m.Access == 1);
            if (userC.Count() != 0)
            {
                ViewBag.error = "Bạn không có quyền đăng nhập";
            }
            else
            {
                if (user_account.Count() == 0)
                {
                    ViewBag.error = "Tên Đăng Nhập Không Đúng";
                }
                else
                {
                    var pass_account = db.Users.Where(m => m.Access != 1 && m.Status == 1 && m.Password == Pass);
                    if (pass_account.Count() == 0)
                    {
                        ViewBag.error = "Mật Khẩu Không Đúng";
                    }
                    else
                    {
                        var user = user_account.First();
                        Role role = db.Roles.Where(m => m.ParentId == user.Access).First();
                        var userSession = new Userlogin();
                        userSession.UserName = user.Username;
                        userSession.UserID = user.Id;
                        userSession.GroupID = role.GropID;
                        userSession.AccessName = role.AccessName;
                        Session.Add(CommonConstants.USER_SESSION, userSession);
                        var i = Session["SESSION_CREDENTIALS"];
                        Session["AccessName"] = role.AccessName;
                        Session["Admin_id"] = user.Id;
                        Session["Admin_user"] = user.Username;
                        Session["Admin_fullname"] = user.Fullname;
                        Response.Redirect("~/Admin");
                    }
                }
            }
            ViewBag.sess = Session["Admin_id"];
            return View("_login");

        }

        public ActionResult logout()
        {
            Session["Admin_id"] = "";
            Session["Admin_user"] = "";
            Response.Redirect("~/Admin");
            return View();
        }
  
        
    }
}