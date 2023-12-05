using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PlaceBooking.Common;
//using PlaceBooking.Common;
using PlaceBooking.Models;

namespace PlaceBooking.Areas.Admin.Controllers
{
    [CustomAuthorizeAttribute(RoleID = "COPYWRITER")]
    public class PostController : BaseController
    {
       private PlaceBookingDbContext db = new PlaceBookingDbContext();

        // GET: Admin/Post
        public ActionResult Index()
        {
            var list = db.Posts.Where(m => m.Status > 0).OrderByDescending(m=>m.ID).ToList();
            return View(list);
        }

        // GET: Admin/Post/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post mpost = db.Posts.Find(id);
            if (mpost == null)
            {
                return HttpNotFound();
            }
            return View(mpost);
        }

        // GET: Admin/Post/Create
        public ActionResult Create()
        {
            ViewBag.listTopic = db.Topics.Where(m => m.Status == 1 ).ToList();
            var topic = new Post();
            return View(topic);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(Post mpost)
        {
            mpost.Topid = 20;
            if (ModelState.IsValid)
            {
                HttpPostedFileBase file;
                var namecateDb = db.Topics.Where(m => m.ID == mpost.Topid).First();
                string slug = Mystring.ToSlug(mpost.Title.ToString());
                string namecate = Mystring.ToStringNospace(namecateDb.Name);
                file = Request.Files["img"];
                string filename = file.FileName.ToString();
                string ExtensionFile = Mystring.GetFileExtension(filename);
                string namefilenew = namecate + "/" + slug + "." + ExtensionFile;
                var path = Path.Combine(Server.MapPath("~/public/images/post/"), namefilenew);
                var folder = Server.MapPath("~/public/images/post/" + namecate);
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                file.SaveAs(path);
                mpost.Img = namefilenew;
                mpost.Slug = slug;
                mpost.Type = "Post";
                mpost.CreatedAt = DateTime.Now;
                mpost.UpdatedAt = DateTime.Now;
                mpost.CreatedBy = int.Parse(Session["Admin_id"].ToString());
                mpost.UpdatedBy = int.Parse(Session["Admin_id"].ToString());
                db.Posts.Add(mpost);
                db.SaveChanges();
                //create Link
             
                db.SaveChanges();
                Message.set_flash("Thêm thành công", "success");
                return RedirectToAction("Index");
            }
            ViewBag.listTopic = db.Topics.Where(m => m.Status != 0).ToList();
            Message.set_flash("Thêm Thất Bại", "danger");
            return View(mpost);
        }

        // GET: Admin/Post/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post mpost = db.Posts.Find(id);
            if (mpost == null)
            {
                return HttpNotFound();
            }
            ViewBag.listTopic = db.Topics.Where(m => m.Status != 0).ToList();
            return View(mpost);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit( Post mpost)
        {
            mpost.Topid = 20;
            if (ModelState.IsValid)
            {
                HttpPostedFileBase file;
                string slug = Mystring.ToSlug(mpost.Title.ToString());
                file = Request.Files["img"];
                string filename = file.FileName.ToString();
                if (filename.Equals("") == false)
                {
                    var namecateDb = db.Topics.Where(m => m.ID == mpost.Topid).First();
                    string namecate = Mystring.ToStringNospace(namecateDb.Name);
                    string ExtensionFile = Mystring.GetFileExtension(filename);
                    string namefilenew = namecate + "/" + slug + "." + ExtensionFile;
                    var path = Path.Combine(Server.MapPath("~/public/images/post"), namefilenew);
                    var folder = Server.MapPath("~/public/images/post/" + namecate);
                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }
                    file.SaveAs(path);
                    mpost.Img = namefilenew;
                }
                mpost.Slug = slug;
                mpost.UpdatedAt = DateTime.Now;
                mpost.UpdatedBy = int.Parse(Session["Admin_id"].ToString());
                db.Entry(mpost).State = EntityState.Modified;
                db.SaveChanges();
                Message.set_flash("Sửa thành công", "success");
              
                return RedirectToAction("Index");
            }
            ViewBag.listTopic = db.Topics.Where(m => m.Status != 0).ToList();
            Message.set_flash("Sửa Thất Bại", "danger");
            return View(mpost);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post mpost = db.Posts.Find(id);
            if (mpost == null)
            {
                return HttpNotFound();
            }
            return View(mpost);
        }
        public ActionResult Status(int id)
        {
            Post mpost = db.Posts.Find(id);
            mpost.Status = (mpost.Status == 1) ? 2 : 1;
            mpost.UpdatedAt = DateTime.Now;
            mpost.UpdatedBy = int.Parse(Session["Admin_id"].ToString());
            db.Entry(mpost).State = EntityState.Modified;
            db.SaveChanges();
            Message.set_flash("Thay đổi trang thái thành công", "success");
            return RedirectToAction("Index");
        }
        public ActionResult trash()
        {
            var list = db.Posts.Where(m => m.Status == 0).ToList();
            return View("Trash", list);
        }
        public ActionResult Deltrash(int id)
        {
            Post mpost = db.Posts.Find(id);
            mpost.Status = 0;
            mpost.UpdatedAt = DateTime.Now;
            mpost.UpdatedBy = int.Parse(Session["Admin_id"].ToString());
            db.Entry(mpost).State = EntityState.Modified;
            db.SaveChanges();
            Message.set_flash("Xóa thành công", "success");
            return RedirectToAction("Index");
        }
        public ActionResult Retrash(int id)
        {
            Post mpost = db.Posts.Find(id);
            mpost.Status = 2;
            mpost.UpdatedAt = DateTime.Now;
            mpost.UpdatedBy = int.Parse(Session["Admin_id"].ToString());
            db.Entry(mpost).State = EntityState.Modified;
            db.SaveChanges();
            Message.set_flash("khôi phục thành công", "success");
            return RedirectToAction("trash");
        }
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        public ActionResult deleteTrash(int id)
        {
            Post mpost = db.Posts.Find(id);         
            db.Posts.Remove(mpost);
            db.SaveChanges();
            Message.set_flash("Đã xóa vĩnh viễn 1 Bài Vết", "success");
            return RedirectToAction("trash");
        }
    }
}
