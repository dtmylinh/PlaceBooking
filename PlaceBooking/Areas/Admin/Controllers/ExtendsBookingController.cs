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
    [CustomAuthorizeAttribute(RoleID = "SALESMAN")]
    public class ExtendsBookingController : BaseController
    {
        private PlaceBookingDbContext db = new PlaceBookingDbContext();

        // GET: Admin/Topic
        public ActionResult Index()
        {
            ViewBag.Accessname = Session["AccessName"].ToString();
            var list = db.ExtendsBookings.Where(m => m.Status !=0).OrderByDescending(m => m.ID).ToList();
            return View(list);
        }

        // GET: Admin/Topic/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExtendsBooking mtopic = db.ExtendsBookings.Find(id);
            if (mtopic == null)
            {
                return HttpNotFound();
            }
            return View(mtopic);
        }

        // GET: Admin/Topic/Create
        public ActionResult Create()
        {
            ViewBag.Room = db.Rooms.Where(m => m.Status != 0).ToList();
            var extendsBookings = new ExtendsBooking();
            return View(extendsBookings);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(ExtendsBooking extendsBooking)
        {
            if (ModelState.IsValid)
            {
                //category
                string slug = Mystring.ToSlug(extendsBooking.Name.ToString());
                extendsBooking.CreateDate = DateTime.Now;
                extendsBooking.CreateBy = int.Parse(Session["Admin_id"].ToString());
                db.ExtendsBookings.Add(extendsBooking);
                db.SaveChanges();
                Message.set_flash("Thêm thành công", "success");
                return RedirectToAction("Index");
            }
            Message.set_flash("Thêm thất bại", "danger");
            ViewBag.Room = db.Rooms.Where(m => m.Status != 0).ToList();
            return View(extendsBooking);
        }

        // GET: Admin/Topic/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
                ExtendsBooking mtopic = db.ExtendsBookings.Find(id);
            if (mtopic == null)
            {
                return HttpNotFound();
            }
            ViewBag.Room = db.Rooms.Where(m => m.Status != 0).ToList();
            return View(mtopic);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(ExtendsBooking mtopic)
        {
            if (ModelState.IsValid)
            {
                string slug = Mystring.ToSlug(mtopic.Name.ToString());
                mtopic.CreateDate = DateTime.Now;
                mtopic.CreateBy = int.Parse(Session["Admin_id"].ToString());
                db.Entry(mtopic).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Room = db.Rooms.Where(m => m.Status != 0).ToList();
            return View(mtopic);
        }

        public ActionResult Status(int id)
        {
            ExtendsBooking mtopic = db.ExtendsBookings.Find(id);
            mtopic.Status = (mtopic.Status == 1) ? 2 : 1;
            mtopic.CreateDate = DateTime.Now;
            mtopic.CreateBy = int.Parse(Session["Admin_id"].ToString());
            db.Entry(mtopic).State = EntityState.Modified;
            db.SaveChanges();
            Message.set_flash("Thay đổi trang thái thành công", "success");
            return RedirectToAction("Index");
        }
        //trash
        public ActionResult trash()
        {
            var list = db.ExtendsBookings.Where(m => m.Status == 0).ToList();
            return View("Trash", list);
        }
        public ActionResult Deltrash(int id)
        {
            ExtendsBooking mtopic = db.ExtendsBookings.Find(id);
            mtopic.Status = 0;
            mtopic.CreateDate = DateTime.Now;
            mtopic.CreateBy = int.Parse(Session["Admin_id"].ToString());
            db.Entry(mtopic).State = EntityState.Modified;
            db.SaveChanges();
            Message.set_flash("Xóa thành công", "success");
            return RedirectToAction("Index");
        }

        public ActionResult Retrash(int id)
        {
            ExtendsBooking mtopic = db.ExtendsBookings.Find(id);
            mtopic.Status = 2;
            mtopic.CreateDate = DateTime.Now;
            mtopic.CreateBy = int.Parse(Session["Admin_id"].ToString());
            db.Entry(mtopic).State = EntityState.Modified;
            db.SaveChanges();
            Message.set_flash("Khôi phục thành Công", "success");
            return RedirectToAction("trash");
        }
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        public ActionResult deleteTrash(int id)
        {
            ExtendsBooking mtopic = db.ExtendsBookings.Find(id);
            db.ExtendsBookings.Remove(mtopic);
            db.SaveChanges();
            Message.set_flash("Đã xóa viễn 1 Chủ đề", "success");
            return RedirectToAction("trash");
        }
    }
}
