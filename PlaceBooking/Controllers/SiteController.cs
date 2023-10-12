using PlaceBooking.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PlaceBooking.Controllers
{
    public class SiteController : Controller
    {
        PlaceBookingDbContext db = new PlaceBookingDbContext();
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult roomSearch(FormCollection fc, int? page)
        {
            if (page == null) page = 1;
            int pageSize = 8;
            int pageNumber = (page ?? 1);

            string LoaiTiec = fc["LoaiTiec"].ToString();

            ViewBag.LoaiTiec = LoaiTiec.ToString();


            int songuoi = int.Parse(fc["songuoi"]);

            ViewBag.songuoi = songuoi.ToString();
            // Địa điểm
            string departure_address = fc["departure_address"];

            ViewBag.departure_address = fc["departure_address"];
            float soTienTu = float.Parse(fc["soTienTu"]);
            float soTienDen = float.Parse(fc["soTienDen"]);

            ViewBag.SoTienTu = soTienTu.ToString();
            ViewBag.SoTienDen = soTienDen.ToString();

            if (soTienTu > soTienDen)
            {
                // Báo lỗi
            }

            var list = db.Rooms.Where(m => m.DepartureAddress.Contains(departure_address)
            && m.RoomType == LoaiTiec
            && m.GuestTotal >= songuoi
            && (m.Price >= soTienTu && m.Price <= soTienDen)
            && m.Status != 0
            ).ToList();
            return View("roomSearchOnway", list.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult AllRoom(int? page)
        {
            if (page == null) page = 1;
            int pageSize = 8;
            ViewBag.url = "chuyen-xe";
            int pageNumber = (page ?? 1);
            ViewBag.breadcrumb = "Tất cả chuyến bay";
            var list_room = db.Rooms.Where(m => m.Status != 0).ToList();
            return View("allroom", list_room.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ActionResult ExtendsBookingDetail(int id)
        {
            var ExtendsBookingDetail = db.ExtendsBookings.Where(m => m.ID == id).FirstOrDefault();
            return View("extendsBookingDetail", ExtendsBookingDetail);
        }

        public ActionResult postOftoPic(int? page, string slug)
        {
            if (page == null) page = 1;
            int pageSize = 8;
            var singleC = db.Topics.Where(m => m.Status == 1 && m.Slug == slug).FirstOrDefault();
            ViewBag.nameTopic = slug;
            ViewBag.url = "tin-tuc/" + slug + "";
            int pageNumber = (page ?? 1);
            var listPost = db.Posts.Where(m => m.Status == 1 && m.Topid == singleC.ID).ToList();
            return View("postOftoPic", listPost.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult AllPost(int? page)
        {
            if (page == null) page = 1;
            int pageSize = 8;

            ViewBag.url = "tin-tuc";
            int pageNumber = (page ?? 1);
            var listPost = db.Posts.Where(m => m.Status == 1).ToList();
            return View("postOftoPic", listPost.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult topic()
        {

            var list = db.Topics.Where(m => m.Status == 1).ToList();
            return View("_topic", list);
        }

        public ActionResult postSearch(string keyw, int? page)
        {
            if (page == null) page = 1;
            int pageSize = 8;
            int pageNumber = (page ?? 1);
            ViewBag.url = "tim-kiem-bai-viet?keyw=" + keyw + "";
            @ViewBag.nameTopic = "Tim kiếm từ khóa: " + keyw;
            var list = db.Posts.Where(m => m.Title.Contains(keyw)).OrderBy(m => m.ID);
            return View("postOftoPic", list.ToList().ToPagedList(pageNumber, pageSize));
        }
        public ActionResult PostDetal(string slug)
        {

            var single = db.Posts.Where(m => m.Status == 1 && m.Slug == slug).First();
            ViewBag.Bra = single.Title;
            return View("PostDetail", single);
        }

        public ActionResult roomDetail(int id)
        {
            if (Session["id"].Equals(""))
            {
                ViewBag.name = "";
            }
            else
            {
                ViewBag.name = Session["user"];
                ViewBag.id = int.Parse(Session["id"].ToString());
            }

            ViewBag.Service = db.ExtendsBookings.Where(x => x.Type == 2 && x.RoomId == id && x.Status == 1).ToList();
            ViewBag.Menu = db.ExtendsBookings.Where(x => x.Type == 1 && x.RoomId == id && x.Status == 1).ToList();

            var single = db.Rooms.Where(m => /*m.Status == 1 &&*/ m.Id == id).First();
            return View("roomDetail", single);
        }
        public ActionResult lienHe()
        {
            var single = db.Posts.Where(m => m.Status == 1 && m.Slug == "lien-he").First();
            return View("PostDetail", single);
        }

    }
}