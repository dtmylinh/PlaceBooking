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
using PlaceBooking.Models;

namespace PlaceBooking.Areas.Admin.Controllers
{
    [CustomAuthorizeAttribute(RoleID = "SALESMAN")]
    public class RoomsController : BaseController
    {
       
        private PlaceBookingDbContext db = new PlaceBookingDbContext();

        // GET: Admin/Tickets
        public ActionResult Index()
        {
            ViewBag.Accessname = Session["AccessName"].ToString();
            return View(db.Rooms.Where(m=>m.Status != 0).OrderByDescending(x=>x.Id).ToList());
        }

        // GET: Admin/Tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Room ticket = db.Rooms.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // GET: Admin/Tickets/Create
        public ActionResult Create()
        {
            var ticket = new Room();
            return View(ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(Room ticket)
        {
            
            ticket.Img = "img";
            ticket.Promote = 0;
            if (ModelState.IsValid)
            {
                HttpPostedFileBase file;
                file = Request.Files["img"];
                string filename = file.FileName.ToString();
                string ExtensionFile = Mystring.GetFileExtension(filename);
                string namefilenew = Mystring.ToSlug(DateTime.Now.Second + ticket.Name.ToSlug().ToString()) + "." + ExtensionFile;
                var path = Path.Combine(Server.MapPath("~/Public/images/roomBooking"), namefilenew);
                file.SaveAs(path);
                ticket.CreatedAt = DateTime.Now;
                ticket.UpdatedAt = DateTime.Now;
                ticket.Promote = 0;
                ticket.Img = namefilenew;
                ticket.CreatedBy = int.Parse(Session["Admin_id"].ToString());
                ticket.UpdatedBy = int.Parse(Session["Admin_id"].ToString());
                ticket.PriceSale = ticket.Price;  
                db.Rooms.Add(ticket);
                Message.set_flash("Thêm vé thành công", "success");
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            Message.set_flash("Thêm vé thất bại", "danger");
            var ticketDefault = new Room();
            return View("Create", ticketDefault);
        }
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Room ticket = db.Rooms.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            ViewBag.listTopic = db.Rooms.Where(m => m.Status != 0).ToList();
            return View(ticket);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Room ticket)   
        {

            if (ModelState.IsValid)
            {
                HttpPostedFileBase file;
                string slug = Mystring.ToSlug(ticket.Name.ToString());
                file = Request.Files["img"];
                string filename = file.FileName.ToString();
                if (filename.Equals("") == false)
                {        
                    file = Request.Files["img"];
                    string ExtensionFile = Mystring.GetFileExtension(filename);
                    string namefilenew = Mystring.ToSlug(DateTime.Now.Second+ticket.Name.ToSlug().ToString()) + "." + ExtensionFile;
                    var path = Path.Combine(Server.MapPath("~/Public/images/roomBooking"), namefilenew);
                    ticket.Img = namefilenew;
                    file.SaveAs(path);
                    ticket.Img = namefilenew;
                }
                ticket.UpdatedAt = DateTime.Now;
                ticket.UpdatedBy = int.Parse(Session["Admin_id"].ToString());
                ticket.PriceSale = ticket.Price;
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                Message.set_flash("Sửa thành công", "success");
                return RedirectToAction("Index");
            }
            Message.set_flash("Sửa thất bại", "danger");
            return View("Edit");
        }

        public ActionResult Status(int id)
        {
            Room Rooms = db.Rooms.Find(id);
            Rooms.Status = (Rooms.Status == 1) ? 2 : 1;
            db.Entry(Rooms).State = EntityState.Modified;
            db.SaveChanges();
            Message.set_flash("Thay đổi trang thái thành công", "success");
            return RedirectToAction("Index");
        }
        //trash
        public ActionResult Trash()
        {
            var list = db.Rooms.Where(m => m.Status == 0).ToList();
            return View("Trash", list);
        }
        public ActionResult Deltrash(int id)
        {
            Room morder = db.Rooms.Find(id);
            morder.Status = 0;
            db.Entry(morder).State = EntityState.Modified;
            db.SaveChanges();
            Message.set_flash("Xóa thành công", "success");
            return RedirectToAction("Index");
        }

        public ActionResult Retrash(int id)
        {
            Room morder = db.Rooms.Find(id);
            morder.Status = 2;
            db.Entry(morder).State = EntityState.Modified;
            db.SaveChanges();
            Message.set_flash("Khôi phục thành công", "success");
            return RedirectToAction("trash");
        }
        public ActionResult deleteTrash(int id)
        {
            Room morder = db.Rooms.Find(id);
            db.Rooms.Remove(morder);
            db.SaveChanges();
            Message.set_flash("Đã xóa vĩnh viễn 1 Đơn hàng", "success");
            return RedirectToAction("trash");
        }


    }
}
