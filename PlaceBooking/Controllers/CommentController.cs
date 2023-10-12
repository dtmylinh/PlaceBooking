using PlaceBooking.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace PlaceBooking.Controllers
{
    public class CommentController : Controller
    {
        PlaceBookingDbContext db = new PlaceBookingDbContext();
        // GET: MCQ
        public ActionResult Index()
        {
            return View();
        }
        // GET: Admin/Topic/Details/5                                                                                                                      
        public ActionResult Detail(int type, int topicId)
        {
            ViewBag.userid = string.IsNullOrEmpty(Session["id"].ToString()) ? 0 : int.Parse(Session["id"].ToString());
            var listPost = db.Comments.Where(x=>x.Type == type && x.TopicId == topicId).OrderByDescending(x=>x.ID).ToList();                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          
            return View("Comment", listPost);
        }
        public void Create(Comment comment)
        {
            var userName = string.Empty;
            var userid = 0;
            if (Session["id"].Equals(""))
            {
                Message.set_flash("Bạn chưa đăng nhập.", "success");
                Response.Redirect("~/");
            }
            else
            {
                userName = Session["user"].ToString();
                userid = int.Parse(Session["id"].ToString());
            }

            // Validate
            comment.CreateDate = DateTime.Now;
            comment.Status = 1;
            comment.UserName = userName;
            comment.UserId = userid;
            db.Comments.Add(comment);
            db.SaveChanges();
            Message.set_flash("Thêm bình luận thành công", "success");
            if (comment.Type == 1)
            {
                Response.Redirect($"~/room-detail/{comment.TopicId}");
            }
            else
            {
                Response.Redirect($"~/room-detail/{comment.TopicId}");
            }
        }

        public ActionResult Delete(int Id, int TopicId)
        {
            var comment = db.Comments.SingleOrDefault(x => x.ID == Id);
            if (comment == null)
            {
                Message.set_flash("Xóa bình luận thất bại", "danger");
                return Redirect($"~/room-detail/{1}");
            }
            db.Comments.Remove(comment);
            db.SaveChanges();

            Message.set_flash("Xóa bình luận thành công", "success");
            return Redirect($"~/room-detail/{1}");
        }
        // GET: Admin/Topic/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment mtopic = db.Comments.Find(id);
            if (mtopic == null)
            {
                return HttpNotFound();
            }
            return View(mtopic);
        }

        [HttpPost]
        public ActionResult Edit(Comment comment)
        {
            var comment1 = db.Comments.Find(comment.ID);
            comment1.Comment1 = comment.Comment1;

            if (ModelState.IsValid)
            {
                db.Entry(comment1).State = EntityState.Modified;
                db.SaveChanges();

                Message.set_flash("Cập nhật bình luận thành công.", "success");
                return Redirect($"~/room-detail/{1}");
            }
            return View(comment);
        }
    }
}                                    