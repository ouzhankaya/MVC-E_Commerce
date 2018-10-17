using ETicaret.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ETicaret.Controllers
{
    public class CategoryController : Controller
    {
        private ECommerceEntities db = new ECommerceEntities();
        // GET: Category
        public ActionResult Index()
        {
            var user = (Member)Session["LogonUser"];
            if (user != null && user.IsAdmin == true)
            {
                var category = db.Categories.ToList();
                return View(category);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
        [HttpPost]
        public ActionResult Create(Category model)
        {
            try
            {
                db.Categories.Add(model);
                db.SaveChanges();
                return Json(new
                {
                    state = 1,
                    Result = "Kategori Eklendi",
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    state = 0,
                    Result = "Kategori Eklenemedi",
                    Log = ex.ToString(),
                });
            }

        }
        [HttpPost]
        public ActionResult Edit(Category model)
        {
            try
            {
                Category category = db.Categories.Find(model.ID);
                category.Name = model.Name;
                category.Description = model.Description;
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new
                {
                    state = 1,
                    Result = "Kategori Güncellendi."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    state = 0,
                    Result = "Kategori Güncellenirken Bir Hata ile Karşılaşıldı.",
                    Log = ex.ToString()
                });
            }
        }
    

    [HttpPost]
    public ActionResult Delete(int id)
    {
        try
        {
            var category = db.Categories.Find(id);
            db.Categories.Remove(category);
            db.SaveChanges();
            return Json(new
            {
                state = 1,
                Result = "Kategori Silindi",
            });
        }
        catch (Exception ex)
        {
            return Json(new
            {
                state = 0,
                Result = "Kategori Silinemedi",
                Log = ex.ToString(),
            });
        }

    }
}
}