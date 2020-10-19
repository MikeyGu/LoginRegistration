using LoginRegistration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
                message = "此帳號已存在";
                ViewBag.message = message;
                return View();
            }
            else
            {

                if (p.Email != null)
                {
                    string resetCode = Guid.NewGuid().ToString();
                    SendVerificationLinkMail(p.fUserName, p.fUserId, p.fEmail, resetCode, "ResetPassword");
                    p.fResetPassword = resetCode;
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();
                    message = "已發送重置密碼連結到你的信箱";
                    ViewBag.cc = message;
                }
                else
                {
                    message = "E-mail為必填";
                    ViewBag.mail = message;
                    return View();
                }
                //if (p.fImage != null)
                //{
                //    string photoName = Guid.NewGuid().ToString() + Path.GetExtension(p.fImg.FileName);
                //    var path = Path.Combine(Server.MapPath("~/Content/"), photoName);
                //    p.fProfileImgPath = "../Content/" + photoName;
                //    p.fImg.SaveAs(path);

                //};
                if (p.fBirthday != null)
                    p.fBirthday = p.fBirthday.ToString();
                p.fProfileImgPath = "../../Content/head.png";
                p.fUpdateDate = DateTime.Now.ToShortDateString();
            }

            db.Users.Add(p);
            db.SaveChanges();



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
    }
}