using PlaceBooking.Common;
using PlaceBooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Google.Apis.Gmail.v1;
using System.IO;
using Google.Apis.Auth.OAuth2;
using System.Threading;
using Google.Apis.Util.Store;
using Google.Apis.Services;
using System.Text;

namespace PlaceBooking.Controllers
{
    public class CustomerController : Controller
    {
        private readonly string clientId = "844620552810-umuq8bscgjsrfh37v7jd1jlva2d5jmnd.apps.googleusercontent.com";
        private readonly string clientSecret = "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";
        private readonly string redirectUri = "http://localhost:22222/Customer/GoogleCallback";
        private readonly string userInfoUrl = "https://www.googleapis.com/oauth2/v3/userinfo";

        private PlaceBookingDbContext db = new PlaceBookingDbContext();
        // GET: Customer

        public ActionResult Login()
        {
            return View("Login");
        }
        public ActionResult verifyOTP()
        {
            ViewBag.usernameRegister = Session["userNameRegister"].ToString();
            return View("VerifyOTP");
        }
        [HttpPost]
        public ActionResult VerifyOTP(FormCollection fc)
        {
            string username = fc["username"].ToString();
            string oTP = fc["OTP"].ToString();

            var user_accountOTP = db.Users.Where(m => m.Access == 1 && m.Status == 3 && (m.Username == username)).FirstOrDefault();
            if (user_accountOTP == null)
            {
                Message.set_flash("Không tìm thấy thông tin tài khoản, vui lòng thử lại.", "danger");
                return Redirect("~/customer/verifyOTP");
            }
            user_accountOTP.Status = 1;
            db.Entry(user_accountOTP).State = EntityState.Modified;
            db.SaveChanges();
            Session["userNameRegister"] = "";
            Message.set_flash("Xác nhận OTP thành công", "success");
            return Redirect("~/dang-nhap");
        }
        [HttpPost]
        public ActionResult login(FormCollection fc)
        {
            String Username = fc["username"];
            string Pass = Mystring.ToMD5(fc["password"]);

            var user_accountOTP = db.Users.Where(m => m.Access == 1 && m.Status == 3 && (m.Username == Username)).FirstOrDefault();
            if(user_accountOTP != null)
            {
                Session["userNameRegister"] = user_accountOTP.Username;
                Message.set_flash("Tài khoản chưa xác nhận, vui lòng xác nhận.", "success");
                return Redirect("~/customer/verifyOTP");
            }

            var user_account = db.Users.Where(m => m.Access == 1 && m.Status == 1 && (m.Username == Username));

            if (user_account.Count() == 0)
            {
                ViewBag.error = "Tên đăng nhâp không đúng";
            }
            else
            {
                var pass_account = db.Users.Where(m => m.Access != 1 && m.Status == 1 && m.Password == Pass);
                if (pass_account.Count() == 0)
                {
                    ViewBag.error = "Mật khẩu không đúng";
                }
                else
                {
                    var user = user_account.First();
                    Session.Add(CommonConstants.CUSTOMER_SESSION, user);
                    Session["userName11"] = user.Fullname;
                    Session["id"] = user.Id;
                    Session["user"] = user.Username;
                    if (!Response.IsRequestBeingRedirected)
                    Message.set_flash("Đăng nhập thành công ", "success");
                    var id = string.IsNullOrEmpty(Session["idTricket"].ToString()) ? 0 : int.Parse(Session["idTricket"].ToString());
                    if(id != 0)
                    {
                        Session["idTricket"] = "";
                        return Redirect("~/Checkout/Index");
                    }
                    return Redirect("~/tai-khoan");
                }
            }

            ViewBag.sess = Session["Admin_id"];
            return View("Login");

        }
        public void Logout()
        {
            Session["userName11"] = "";
            Session["id"] = "";
            Session[Common.CommonConstants.CUSTOMER_SESSION] = null;
            Message.set_flash("Đăng xuất thành công", "success");
            Response.Redirect("~/dang-nhap");

        }
        public ActionResult register()
        {
            return View("register");
        }
        [HttpPost]
        [Obsolete]
        public ActionResult Register(User muser, FormCollection fc)
        {                   
            string uname = fc["uname"];
            string fname = fc["fname"];
            string Pass = Mystring.ToMD5(fc["psw"]);
            string Pass2 = Mystring.ToMD5(fc["repsw"]);
            if (Pass2 != Pass)
            {
                ViewBag.error = "Mật khẩu không khớp";
                return View("loginEndRegister");
            }
            string email = fc["email"];
            string address = fc["address"];
            string phone = fc["phone"];
            if (ModelState.IsValid)
            {
                var Luser = db.Users.Where(m => m.Status == 1 && m.Username == uname && m.Access == 1);
                if (Luser.Count() > 0)
                {
                    ViewBag.error = "Tên đăng nhập đã tồn tại";
                    return View("loginEndRegister");
                }
                else
                {
                    muser.Img = "defalt.png";
                    muser.Password = Pass;
                    muser.Username = uname;
                    muser.Fullname = fname;
                    muser.Email = email;
                    muser.Address = address;
                    muser.Phone = phone;
                    muser.Gender = "nam";
                    muser.Access = 1;
                    muser.Status = 3;
                    muser.OTP = GenerateOTP(5).ToString();
                    db.Users.Add(muser);
                    db.SaveChanges();
                    Session["userNameRegister"] = muser.Username;
                    SendEmailAsync(muser.Email, muser.Fullname, muser.OTP);
                    Message.set_flash($"Đã gửi mã OTP xác nhận đăng ký đến {muser.Email} vui lòng kiểm tra email để lấy mã xác nhận.", "success");
                    return Redirect("~/customer/verifyOTP");
                }   
            }
            Message.set_flash("Đăng ky tài khoản thất bại", "danger");
            return View("register");
        }
        public ActionResult Myaccount()
        {
            User sessionUser = new User();
            try
            {
                sessionUser = (User)Session[Common.CommonConstants.CUSTOMER_SESSION];
            }
            catch
            {
                Message.set_flash("Vui lòng đang nhập ", "danger");
                return Redirect("~/dang-nhap");
            }
            if (sessionUser == null)
            {
                Message.set_flash("Vui lòng đang nhập ", "danger");
                return Redirect("~/dang-nhap");
            }

            return View("Myaccount", sessionUser);
        }
        [HttpPost]
        public ActionResult Myaccount(User user)
        {
            Session[Common.CommonConstants.CUSTOMER_SESSION] = "";
            Session.Add(CommonConstants.CUSTOMER_SESSION, user);
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            ViewBag.success = "Cập nhật thành công.";
            return View("Myaccount", user);
        }
        public ActionResult ListOderCus()
        {
            User sessionUser = new User();
            try
            {
                sessionUser = (User)Session[Common.CommonConstants.CUSTOMER_SESSION];
            }
            catch
            {
                Message.set_flash("Vui lòng đang nhập ", "danger");
                return Redirect("~/dang-nhap");
            }
            if (sessionUser == null)
            {
                Message.set_flash("Vui lòng đang nhập ", "danger");
                return Redirect("~/dang-nhap");
            }

            var listOrder = db.Orders.Where(m => m.Email == sessionUser.Email || m.Phone == sessionUser.Phone || m.UserId == sessionUser.Id).OrderByDescending(m => m.Id).ToList();
            return View("ListOderCus", listOrder);
        }
        public ActionResult ListOrderBill()
        {
            User sessionUser = new User();
            try
            {
                sessionUser = (User)Session[Common.CommonConstants.CUSTOMER_SESSION];
            }
            catch
            {
                Message.set_flash("Vui lòng đang nhập ", "danger");
                return Redirect("~/dang-nhap");
            }
            if(sessionUser == null)
            {
                Message.set_flash("Vui lòng đang nhập ", "danger");
                return Redirect("~/dang-nhap");
            }

            var listOrder = db.Orders.Where(m => m.Email == sessionUser.Email || m.Phone == sessionUser.Phone).OrderByDescending(m => m.Id).ToList();
            var listOrder1 = listOrder.Select(x => x.Code).ToList();
            var listBill = db.BillOrders.Where(x => listOrder1.Contains(x.OrderCode)).ToList();
            return View("ListOrderBill", listBill);
        }

        public ActionResult OrderDetailCus(int id)
        {
            var sigleOrder = db.Orders.Find(id);
            return View("orderDetailCus", sigleOrder);
        }
        public ActionResult GoogleLogin()
        {
            string authUrl = $"https://accounts.google.com/o/oauth2/auth?client_id={clientId}&redirect_uri={redirectUri}&response_type=code&scope=openid%20email%20profile";
            return Redirect(authUrl);
        }
        [HttpGet]
        public ActionResult GoogleCallback(string code)
        {
            // Sau khi đăng nhập thành công thì google sẽ gửi về một mã code
            // Chúng ta sẽ dùng mã code đó để lấy token
            var httpClient = new HttpClient();
            var tokenUrl = "https://accounts.google.com/o/oauth2/token";
            var tokenResponse = httpClient.PostAsync(tokenUrl, new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "code", code },
                    { "client_id", clientId },
                    { "client_secret", clientSecret },
                    { "redirect_uri", redirectUri },
                    { "grant_type", "authorization_code" }
                })).Result;

            TokenData tokenData = null;
            if (tokenResponse.IsSuccessStatusCode)
            {
                var StrtokenData = tokenResponse.Content.ReadAsStringAsync().Result;
                tokenData = JsonConvert.DeserializeObject<TokenData>(StrtokenData);
            }

            // Sau khi bạn đã có AccessToken từ TokenData Thì chúng ta sẽ gọi api lấy thông tin của user từ google
            var accessToken = tokenData.Access_token;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var userInfoResponse = httpClient.GetAsync(userInfoUrl).Result;
            if (userInfoResponse.IsSuccessStatusCode)
            {
                var userInfoData = userInfoResponse.Content.ReadAsStringAsync().Result;
                UserData userData = JsonConvert.DeserializeObject<UserData>(userInfoData);

                // Kiểm tra email tồn tại trong hệ thống chứ
                var userGoogle = db.Users.Where(x => x.Username == userData.Email).FirstOrDefault();
                var singleUser = new User();
                if(userGoogle == null)
                {
                    var muser = new User();
                    muser.Img = "defalt.png";
                    muser.Password = Mystring.ToMD5("123456");
                    muser.Username = userData.Email;
                    muser.Fullname = userData.Family_name+ " "+ userData.Given_name;
                    muser.Email = userData.Email;
                    muser.Address = "";
                    muser.Phone = null;
                    muser.Gender = "nam";
                    muser.Access = 1;
                    muser.Status = 1;
                    db.Users.Add(muser);
                    db.SaveChanges();

                    singleUser = db.Users.Where(x => x.Username == userData.Email).FirstOrDefault();
                }
                else
                {
                    singleUser = userGoogle;
                }

                Session.Add(CommonConstants.CUSTOMER_SESSION, singleUser);
                Session["userName11"] = singleUser.Fullname;
                Session["id"] = singleUser.Id;
                Session["user"] = singleUser.Username;
                if (!Response.IsRequestBeingRedirected)
                    Message.set_flash("Đăng nhập thành công ", "success");
                var id = string.IsNullOrEmpty(Session["idTricket"].ToString()) ? 0 : int.Parse(Session["idTricket"].ToString());
                if (id != 0)
                {
                    Session["idTricket"] = "";
                    return Redirect("~/Checkout/Index");
                }
                return Redirect("~/tai-khoan");
            }

            return RedirectToAction("Index", "Home"); // Chuyển hướng sau khi đăng nhập thành công.
        }
        // Hàm tạo số ngẫu nhiên
        static string GenerateOTP(int length)
        {
            Random random = new Random();
            string otp = "";

            // Tạo ngẫu nhiên 'length' số từ 0 đến 9 và nối chúng lại thành một chuỗi
            for (int i = 0; i < length; i++)
            {
                otp += random.Next(0, 10);
            }

            return otp;
        }

        [Obsolete]
        public void SendEmailAsync(string CustomerEmail, string CustomerName, string otp)
        {
            try
            {
                string[] Scopes = { GmailService.Scope.MailGoogleCom };
                UserCredential credential;

                var path = Path.Combine(Server.MapPath("~/public/UserCredential/client_secret_445635.com.json"));
                var pathUserCredential = Path.Combine(Server.MapPath("~/public/UserCredential"));

                using (FileStream stream = new FileStream(Convert.ToString(path),
                    FileMode.Open, FileAccess.Read))
                {
                    String FolderPath = Convert.ToString(pathUserCredential);
                    String FilePath = Path.Combine(FolderPath, "Credential");

                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(FilePath, true)).Result;
                }
                // Create Gmail API service.
                GmailService service = new GmailService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "DPLACE.COM",
                });


                // Tạo email
                var email = new Google.Apis.Gmail.v1.Data.Message();
                email.Raw = Convert.ToBase64String(Encoding.UTF8.GetBytes(
                    $"From: DPLACE\r\n" +
                    $"To: {CustomerEmail} <{CustomerEmail}>\r\n" +
                    $"Subject: DPLACE.COM OTP \r\n" +
                    $"\r\n" +
                    $"Xin chào {CustomerName}, DPLACE.COM vừa nhận được yêu cầu tạo tài khoản từ bạn, mã otp của bạn là: {otp}. Cảm ơn bạn đã đăng ký tài khoản tại website, Chúc bạn một ngày tốt lành!"
                ));

                // Gửi email bằng Gmail API
                service.Users.Messages.Send(email, "dplacebooking@gmail.com").ExecuteAsync();

            }
            catch (Exception ex)
            {
                var excaption = ex;
                Console.WriteLine(excaption.Message.ToString());
            }
        }

        public class TokenData
        {
            [JsonProperty("access_token")]
            public string Access_token { get; set; }
            [JsonProperty("expires_in")]
            public int Expires_in { get; set; }
            public string Scope { get; set; }
            [JsonProperty("token_type")]
            public string Token_type { get; set; }
            [JsonProperty("id_token")]
            public string Id_token { get; set; }
        }
        public class UserData
        {
            public string Sub { get; set; }
            public string Name { get; set; }
            [JsonProperty("given_name")]
            public string Given_name { get; set; }
            [JsonProperty("family_name")]
            public string Family_name { get; set; }
            public string Picture { get; set; }
            public string Email { get; set; }
            [JsonProperty("email_verified")]
            public bool Email_verified { get; set; }
            public string Locale { get; set; }
        }
    }
}