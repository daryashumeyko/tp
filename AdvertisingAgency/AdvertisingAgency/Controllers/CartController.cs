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
        public ActionResult Index()
        {
            //Объявление list типа CartVM (если сессия пуст асоздаётся новый лист
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
                var list = (List<CartVM>)Session["cart"];

                foreach (var item in list)
                {
                    qty += item.Quantity;
                    price += item.Quantity * item.Price;
                }
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
    }
}