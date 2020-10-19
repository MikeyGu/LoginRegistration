using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace LoginRegistration.Models
{
    public class SendMailFunc
    {
        public HttpRequestBase Request { get; }

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