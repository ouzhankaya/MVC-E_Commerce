using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace ETicaret
{
    public class MyMail
    {
        public string ToMail { get; private set; }
        public string Subject { get; private set; }
        public string Body { get; private set; }

        public MyMail(string _toMail, string _subject, string _body)
        {
            this.Subject = _subject;
            this.Body = _body;
            this.ToMail = _toMail;
        }
        public void SendMail()
        {
            MailMessage mail = new MailMessage()
            {
                From = new MailAddress("scoutprojesi@gmail.com", "MyShop Sitesi")
            };
            mail.To.Add(this.ToMail);
            mail.Subject = this.Subject;
            mail.Body = this.Body;

            SmtpClient client = new SmtpClient()
            {
                Port = 587,
                Host = "smtp.gmail.com",
                EnableSsl = true
            };
            client.Credentials = new System.Net.NetworkCredential("scoutprojesi@gmail.com", "felanfilan");
            client.Send(mail);
        }
    }
}