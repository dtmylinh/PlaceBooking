using PlaceBooking.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using API_NganLuong;
using PlaceBooking.nganluonAPI;
using MoMo;
using Newtonsoft.Json.Linq;
using PlaceBooking.MomoAPI;
using System.Globalization;
using System.Data.Entity.Core.Objects;
using System.Web.Services.Description;
using Org.BouncyCastle.Asn1.X509;

namespace PlaceBooking.Controllers
{
    public class CheckoutController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        PlaceBookingDbContext db = new PlaceBookingDbContext();
        // GET: Checkout
        [HttpPost]
        public ActionResult Index(FormCollection fc)
        {
            int id = int.Parse(fc["idRoom"]);
            var list = new List<Room>();
            var id1 = string.IsNullOrEmpty(Session["id"].ToString()) ? 0 : int.Parse(Session["id"].ToString());
            if (id1 == 0)
            {
                Message.set_flash("Vui lòng đăng nhập ", "danger");
                return Redirect("~/dang-nhap");
            }
            var Scope = fc["Scope"];
            ViewBag.Scope = Scope;

            // Ngày chọn đặt
            string StrDepartureDate = fc["departureDate"];
            DateTime departureDate = DateTime.ParseExact(StrDepartureDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            ViewBag.departureDate = StrDepartureDate;

            // Lấy ngày hiện tại
            DateTime currentDate = DateTime.Now;
            if (departureDate <= currentDate)
            {
                Message.set_flash("Vui lòng chọn ngày lớn hơn ngày hiện tại.", "danger");
                return Redirect($"~/room-detail/{id}");
            }

            var dateToCompare = departureDate.Date;
            var listOrderId = db.Orders
                .Where(x => x.RoomId == id && x.Scope == Scope && EntityFunctions.TruncateTime(x.DepartureDate) == dateToCompare)
                .FirstOrDefault();


            if (listOrderId != null)
            {
                Message.set_flash($"Ngày {departureDate.ToString("dd/MM/yyyy")}, khung giờ {Scope} đã có người đặt. Vui lòng chọn ngày khác.", "danger");
                return Redirect($"~/room-detail/{id}");
            }

            // Kiểm tra ngày này đã được đặt chưa
            // Kiểm tra ngày hôm đó có đơn đặt chưa ==> Show thông báo
            // Lưu ngày lại rồi trả ra 
            var serviceId = fc["idService"];
            var idMenuId = fc["idMenu"];


            var list1 = db.Rooms.Find(id);
            list.Add(list1);

            ViewBag.ve1 = id;
            ViewBag.idService = serviceId;
            ViewBag.idMenu = idMenuId;
            ViewBag.DepartureDate = StrDepartureDate;
            List<int> listServiceint = !string.IsNullOrEmpty(serviceId) ?  (serviceId?.Split(',')
                               ?.Select(int.Parse)
                               ?.ToList() ?? null) : null;

            List<int> listMenuint = !string.IsNullOrEmpty(idMenuId) ? (idMenuId?.Split(',')
                               ?.Select(int.Parse)
                               ?.ToList() ?? null) : null;

            ViewBag.Service = listServiceint != null ? db.ExtendsBookings?.Where(x => x.Type == 2 && listServiceint.Contains(x.ID))?.ToList(): null;
            ViewBag.Menu = listMenuint != null ? db.ExtendsBookings?.Where(x => x.Type == 1 && listMenuint.Contains(x.ID))?.ToList() : null;

            return View("", list.ToList());
        }
        [HttpPost]
        public ActionResult checkOut(Order order, FormCollection fc)
        {
            string orderCode = "DH"+DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
            Session["OrderCode1"] = orderCode;
            float total = float.Parse(fc["total"]);
            int id1 = int.Parse(fc["veOnvay"]);
            string veReturn = fc["veReturn"];
            string payment_method = Request["option_payment"];

            // Xử lý ngày 
            string StrDepartureDate = fc["departureDate"];
            DateTime departureDate = DateTime.ParseExact(StrDepartureDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            order.DepartureDate = departureDate;

            var serviceId = fc["idService"];
            var idMenuId = fc["idMenu"];
            var Scope = fc["Scope"];

            List<int> listServiceint = !string.IsNullOrEmpty(serviceId) ? (serviceId?.Split(',')
                               ?.Select(int.Parse)
                               ?.ToList() ?? null) : null;

            List<int> listMenuint = !string.IsNullOrEmpty(idMenuId) ? (idMenuId?.Split(',')
                               ?.Select(int.Parse)
                               ?.ToList() ?? null) : null;

            order.MenuId = idMenuId;
            order.ServiceId = serviceId;
            order.Scope = Scope;
            ViewBag.Scope = Scope;

            ViewBag.Service1 = listServiceint != null ? db.ExtendsBookings?.Where(x => x.Type == 2 && listServiceint.Contains(x.ID))?.ToList() : null;
            ViewBag.Menu1 = listMenuint != null ? db.ExtendsBookings?.Where(x => x.Type == 1 && listMenuint.Contains(x.ID))?.ToList() : null;

            if (payment_method.Equals("COD"))
            {
                // Cập nhật thông tin sau khi thanh toán thành công
                // 3 Tiền mặt
                // 2 MOMO
                SaveOrder(order, total, id1, veReturn, "Thanh toán chuyển khoản", 3, orderCode);
                return View("checkOutComfin", order);
            }
            else if (payment_method.Equals("MOMO"))
            {
                if(order.Total > 10000000)
                {
                    Message.set_flash($"Phương thức thanh toán MOMO chỉ cấp nhận thanh toán dưới 10 triệu VND.", "danger");
                    return Redirect($"~/room-detail/{id1}");
                }
                //request params need to request to MoMo system
                string endpoint = momoInfo.endpoint;
                string partnerCode = momoInfo.partnerCode;
                string accessKey = momoInfo.accessKey;
                string serectkey = momoInfo.serectkey;
                string orderInfo = momoInfo.orderInfo;
                string returnUrl = momoInfo.returnUrl;
                string notifyurl = momoInfo.notifyurl;

                string amount = total.ToString();
                string orderid = Guid.NewGuid().ToString();
                string requestId = Guid.NewGuid().ToString();
                string extraData = "";

                //Before sign HMAC SHA256 signature
                string rawHash = "partnerCode=" +
                    partnerCode + "&accessKey=" +
                    accessKey + "&requestId=" +
                    requestId + "&amount=" +
                    amount + "&orderId=" +
                    orderid + "&orderInfo=" +
                    orderInfo + "&returnUrl=" +
                    returnUrl + "&notifyUrl=" +
                    notifyurl + "&extraData=" +
                    extraData;

                log.Debug("rawHash = " + rawHash);
                MoMoSecurity crypto = new MoMoSecurity();
                //sign signature SHA256
                string signature = crypto.signSHA256(rawHash, serectkey);
                log.Debug("Signature = " + signature);

                //build body json request
                JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderid },
                { "orderInfo", orderInfo },
                { "returnUrl", returnUrl },
                { "notifyUrl", notifyurl },
                { "extraData", extraData },
                { "requestType", "captureMoMoWallet" },
                { "signature", signature }

            };
                log.Debug("Json request to MoMo: " + message.ToString());
                string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());
                JObject jmessage = JObject.Parse(responseFromMomo);

                SaveOrder(order, total, id1, veReturn, "Thanh toán MOMO", 2, orderCode);
                return Redirect(jmessage.GetValue("payUrl").ToString());
            }

            return View("checkOutComfin", order);
        }
        // Lấy thông tin các vé đã book
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

        public void SaveOrder(Order order, float total,int id1,string veReturn, string paymentMethod, int StatusPayment, string ordercode)
        {

            BillOrder billOrder = new BillOrder();
            billOrder.OrderCode = ordercode;
            billOrder.Total = (decimal)total;
            billOrder.CreateDate = DateTime.Now;
            billOrder.PaymentStatus = 2;
            billOrder.PaymentType = StatusPayment;
            billOrder.Name = $"Hóa đơn cho hợp đồng {ordercode}";
            billOrder.Status = 1;
            billOrder.UserId = string.IsNullOrEmpty(Session["id"].ToString()) ? 0 : int.Parse(Session["id"].ToString());
            db.BillOrders.Add(billOrder);

            // tru so luong nghe
            var ticket = db.Rooms.Find(id1);
            order.CreateDate = DateTime.Now;
            order.Status = 2;
            order.UserId = string.IsNullOrEmpty(Session["id"].ToString()) ? 0 : int.Parse(Session["id"].ToString()) ;
            order.Total = total;
            order.DeliveryPaymentMethod = paymentMethod;
            order.StatusPayment = StatusPayment;
            order.Code = ordercode;
            order.NameRoom = ticket.Name;
            order.RoomId = id1;
            db.Orders.Add(order);
            db.SaveChanges();
            int lastOrderID = order.Id;
            Ordersdetail orderDetail = new Ordersdetail();
            orderDetail.TicketId = id1;
            orderDetail.Orderid = lastOrderID;
            db.Ordersdetails.Add(orderDetail);

            //ticket.guestTotal = ticket.guestTotal - order.guestTotal;
            ticket.IsBooking = 1;
            db.Entry(ticket).State = EntityState.Modified;
            //neu ton tai ve 2 chieu
            if (!string.IsNullOrEmpty(veReturn))
            {
                int id2 = int.Parse(veReturn);
                Ordersdetail orderDetail2 = new Ordersdetail();
                orderDetail2.TicketId = id2;
                orderDetail2.Orderid = lastOrderID;
                db.Ordersdetails.Add(orderDetail2);
                // tru so luong nghe
                var ticket2 = db.Rooms.Find(id2);
                ticket2.GuestTotal = ticket2.GuestTotal - order.GuestTotal;
                db.Entry(ticket2).State = EntityState.Modified;
            }
            db.SaveChanges();
            if(order.StatusPayment == 3)
            {
                // neeu chon hinh thuc thanh toan Mua ves
                //SendEmail(order.email, order.name);
            }
          
        }

        //Khi huy thanh toán Ngan Luong
        public ActionResult cancel_order()
        {

            return View("cancel_order");
        }
        //Khi thanh toán Ngan Luong XOng
        public ActionResult confirm_orderPaymentOnline()
        {
            String Token = Request["token"];
            RequestCheckOrder info = new RequestCheckOrder();
            info.Merchant_id = nganluongInfo.Merchant_id;
            info.Merchant_password = nganluongInfo.Merchant_password;
            info.Token = Token;
            APICheckoutV3 objNLChecout = new APICheckoutV3();
            ResponseCheckOrder result = objNLChecout.GetTransactionDetail(info);
            if (result.errorCode == "00")
            {
                String codeOrder = Session["OrderId1"].ToString();
                var OrderInfo = db.Orders.OrderByDescending(m => m.Code == codeOrder).FirstOrDefault();
                OrderInfo.StatusPayment = 1;
                db.Entry(OrderInfo).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.paymentStatus = OrderInfo.StatusPayment;
                ViewBag.Methodpayment = OrderInfo.DeliveryPaymentMethod;
                //send email
                //SendEmail(OrderInfo.email, OrderInfo.name);
                return View("checkOutComfin", OrderInfo);
            }
            else
            {
                ViewBag.status = false;
            }

            return View("confirm_orderPaymentOnline");
        }

        //Khi huy thanh toán MOMO
        public ActionResult cancel_order_momo()
        {

            return View("cancel_order");
        }
        //Khi Thanh toám momo xong
        [HttpGet]
        public ActionResult confirm_orderPaymentOnline_momo()
        {

            String errorCode = Request["errorCode"];
            string orderId = Session["OrderCode1"].ToString();
            if (errorCode == "0")
            {
                Session["SessionCart"] = null;
                var OrderInfo = db.Orders.Where(m => m.Code == orderId).FirstOrDefault();
                OrderInfo.StatusPayment = 1;
                BillOrder billOrder = db.BillOrders.Where(x => x.OrderCode == orderId).FirstOrDefault();
                billOrder.PaymentStatus = 1;
                db.Entry(billOrder).State = EntityState.Modified;
                db.Entry(OrderInfo).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.paymentStatus = OrderInfo.StatusPayment;
                ViewBag.Methodpayment = OrderInfo.DeliveryPaymentMethod;
                //send mail
                //SendEmail(OrderInfo.email, OrderInfo.name);

                List<int> listServiceint = !string.IsNullOrEmpty(OrderInfo.ServiceId) ? (OrderInfo.ServiceId?.Split(',')
                ?.Select(int.Parse)
                ?.ToList() ?? null) : null;

                List<int> listMenuint = !string.IsNullOrEmpty(OrderInfo.MenuId) ? (OrderInfo.MenuId?.Split(',')
                ?.Select(int.Parse)?.ToList() ?? null) : null;
                if(listServiceint != null || listMenuint!= null)
                {
                    ViewBag.Service1 = listServiceint != null ? db.ExtendsBookings?.Where(x => x.Type == 2 && listServiceint.Contains(x.ID))?.ToList() : null;
                    ViewBag.Menu1 = listMenuint != null ? db.ExtendsBookings?.Where(x => x.Type == 1 && listMenuint.Contains(x.ID))?.ToList() : null;
                }

                return View("checkOutComfin", OrderInfo);
            }
            else
            {
                ViewBag.status = false;
            }
            return View("confirm_orderPaymentOnline");
        }
    }
}