using ETicaret.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ETicaret.Controllers
{
    public class ProductController : Controller
    {
        private ECommerceEntities db = new ECommerceEntities();

        // GET: Product
        public ActionResult Index()
        {
            var user = (Member)Session["LogonUser"];
            var products = db.Products.Where(x => x.OwnerUserId == user.ID).OrderByDescending(x => x.AddeDate).ToList();
            return View(products);
        }
        public ActionResult Edit(int id)
        {
            var product = db.Products.Find(id);

            var categories = db.Categories.Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.ID.ToString()
            }).ToList();
            ViewBag.Categories = categories;
            return View(product);
        }
        [HttpPost]
        public ActionResult Edit(Product model)
        {
            var user = (Member)Session["LogonUser"];
            model.AddeDate = DateTime.Now;
            model.OwnerUserId = user.ID;
            if (model.UnitsInStock > 0)
            {
                model.IsContinued = true;
            }
            model.ImageName = db.Products.Where(x => x.ID == model.ID).Select(x => x.ImageName).FirstOrDefault();
            db.Entry(model).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index", "Product");
        }
        public ActionResult Create()
        {
            var categories = db.Categories.Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.ID.ToString()
            }).ToList();
            ViewBag.Categories = categories;
            return View();
        }
        [HttpPost]
        public ActionResult Create(Product model)
        {
            var user = (Member)Session["LogonUser"];
            model.AddeDate = DateTime.Now;
            model.OwnerUserId = user.ID;
            if (model.UnitsInStock > 0)
            {
                model.IsContinued = true;
            }
            if (Request.Files != null && Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                if (file.ContentLength > 0)
                {
                    var folder = Server.MapPath("~/images");
                    var fileName = Guid.NewGuid() + ".jpg";
                    file.SaveAs(Path.Combine(folder, fileName));

                    var filePath =  fileName;
                    model.ImageName = filePath;
                }
            }

            db.Products.Add(model);
            db.SaveChanges();
            return RedirectToAction("Index", "Product");
        }
        public ActionResult Delete(int id)
        {
            try
            {
                var pro = db.Products.Find(id);
                db.Products.Remove(pro);
                db.SaveChanges();
                return Json(new
                {
                    state = 1,
                    Result = "Ürün Silindi",
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    state = 0,
                    Result = "Ürün Silinemedi",
                    Log = ex.ToString()
                });
            }
        }
    }
}