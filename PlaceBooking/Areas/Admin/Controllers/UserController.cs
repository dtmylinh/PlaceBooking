using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PlaceBooking.Common;
using PlaceBooking.Models;

namespace PlaceBooking.Areas.Admin.Controllers
{


    public class UserController : BaseController
    {
        private PlaceBookingDbContext db = new PlaceBookingDbContext();

        // GET: Admin/User
        public ActionResult Index()
        {
            ViewBag.Accessname = Session["AccessName"].ToString();
            var list = db.Users.Where(m => m.Status != 0 && m.Access != 0).OrderByDescending(m => m.Id).ToList();
            return View(list);
        }

        // GET: Admin/User/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User muser = db.Users.Find(id);
            if (muser == null)
            {
                return HttpNotFound();
            }
            return View(muser);
        }

        // GET: Admin/User/Create
        public ActionResult Create()
        {
            ViewBag.role = db.Roles.Where(x=>x.AccessName != "ADMIN").ToList();
            var topic = new User();
            return View(topic);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( User muser, FormCollection data)
        {
            if (ModelState.IsValid)
            {
                string password1 = data["password1"];
                string password2 = data["password2"];
                string username = muser.Username;
                var Luser = db.Users.Where(m => m.Status == 1 && m.Username == username);
                if (password1!=password2) {ViewBag.error = "PassWord không khớp";}
                if (Luser.Count()>0) { ViewBag.error1 = "Tên Đăng nhâp đã tồn tại";}
                else
                {
                    string pass = Mystring.ToMD5(password1);
                    muser.Img = "ádasd";
                    muser.Password = pass;
                    muser.Address = "";
                    db.Users.Add(muser);
                    db.SaveChanges();
                    Message.set_flash("Tạo user  thành công", "success");
                    return RedirectToAction("Index");
                }
            }
            return View(muser);
        }

        // GET: Admin/User/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User muser = db.Users.Find(id);
            if (muser == null)
            {
                return HttpNotFound();
            }
            ViewBag.role = db.Roles.Where(x => x.AccessName != "ADMIN").ToList();
            return View(muser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( User muser)
        {
            if (ModelState.IsValid)
            {
                    muser.Img = "ádasd";               
                    db.Entry(muser).State = EntityState.Modified;
                    db.SaveChanges();
                Message.set_flash("Cập nhật thành công", "success");
                return RedirectToAction("Index");
            }
            return View(muser);
        }

        //status
        public ActionResult Status(int id)
        {
            User muser = db.Users.Find(id);
            muser.Status = (muser.Status == 1) ? 2 : 1;
            db.Entry(muser).State = EntityState.Modified;
            db.SaveChanges();
            Message.set_flash("Thay đổi trang thái thành công", "success");
            return RedirectToAction("Index");
        }
        //trash
        public ActionResult trash()
        {
            var list = db.Users.Where(m => m.Status == 0).ToList();
            return View("Trash", list);
        }
        public ActionResult Deltrash(int id)
        {
            User muser = db.Users.Find(id);
            muser.Status = 0;
            db.Entry(muser).State = EntityState.Modified;
            db.SaveChanges();
            Message.set_flash("Xóa thành công", "success");
            return RedirectToAction("Index");
        }

        public ActionResult Retrash(int id)
        {
            User muser = db.Users.Find(id);
            muser.Status = 2;
            db.Entry(muser).State = EntityState.Modified;
            db.SaveChanges();
            Message.set_flash("khôi phục thành công", "success");
            return RedirectToAction("trash");
        }
        public ActionResult deleteTrash(int id)
        {
            User muser = db.Users.Find(id);
            db.Users.Remove(muser);
            db.SaveChanges();
            Message.set_flash("Đã xóa vĩnh viễn 1 User", "success");
            return RedirectToAction("trash");
        }

    }
}
