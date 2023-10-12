using PlaceBooking.Common;
using PlaceBooking.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PlaceBooking.Areas.Admin.Controllers
{
    public class DashboardController : BaseController
    {
        private PlaceBookingDbContext db = new PlaceBookingDbContext();
        // GET: Admin/Dashboard
        public ActionResult Index()
        {
            var orders = db.Orders.ToList(); // Thay thế db.Orders bằng nguồn dữ liệu của bạn

            // Lấy danh sách các đơn hàng theo tuần
            var weeklyRevenue = orders.GroupBy(order => new
            {
                Year = order.CreateDate.Year,
                Week = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(order.CreateDate, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)
            })
            .Select(group => new
            {
                Year = group.Key.Year,
                Week = group.Key.Week,
                Revenue = group.Sum(order => order.Total)
            });

            // Lấy danh sách các đơn hàng theo tháng
            var monthlyRevenue = orders.GroupBy(order => new
            {
                Year = order.CreateDate.Year,
                Month = order.CreateDate.Month
            })
            .Select(group => new
            {
                Year = group.Key.Year,
                Month = group.Key.Month,
                Revenue = group.Sum(order => order.Total)
            });

            // Lấy danh sách các đơn hàng theo năm
            var yearlyRevenue = orders.GroupBy(order => order.CreateDate.Year)
                .Select(group => new
                {
                    Year = group.Key,
                    Revenue = group.Sum(order => order.Total)
                });

            ViewBag.WeeklyRevenue = weeklyRevenue.FirstOrDefault()?.Revenue ?? 0;
            ViewBag.MonthlyRevenue = monthlyRevenue.FirstOrDefault()?.Revenue ?? 0;
            ViewBag.YearlyRevenue = yearlyRevenue.FirstOrDefault()?.Revenue ?? 0;

            // Lấy danh sách các đơn hàng theo tuần
            var weeklyRevenue1 = orders.GroupBy(order => new
            {
                Year = order.CreateDate.Year,
                Week = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(order.CreateDate, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)
            })
            .Select(group => new
            {
                Year = group.Key.Year,
                Week = group.Key.Week,
                Revenue = group.Sum(order => order.Total),
                NumberOfOrders = group.Count()
            });

            // Lấy danh sách các đơn hàng theo tháng
            var monthlyRevenue1 = orders.GroupBy(order => new
            {
                Year = order.CreateDate.Year,
                Month = order.CreateDate.Month
            })
            .Select(group => new
            {
                Year = group.Key.Year,
                Month = group.Key.Month,
                Revenue = group.Sum(order => order.Total),
                NumberOfOrders = group.Count()
            });

            // Lấy danh sách các đơn hàng theo năm
            var yearlyRevenue1 = orders.GroupBy(order => order.CreateDate.Year)
                .Select(group => new
                {
                    Year = group.Key,
                    Revenue = group.Sum(order => order.Total),
                    NumberOfOrders = group.Count()
                });

            ViewBag.WeeklyCountContract = weeklyRevenue1.FirstOrDefault()?.NumberOfOrders ?? 0;
            ViewBag.MonthlyCountContract = monthlyRevenue1.FirstOrDefault()?.NumberOfOrders ?? 0;
            ViewBag.YearlyCountContract = yearlyRevenue1.FirstOrDefault()?.NumberOfOrders ?? 0;

            return View();
        }
        public ActionResult usersession()
        {
            var session = (Userlogin)Session[Common.CommonConstants.USER_SESSION];
            return View("_adminSession", session);
        }
        
    }
}