using LoginRegistration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Services.Description;
using System.Web.UI.WebControls;

namespace LoginRegistration.Controllers
{
    public class HomeController : Controller
    {
        MemberBaseEntities db = new MemberBaseEntities();
        public ActionResult Registration()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration(Users p )
        {
            string message = "";
            Users x = db.Users.FirstOrDefault(m => m.UserAccount == p.UserAccount);

            if(x != null)
            {
                message = "帳號已存在，如忘記密碼請點選忘記密碼";
                ViewBag.err = message;
                return View();
            }
            else
            {
                p.ActionCode = Guid.NewGuid();
                string resetCode = p.ActionCode.ToString();
                string emailFor = "VerifyAccount";
                SendVerificationLinkMail(p.UserAccount, p.Email, resetCode,emailFor);
                db.Configuration.ValidateOnSaveEnabled = false;
                
                db.Users.Add(p);
                db.SaveChanges();
                message = "註冊成功,3秒後跳回登入頁";
                ViewBag.message = message;
            }

            return View();
        }


        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Models.Login p)
        {
            string message = "";
            string err1 = "";
            var s = db.Users.Where(a => a.UserAccount == p.account).FirstOrDefault();
            if(s != null)
            {
                if(s.Password == p.userPwd)
                {
                    if (s.IsEmailConfirm != true) 
                    {
                        err1 = "Email尚未驗證";
                        ViewBag.err2 = err1;
                        return View();
                    }
                    int timeout = p.rememberMe ? 525600 : 120;
                    var ticket = new FormsAuthenticationTicket(p.account, p.rememberMe, timeout);
                    string encryted = FormsAuthentication.Encrypt(ticket);
                    var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryted);
                    cookie.Expires = DateTime.Now.AddMinutes(timeout);
                    cookie.HttpOnly = true;
                    Response.Cookies.Add(cookie);
                    return RedirectToAction("CustomList", "Member");
                }
                else
                {
                    message = "密碼錯誤";
                }
            }
            else
            {
                err1 = "無此帳號";
            }
            ViewBag.err1 = err1;
            ViewBag.err = message;
            return View();
        }
        [Authorize]
        [HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Home");
        }
        public ActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgetPassword(string account, string emailId)
        {
            string message = "";
            var v = db.Users.Where(m => m.UserAccount == account).FirstOrDefault();
            if(v != null)
            {
                if(v.Email == emailId)
                {
                    v.ActionCode = Guid.NewGuid();
                    string resetCode = v.ActionCode.ToString();
                    string emailFor = "ResetPassword";
                    SendVerificationLinkMail(v.UserAccount, v.Email, resetCode, emailFor);
                    v.ResetPwdCode = resetCode;
                    //Login.cs 密碼確認
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();
                    ViewBag.cc = "重置密碼連結已發送置你的信箱";
                    message = emailId + "，請點選信件中連結重新設定你的密碼";
                }
                else
                {
                    ViewBag.err1 = "Email不符合";
                }
            }
            else
            {
                ViewBag.err = "帳號錯誤";
            }
            ViewBag.message = message;
            return View();
        }
        public ActionResult ResetPassword(string id)
        {
            var v = db.Users.Where(m => m.ActionCode.ToString() == id);
            if(v != null)
            {
                ResetPasswprd p = new ResetPasswprd();
                p.ResetCode = id;
                return View(p);
            }
            else
            {
                return HttpNotFound();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswprd R)
        {
            string err = "";
            string message = "";
            if (ModelState.IsValid)
            {
                var v = db.Users.Where(a => a.ActionCode.ToString() == R.ResetCode).FirstOrDefault();
                if(v != null)
                {
                    v.Password = R.NewPWd;
                    v.ResetPwdCode = "";
                    v.IsEmailConfirm = true;
                    db.Configuration.ValidateOnSaveEnabled = false;             
                    db.SaveChanges();
                    message = "密碼更新成功";
                }
            }
            else
            {
                err = "錯誤的輸入";
            }
            ViewBag.err = err;
            ViewBag.message = message;
            return View(R);
        }

        [NonAction]
        public void SendVerificationLinkMail(string name, string EmailId, string activactionCode, string emailFor )
        {
            var verifyUrl = "/Home/" + emailFor + "/" + activactionCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);


            var fromEmail = new MailAddress("ecodailymanager@gmail.com", "Eco Daily");
            var toEmail = new MailAddress(EmailId);
            var fromEmailPassword = "6666Ecodaily"; //在mail中替換密碼

            string subject = "";
            string body = "";
            
            if (emailFor == "VerifyAccount")
            {
                subject = "Hi :" + name + "歡迎加入 EcoDaily";
                body = "<br/><br>Hey : <b>" + name + "</b> 這是來自 <span style='color:#28df99'> ECO DAILY </span> 發出的註冊連結 , 你的帳號 : <b>" + EmailId + "</b> 已註冊完成 , 請點以下連結設置你的密碼<br/><br/><a href='" + link + "'>" + link + "</a>";
            }
            else if (emailFor == "ResetPassword")
            {
                subject = "Reset Password";
                body = "Hi<br/><br/>我們已發送MAIL到" + EmailId + "請點選信中連結更改你的新密碼" + "<br/><br/><a href='" + link + "'>" + link + "</a>";
            }


            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }
        [NonAction]
        public bool IsEmailExist(string emailId)
        {
            var v = db.Users.Where(a => a.Email == emailId).FirstOrDefault();
            return v != null;
        }
        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            bool status = false;
            using (MemberBaseEntities db = new MemberBaseEntities())
            {
                db.Configuration.ValidateOnSaveEnabled = false;  

                var v = db.Users.Where(a => a.ActionCode == new Guid(id)).FirstOrDefault();
                if (v != null)
                {
                    v.IsEmailConfirm = true;
                    db.SaveChanges();
                    status = true;
                    ViewBag.ss = "認證成功";
                }
                else
                {
                    ViewBag.err = "連結無效";
                }

            }
            ViewBag.status = status;
            return View();
        }
    }
}