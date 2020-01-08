using AdvertisingAgency.Models.Date;
using AdvertisingAgency.Models.ViewModels.Account;
using AdvertisingAgency.Models.ViewModels.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdvertisingAgency.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        //Просмотр корзины
        public ActionResult Index()
        {
            //Объявление list типа CartVM (если сессия пуста создаётся новый лист)
            var cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            //Проверка корзины = 0 или нет
            if (cart.Count == 0 || Session["cart"] == null)
            {
                ViewBag.Message = "Ваша корзина пуста.";
                return View();
            }

            //Сумма и ее запись во ViewBag
            int total = 0;

            foreach (var item in cart)
            {
                total += item.Total;
            }

            ViewBag.GrandTotal = total;

            //Возврат представления
            return View(cart);
        }

        public ActionResult CartPartial()
        {
            //Объявление модели CartVM
            CartVM model = new CartVM();

            //Объявление переменной количества
            int qty = 0;

            //Объявление переменной цены
            int price = 0;

            //Проверка сессии
            if (Session["cart"] != null)
            {
                //Получение общего количества товаров и цену
                var list = (List<CartVM>) Session["cart"];

                foreach (var item in list)
                {
                    qty += item.Quantity;
                    price += item.Quantity * item.Price;
                }

                model.Quantity = qty;
                model.Price = price;
            }
            else
            {
                //Или цена и кол-во равны 0
                model.Quantity = 0;
                model.Price = 0;
            }
            
            //Возврат частичного представления с моделью
            return PartialView("_CartPartial", model);
        }

        //Добавление рекламы в корзину
        public ActionResult AddToCartPartial(int id)
        {
            //Объявление List типа CartVM   (если сессия пуста создаётся новый лист)
            List<CartVM> cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            //Объявление модели CartVM
            CartVM model = new CartVM();

            using (Db db = new Db())
            {
                //Получение рекламы
                ProductDTO product = db.Products.Find(id);

                //Проверка нахождения рекламы в корзине
                var productInCart = cart.FirstOrDefault(x => x.ProductId == id);

                //Если нет, то добавление товара в корзину
                if (productInCart == null)
                {
                    cart.Add(new CartVM()
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity = 1,
                        Price = product.Price,
                        Image = product.ImageName
                    });
                }

                //Если есть, добавление единицы товара
                else
                {
                    productInCart.Quantity++;
                }
            }

            //Получение общего кол-ва рекламы, цену и добавление данных в модель
            int qty = 0;
            int price = 0;

            foreach (var item in cart)
            {
                qty += item.Quantity;
                price += item.Quantity * item.Price;
            }

            model.Quantity = qty;
            model.Price = price;

            //Сохранение состояния корзины в сессию
            Session["cart"] = cart;

            //Возврат частичного представления с моделью
            return PartialView("_AddToCartPartial", model);
        }

        //GET: /cart/IncrementProduct
        //Увеличить кол-во товаров
        public JsonResult IncrementProduct(int productId)
        {
            //Объявление list cart
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Db db = new Db())
            {
                //Получение модели CartVM из list
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                //Добавление количества
                model.Quantity++;

                //Сохранение необходимых данных
                var result = new { qty = model.Quantity, price = model.Price };

                //возврат Json ответ с данными
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        //GET: /cart/DecrementProduct
        //Уменьшить кол-во товаров
        public ActionResult DecrementProduct(int productId)
        {
            //Объявление list cart
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Db db = new Db())
            {
                //Получение модели CartVM из list
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                //Убавление количества
                if (model.Quantity > 1)
                    model.Quantity--;
                else
                {
                    model.Quantity = 0;
                    cart.Remove(model);
                }

                //Сохранение необходимых данных
                var result = new { qty = model.Quantity, price = model.Price };

                //возврат Json ответ с данными
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        //GET: /cart/RemoveProduct
        //Удалить товар из корзины
        public void RemoveProduct(int productId)
        {
            //Объявление list cart
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Db db = new Db())
            {
                //Получение модели CartVM из list
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                cart.Remove(model);
            }
        }

        //GET: /cart/order
        //Оформить заказ
        [HttpGet]
        public ActionResult Order()
        {
            //Получение имени пользователя
            string userName = User.Identity.Name;

            //Объявление модели
            UserProfileVM model;

            using (Db db = new Db())
            {
                //Получение пользователя
                UserDTO dto = db.Users.FirstOrDefault(x => x.Username == userName);

                //Инициализируем модель данными
                model = new UserProfileVM(dto);
            }
            //Возврат модели в представление
            return View("Order", model);
        }

    }
}