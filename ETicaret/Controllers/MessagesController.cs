using ETicaret.Models;
using ETicaret.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ETicaret.Controllers
{
    public class MessagesController : Controller
    {
        ECommerceEntities db = new ECommerceEntities();
        public ActionResult Index()
        {
            if (Session["LogonUser"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var user = (Member)Session["LogonUser"];
            MessageModel model = new MessageModel();
            model.Users = new List<SelectListItem>();
            var users = db.Members.Where(x => x.IsEditor == true || x.IsAdmin == true).Where(x => x.ID != user.ID).ToList();
            model.Users = users.Select(x => new SelectListItem()
            {
                Value = x.ID.ToString(),
                Text = string.Format("{0} {1}", x.Name, x.Surname)
            }).ToList();

            var mList = db.Messages.Where(x => x.MemberToId == user.ID || x.MemberFromId == user.ID).ToList();
            model.Messages = mList;
            return View(model);
        }
        public ActionResult SendMessage(SendMessages message)
        {
            var user = (Member)Session["LogonUser"];
            if (Session["LogonUser"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            Message mesaj = new Message()
            {
                AddedDate = DateTime.Now,
                IsRead = false,
                Subject = message.Subject,
                MemberFromId = user.ID,
                MemberToId = message.ToUserId
            };
            db.Messages.Add(mesaj);

            int messageId = db.MessageReplies.OrderByDescending(x => x.AddedDate).Select(x => x.MessageId).Take(1).SingleOrDefault();

            MessageReply mr = new MessageReply()
            {
                AddedDate = DateTime.Now,
                MemberFromId = user.ID,
                Text = message.MessageBody,
                MemberToId = message.ToUserId,
            };
            if (mr.MessageId == 0)
            {
                mr.MessageId = messageId + 1;
            }
            else
            {
                mr.MessageId = 1;
            }
            db.MessageReplies.Add(mr);
            db.SaveChanges();
            return RedirectToAction("Index", "Messages");
        }

        public ActionResult MessageReplies(int id = 0)
        {
            if (Session["LogonUser"] != null)
            {
                MessageRepliesModel model = new MessageRepliesModel();
                model.MReplies = db.MessageReplies.Where(x => x.ID == id).ToList();
                bool isRead = db.MessageReplies.Where(x => x.ID == id).Select(x => x.IsRead).FirstOrDefault();
                isRead = true;
                return View(model);
            }
            return RedirectToAction("Login", "Account");
        }
        [HttpPost]
        public ActionResult MessageReplies(MessageReply message)
        {
            var user = (Member)Session["LogonUser"];
            if (user == null) return RedirectToAction("Login", "Account");

            message.AddedDate = DateTime.Now;
            message.MemberFromId = user.ID;
            db.MessageReplies.Add(message);
            db.SaveChanges(); 
            return RedirectToAction("MessageReplies", "Messages", new { id = message.MessageId });
        }
        public ActionResult RenderMessage()
        {
            RenderMessageModel model = new RenderMessageModel();
            var user = (Member)Session["LogonUser"];
            var messageList = db.Messages.Where(x=>x.MemberToId == user.ID || x.MemberFromId==user.ID).Where(x=>x.IsRead==false).ToList();
            model.Messages = messageList;
            model.Count = messageList.Count();

            return PartialView("_Message", model);
        }
        public ActionResult RemoveMessage(int id)
        {
            var msg = db.Messages.Find(id);
            db.Messages.Remove(msg);
            db.SaveChanges();
            return RedirectToAction("Index", "Messages");
        }
    }
}