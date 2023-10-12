using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PlaceBooking
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
            name: "tra-cuu-order",
            url: "tra-cuu-order",
            defaults: new { controller = "Site", action = "OrderSearch", id = UrlParameter.Optional }
        );
            routes.MapRoute(
                name: "chi-tiet-order",
                url: "chi-tiet-order/{id}",
                defaults: new { controller = "Customer", action = "orderDetailCus", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                 name: "lien-he",
                 url: "lien-he",
                 defaults: new { controller = "Site", action = "lienHe", id = UrlParameter.Optional }
             );

            routes.MapRoute(
            name: "myaccout",
            url: "tai-khoan",
            defaults: new { controller = "Customer", action = "Myaccount", id = UrlParameter.Optional }
            );

            //xu ly thanh toán
            routes.MapRoute(
              name: "huy thanh toan online",
              url: "cancel-order",
              defaults: new { controller = "Checkout", action = "cancel_order", id = UrlParameter.Optional }
              );

            routes.MapRoute(
           name: "thanh toan thanh cong",
           url: "confirm-orderPaymentOnline",
           defaults: new { controller = "Checkout", action = "confirm_orderPaymentOnline", id = UrlParameter.Optional }
           );

            routes.MapRoute(
           name: "huy thanh toan online momo",
           url: "cancel-order-momo",
           defaults: new { controller = "Checkout", action = "cancel_order_momo", id = UrlParameter.Optional }
           );

            routes.MapRoute(
           name: "thanh toan thanh cong momo",
           url: "confirm-orderPaymentOnline-momo",
           defaults: new { controller = "Checkout", action = "confirm_orderPaymentOnline_momo", id = UrlParameter.Optional }
           );
            routes.MapRoute(
               name: "Đăng nhập",
               url: "dang-nhap",
               defaults: new { controller = "Customer", action = "login", id = UrlParameter.Optional }
           );
            routes.MapRoute(
               name: "Đăng nhập google",
               url: "dang-nhap-google",
               defaults: new { controller = "Customer", action = "GoogleCallback", id = UrlParameter.Optional }
            );
            routes.MapRoute(
              name: "dăng ký",
              url: "dang-ky",
              defaults: new { controller = "Customer", action = "register", id = UrlParameter.Optional }
          ); 
            routes.MapRoute(
               name: "chu-tiet-bv",
               url: "chi-tiet-bai-viet/{slug}",
               defaults: new { controller = "Site", action = "PostDetal", id = UrlParameter.Optional }
           );
            routes.MapRoute(
                 name: "tim-kiem-bai-viet",
                 url: "tim-kiem-bai-viet",
                 defaults: new { controller = "Site", action = "postSearch", id = UrlParameter.Optional }
             );
            routes.MapRoute(
                 name: "ctin-tuc",
                 url: "tin-tuc/{slug}",
                 defaults: new { controller = "Site", action = "postOftoPic", id = UrlParameter.Optional }
             );
            routes.MapRoute(
                 name: "ctin-tuc1",
                 url: "tin-tuc",
                 defaults: new { controller = "Site", action = "AllPost", id = UrlParameter.Optional }
             );
            routes.MapRoute(
               name: "chi tiet phòng",
               url: "room-detail/{id}",
               defaults: new { controller = "Site", action = "roomDetail", id = UrlParameter.Optional }
           );
            routes.MapRoute(
                name: "Tất cả phòng",
                url: "all-room",
                defaults: new { controller = "Site", action = "AllRoom", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );


        }
    }
}
