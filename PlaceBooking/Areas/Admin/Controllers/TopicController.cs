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
    [CustomAuthorizeAttribute(RoleID = "COPYWRITER")]
    public class TopicController : BaseController
    {
        private PlaceBookingDbContext db = new PlaceBookingDbContext();

        // GET: Admin/Topic
        public ActionResult Index()
        {
            
            var list = db.Topics.Where(m => m.Status !=0).OrderByDescending(m => m.ID).ToList();
            return View(list);
        }

        // GET: Admin/Topic/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Topic mtopic = db.Topics.Find(id);
            if (mtopic == null)
            {
                return HttpNotFound();
            }
            return View(mtopic);
        }

        // GET: Admin/Topic/Create
        public ActionResult Create()
        {
            ViewBag.listtopic = db.Topics.Where(m => m.Status != 0).ToList();
            var topic = new Topic();
            return View(topic);
        }

        // POST: Admin/Topic/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Topic mtopic)
        {
            if (ModelState.IsValid)
            {
                //category
                string slug = Mystring.ToSlug(mtopic.Name.ToString());
                mtopic.Slug = slug;
                mtopic.CreatedAt = DateTime.Now;
                mtopic.UpdatedAt = DateTime.Now;
                mtopic.CreatedBy = int.Parse(Session["Admin_id"].ToString());
                mtopic.UpdatedBy = int.Parse(Session["Admin_id"].ToString());
                db.Topics.Add(mtopic);
                db.SaveChanges();
                Message.set_flash("Thêm thành công", "success");
                return RedirectToAction("Index");
            }
            Message.set_flash("Thêm thất bại", "danger");
            ViewBag.listtopic = db.Topics.Where(m => m.Status != 0).ToList();
            return View(mtopic);
        }

        // GET: Admin/Topic/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Topic mtopic = db.Topics.Find(id);
            if (mtopic == null)
            {
                return HttpNotFound();
            }
            ViewBag.listtopic = db.Topics.Where(m => m.Status != 0).ToList();
            return View(mtopic);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( Topic mtopic)
        {
            if (ModelState.IsValid)
            {
                string slug = Mystring.ToSlug(mtopic.Name.ToString());
                mtopic.CreatedAt = DateTime.Now;
                mtopic.Slug = slug;
                mtopic.UpdatedAt = DateTime.Now;
                mtopic.UpdatedBy = int.Parse(Session["Admin_id"].ToString());
                db.Entry(mtopic).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.listtopic = db.Topics.Where(m => m.Status != 0).ToList();
            return View(mtopic);
        }

        public ActionResult Status(int id)
        {
            Topic mtopic = db.Topics.Find(id);
            mtopic.Status = (mtopic.Status == 1) ? 2 : 1;
            mtopic.UpdatedAt = DateTime.Now;
            mtopic.UpdatedBy = int.Parse(Session["Admin_id"].ToString());
            db.Entry(mtopic).State = EntityState.Modified;
            db.SaveChanges();
            Message.set_flash("Thay đổi trang thái thành công", "success");
            return RedirectToAction("Index");
        }
        //trash
        public ActionResult trash()
        {
            var list = db.Topics.Where(m => m.Status == 0).ToList();
            return View("Trash", list);
        }
        public ActionResult Deltrash(int id)
        {
            Topic mtopic = db.Topics.Find(id);
            mtopic.Status = 0;
            mtopic.UpdatedAt = DateTime.Now;
            mtopic.UpdatedBy = int.Parse(Session["Admin_id"].ToString());
            db.Entry(mtopic).State = EntityState.Modified;
            db.SaveChanges();
            Message.set_flash("Xóa thành công", "success");
            return RedirectToAction("Index");
        }

        public ActionResult Retrash(int id)
        {
            Topic mtopic = db.Topics.Find(id);
            mtopic.Status = 2;
            mtopic.UpdatedAt = DateTime.Now;
            mtopic.UpdatedBy = int.Parse(Session["Admin_id"].ToString());
            db.Entry(mtopic).State = EntityState.Modified;
            db.SaveChanges();
            Message.set_flash("Khôi phục thành Công", "success");
            return RedirectToAction("trash");
        }
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        public ActionResult deleteTrash(int id)
        {
            Topic mtopic = db.Topics.Find(id);
            db.Topics.Remove(mtopic);
            db.SaveChanges();
            Message.set_flash("Đã xóa vĩnh viễn 1 Chủ đề", "success");
            return RedirectToAction("trash");
        }
    }
}
