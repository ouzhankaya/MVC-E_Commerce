using ETicaret.Models;
using ETicaret.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ETicaret.Controllers
{
    public class AccountController : Controller
    {
        ECommerceEntities db = new ECommerceEntities();
        // GET: Account
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(Register model)
        {
            try
            {
                if (model.member.Password != model.rePassword)
                {
                    throw new Exception("Şifre ile Şifre Tekrar Uyuşmuyor");
                }
                if (db.Members.Any(x => x.Email == model.member.Email))
                {
                    throw new Exception("Email Adresi Kayıtlı");
                }
                model.member.IsAdmin = false;
                model.member.IsEditor = false;
                model.member.AddedDate = DateTime.Now;
                db.Members.Add(model.member);
                db.SaveChanges();
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                ViewBag.ReError = ex.Message;
                return View();
            }
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(Member model)
        {
            try
            {
                var user = db.Members.FirstOrDefault(x => x.Password == model.Password && x.Email == model.Email);
                if (user != null)
                {
                    Session["LogonUser"] = user;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.ReError = "Kullanıcı bilgileriniz Yanlış";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ReError = ex.Message;
            }
            return View();
        }
        public ActionResult Logout()
        {
            Session["LogonUser"] = null;
            return RedirectToAction("Login", "Account");
        }
        public ActionResult Profil(int id = 0)
        {
            List<Adress> addresses = null;
            var member = (Member)Session["LogonUser"];

            if (id != 0)
            {
                //id = member.ID;
                addresses = db.Adresses.Where(x => x.MemberId == id).ToList();
            }
            var user = db.Members.Where(x => x.ID == id).FirstOrDefault();
            if (user == null) return RedirectToAction("Index", "Home");
            ProfilModel model = new ProfilModel()
            {
                member = user,
                adress = addresses
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult EditProfil(Member model)
        {
            try
            {
                var updateMember = db.Members.Find(model.ID);
                updateMember.ModifiedDate = DateTime.Now;
                updateMember.Bio = model.Bio;
                updateMember.Name = model.Name;
                updateMember.Surname = model.Surname;
                if (string.IsNullOrEmpty(model.Password) == false)
                {
                    updateMember.Password = model.Password;
                }


                db.SaveChanges();


                return RedirectToAction("Profil", "Account");
            }
            catch (Exception ex)
            {
                ViewBag.Myrror = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public ActionResult Address(Adress adress)
        {
            adress.AddedDate = DateTime.Now;
            var member = (Member)Session["LogonUser"];
            adress.MemberId = member.ID;
            db.Adresses.Add(adress);
            db.SaveChanges();
            return RedirectToAction("Profil", "Account");
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            var member = db.Members.FirstOrDefault(x => x.Email == email);
            if (member == null)
            {
                ViewBag.MyError = "Böyle bir email hesabı bulunamadı";
                return View();
            }
            else
            {
                var body = "Şifreniz:" + member.Password;
                MyMail mail = new MyMail(member.Email, "Şifremi Unuttum", body);
                mail.SendMail();
                TempData["Info"] = email + " mail adresinize şifre gönderilmiştir.";
                return RedirectToAction("Login");
            }
            
        }
    }
}