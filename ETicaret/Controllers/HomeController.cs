using ETicaret.Models;
using ETicaret.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ETicaret.Controllers
{
    public class HomeController : Controller
    {
        ECommerceEntities db = new ECommerceEntities();

        public ActionResult Index(int? id)
        {
            IQueryable<Product> products = db.Products;
            Category category = null;
            if (id.HasValue)
            {
                products = products.Where(x => x.CategoryId == id);
                category = db.Categories.FirstOrDefault(x => x.ID == id);
            }
            return View(products.OrderByDescending(x => x.AddeDate).ToList());
        }
        public ActionResult Product(int id = 0)
        {
            var pro = db.Products.Where(x => x.ID == id).FirstOrDefault();
            if (pro == null) return RedirectToAction("Index", "Home");
            ProductModel model = new ProductModel()
            {
                products = pro,
                comments = db.Comments.Where(x => x.ProductId == id).ToList()
            };
            return View(model);
        }
        [HttpPost]
        public ActionResult Product(Comment comment)
        {
            try
            {
                Member user = (Member)Session["LogonUser"];
                comment.MemberId = user.ID;
                comment.AddedDate = DateTime.Now;

                db.Comments.Add(comment);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ViewBag.MyError = ex.Message;
            }

            return RedirectToAction("Product", "Home");
        }
        public ActionResult AddBasket(int id, bool remove = false)
        {
            List<BasketModel> basket = null;
            if (Session["Basket"] == null)
            {
                basket = new List<BasketModel>();
            }
            else
            {
                basket = (List<BasketModel>)Session["Basket"];
            }
            if (basket.Any(x => x.Products.ID == id))
            {
                var pro = basket.FirstOrDefault(x => x.Products.ID == id);
                if (remove && pro.Count > 0)
                {
                    pro.Count -= 1;
                }
                else
                {
                    if (pro.Products.UnitsInStock > pro.Count)
                    {
                        pro.Count += 1;
                    }
                    else
                    {
                        TempData["MyError"] = "Yeterli Stok Yok";
                    }
                }

            }
            else
            {
                var pro = db.Products.FirstOrDefault(x => x.ID == id);
                if (pro != null && pro.IsContinued && pro.UnitsInStock > 0)
                {
                    basket.Add(new BasketModel()
                    {
                        Count = 1,
                        Products = pro
                    });
                }
                else if (pro.IsContinued == false && pro == null)
                {
                    TempData["MyError"] = "Bu ürünün satışı durduruldu";
                }
            }
            basket.RemoveAll(x => x.Count < 1);
            Session["Basket"] = basket;

            return RedirectToAction("Basket", "Home");
        }
        public ActionResult Basket()
        {
            List<BasketModel> basketModel = (List<BasketModel>)Session["Basket"];
            if (basketModel == null)
            {
                basketModel = new List<BasketModel>();
            }
            var user = (Member)Session["LogonUser"];
            int userid = 0;
            if (user != null)
            {
                userid = user.ID;
                ViewBag.Addresses = db.Adresses.Where(x => x.MemberId == userid).Select(y => new SelectListItem()
                {
                    Text = y.Name + "-" + y.Description,
                    Value = y.ID.ToString()

                }).ToList();
            }

            ViewBag.TotalPrice = basketModel.Select(x => x.Products.Price * x.Count).Sum();
            ViewBag.TotalCount = basketModel.Select(x => x.Count).Sum();
            return View(basketModel);
        }
        public ActionResult RemoveBasket(int id)
        {
            List<BasketModel> basket = (List<BasketModel>)Session["Basket"];
            if (basket != null)
            {
                if (id > 0)
                {
                    basket.RemoveAll(x => x.Products.ID == id);
                }
                else if (id == 0)
                {
                    basket.Clear();
                }
                Session["Basket"] = basket;
            }
            return RedirectToAction("Basket", "Home");
        }
        [HttpGet]
        public ActionResult Buy()
        {
            if (Session["LogonUser"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {

            }
            return View();
        }
        [HttpPost]
        public ActionResult Buy(int? Address)
        {
            if (Session["LogonUser"] != null)
            {
                try
                {
                    var basket = (List<BasketModel>)Session["Basket"];
                    var user = (Member)Session["LogonUser"];
                    var order = new Order()
                    {
                        AddedDate = DateTime.Now,
                        AddressId = Address.Value,
                        MemberId = user.ID,
                        Status = "Sipariş Verildi",
                        Number = "05545507642"
                    };
                    db.Orders.Add(order);
                    foreach (BasketModel item in basket)
                    {
                        var orderDetail = new OrderDetail();
                        orderDetail.AddedDate = DateTime.Now;
                        orderDetail.Price = Convert.ToDecimal(item.Products.Price * item.Count);
                        orderDetail.OwnerUserId = user.ID;
                        orderDetail.Quantity = item.Count;
                        int OrderrId = Convert.ToInt32(db.OrderDetails.OrderByDescending(x => x.AddedDate).Select(x => x.OrderId).Take(1).SingleOrDefault());
                        if (order.ID == 0)
                        {
                            orderDetail.OrderId = OrderrId + 1;
                        }
                        else
                        {
                            orderDetail.OrderId = 1;
                        }
                        orderDetail.ProductId = item.Products.ID;

                        var _product = db.Products.Where(x => x.ID == item.Products.ID).FirstOrDefault();
                        if (_product != null && _product.UnitsInStock >= item.Count)
                        {
                            _product.UnitsInStock -= item.Count;
                        }
                        else
                        {
                            throw new Exception(string.Format("{0} ürünü için yeterli stok yoktur", item.Products.Name));
                        }
                        db.OrderDetails.Add(orderDetail);
                    }

                    db.SaveChanges();
                    Session["Basket"] = null;
                }
                catch (Exception ex)
                {
                    ViewBag.MyError = ex.Message;
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

        }
        [HttpGet]
        public JsonResult GetProductDesc(int id)
        {
            var pro = db.Products.Find(id);
            return Json(pro.Description, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreditCardInfo(int id = 0)
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreditCardInfo(string status, int id)
        {
            var order = db.Orders.Find(id);
            status = "Ödeme Yapıldı";
            order.Status = status;
            db.Entry(order).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Buy", "Home");
        }
        public ActionResult DoYouWantBeSupplier(int id = 0)
        {
            if (Session["LogonUser"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

        }
        [HttpPost]
        public ActionResult DoYouWantBeSupplier(bool? isEditor, int id, Supplier model)
        {
            var user = db.Members.Find(id);
            isEditor = true;
            user.IsEditor = isEditor;
            db.Entry(user).State = EntityState.Modified;

            model.AddedDate = DateTime.Now;
            db.Suppliers.Add(model);
            db.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Orders()
        {
            var user = (Member)Session["LogonUser"];
            if (user.IsAdmin == true)
            {
                var access = db.Orders.Where(x => x.Status == "Ödeme Yapıldı").ToList();
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
        public ActionResult CheckOrder(int id)
        {
            try
            {
                Order order = db.Orders.Find(id);
                order.Status = "Sipariş Tamamlandı";
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new
                {
                    state = 1,
                    Result = "Sipariş Onaylandı"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    state = 1,
                    Result = "Sipariş Onaylanamadı",
                    Log = ex.ToString()
                });
            }
        }

        public ActionResult BuyDetails(int id)
        {
            var order = db.OrderDetails.Where(x => x.OrderId == id).ToList();
            return View(order);
        }
    }
}
