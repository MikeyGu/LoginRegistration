using LoginRegistration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace LoginRegistration.Controllers
{
    public class MemberController : Controller
    {
        MemberBaseEntities db = new MemberBaseEntities();
        // GET: Member
        [Authorize]
        public ActionResult HomeMember()
        {
            return View();
        }
        public ActionResult CustomList()
        {
            string message = "";
            IQueryable<Users> memberT = null;
            string keyword = Request.Form["txtKeyWord"];
            if (string.IsNullOrEmpty(keyword))
            {
                memberT = from p in (new MemberBaseEntities()).Users select p;
                         
            }
            else
            {
                memberT = from p in (new MemberBaseEntities()).Users
                          where p.UserAccount.Contains(keyword)
                          select p;
            }

            return View(memberT);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Users p)
        {
            string message = "";
            Users x = db.Users.FirstOrDefault(m => m.UserAccount == p.UserAccount);

            if (x != null)
            {
                message = "帳號已存在";
                ViewBag.err = message;
                return View();
            }
            else
            {
                p.ActionCode = Guid.NewGuid();
                string resetCode = p.ActionCode.ToString();
                string emailFor = "ResetPassword";
                SendVerificationLinkMail(p.UserAccount, p.Email, resetCode, emailFor);
                db.Configuration.ValidateOnSaveEnabled = false;

                db.Users.Add(p);
                db.SaveChanges();
                message = "註冊成功,3秒後跳回登入頁";
                ViewBag.message = message;
            }

            return RedirectToAction("CustomList");

        }
        public ActionResult Edit(string id)
        {
            string message = "";
            if (id == null)
            {
                message = "查無此會員資料";
                ViewBag.message = message;
                return RedirectToAction("CustomList");
            }
            message = id;
            ViewBag.message = message;
            Users x = db.Users.FirstOrDefault(m => m.UserAccount == id);
            return View(x);
        }
        [HttpPost]
        public ActionResult Edit(Users p)
        {
            Users cust = db.Users.FirstOrDefault(m => m.UserAccount == p.UserAccount);
            if (cust != null)
            {
                
                cust.FirstName = p.FirstName;
                cust.LastName = p.LastName;
                cust.Birthday = p.Birthday;
                cust.Email = p.Email;
                cust.Password = p.Password;
                //if (file != null) 添加圖片用
                //{
                //    string phoneName = Guid.NewGuid().ToString() + Path.GetFileName(file.FileName);
                //    var path = Path.Combine(Server.MapPath("~/Content/"), phoneName);
                //    file.SaveAs(path);
                //    cust.fProfileImgPath = "../Content/" + phoneName;
                //}
                db.SaveChanges();
            }

            return RedirectToAction("CustomList");
        }
        public ActionResult Details(string id)
        {
            Users x = db.Users.FirstOrDefault(m => m.UserAccount == id);
            return View(x);
        }
        public ActionResult Delete(string id)
        {
            if (id == null)
                return RedirectToAction("CustomList");

            Users x = db.Users.FirstOrDefault(m => m.UserAccount == id);
            //刪除關聯資料
            //var y = db.tForumComment.Where(m => m.fUserId == id).ToList();
            //db.tForumComment.RemoveRange(y);
            if (x != null)
            {
                db.Users.Remove(x);
                db.SaveChanges();
            }
            return RedirectToAction("CustomList");
        }
        [NonAction]
        public void SendVerificationLinkMail(string name, string EmailId, string activactionCode, string emailFor)
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
    }
}