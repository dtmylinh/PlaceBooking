using PlaceBooking.Common;
using PlaceBooking.Models;
using System.Linq;
using System.Web.Mvc;

namespace PlaceBooking.Areas.Admin.Controllers
{
    [CustomAuthorizeAttribute(RoleID = "SALESMAN")]
    public class CommentsController : BaseController
    {
        private PlaceBookingDbContext db = new PlaceBookingDbContext();
        // GET: Admin/Topic
        public ActionResult Index()
        {
            var list = db.Comments.OrderByDescending(m => m.ID).ToList();
            return View(list);
        }
        public ActionResult deleteTrash(int id)
        {
            Comment comment = db.Comments.Find(id);
            db.Comments.Remove(comment);
            db.SaveChanges();
            Message.set_flash("Đã xóa vĩnh viễn 1 bình luận", "success");
            return RedirectToAction("index");
        }
    }
}