using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using PlaceBooking.Common;
using PlaceBooking.Models;

namespace PlaceBooking.Areas.Admin.Controllers
{

    [CustomAuthorizeAttribute(RoleID = "SALESMAN")]
    public class OrdersController : BaseController
    {
        private PlaceBookingDbContext db = new PlaceBookingDbContext();

        // GET: Admin/Orders
        public ActionResult Index()
        {
            return View(db.Orders.Where(m=>m.Status!=0).ToList());
        }

        // GET: Admin/Orders/Details/5
        public ActionResult Details(int? id)
        {
                if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);

            List<int> listServiceint = !string.IsNullOrEmpty(order.ServiceId) ? (order.ServiceId?.Split(',')
                   ?.Select(int.Parse)
                   ?.ToList() ?? null) : null;

            List<int> listMenuint = !string.IsNullOrEmpty(order.MenuId) ? (order.MenuId?.Split(',')
                               ?.Select(int.Parse)
                               ?.ToList() ?? null) : null;

            ViewBag.Service = listServiceint != null ? db.ExtendsBookings?.Where(x => x.Type == 2 && listServiceint.Contains(x.ID))?.ToList() : null;
            ViewBag.Menu = listMenuint != null ? db.ExtendsBookings?.Where(x => x.Type == 1 && listMenuint.Contains(x.ID))?.ToList() : null;

            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }
   
        public ActionResult _BookingConnfig(int orderId)
        {
            var list = db.Ordersdetails.Where(m => m.Orderid == orderId).ToList();
            var list1 = new List<Room>();
            foreach (var item in list)
            {
                Room ticket = db.Rooms.Find(item.TicketId);
                list1.Add(ticket);
            }
            return View("_BookingConnfig", list1.ToList());
        }

        //status
        public ActionResult Status(int id)
        {
            Order morder = db.Orders.Find(id);
            morder.Status = (morder.Status == 1) ? 2 : 1;
            db.Entry(morder).State = EntityState.Modified;
            db.SaveChanges();
            Message.set_flash("Thay đổi trang thái thành công", "success");
            return RedirectToAction("Index");
        }
        //trash
        public ActionResult trash()
        {
            var list = db.Orders.Where(m => m.Status == 0).ToList();
            return View("Trash", list);
        }
        public ActionResult Deltrash(int id)
        {
            Order morder = db.Orders.Find(id);
            morder.Status = 0;
            db.Entry(morder).State = EntityState.Modified;
            db.SaveChanges();
            Message.set_flash("Xóa thành công", "success");
            return RedirectToAction("Index");
        }

        public ActionResult Retrash(int id)
        {
            Order morder = db.Orders.Find(id);
            morder.Status = 2;
            db.Entry(morder).State = EntityState.Modified;
            db.SaveChanges();
            Message.set_flash("Khôi phục thành công", "success");
            return RedirectToAction("trash");
        }
        [CustomAuthorizeAttribute(RoleID = "ADMIN")]
        public ActionResult deleteTrash(int id)
        {

            Order morder = db.Orders.Find(id);
            db.Orders.Remove(morder);
            db.SaveChanges();
            Message.set_flash("Đã xóa vĩnh viễn 1 Đơn hàng", "success");
            return RedirectToAction("trash");
        }
    }
}
