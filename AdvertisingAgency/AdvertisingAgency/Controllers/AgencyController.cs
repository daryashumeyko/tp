using AdvertisingAgency.Models.Date;
using AdvertisingAgency.Models.ViewModels.Agency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdvertisingAgency.Controllers
{
    public class AgencyController : Controller
    {
        // GET: Agency
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Pages");
        }

        // GET: вывод категорий рекламы в меню
        public ActionResult CategoryMenuPartial()
        {
            //Объявление модели типа list CategoryVM
            List<CategoryVM> categoryVMList;

            //Инициализация модели данными
            using (Db db = new Db())
            {
                categoryVMList = db.Categories.ToArray().OrderBy(x => x.Sorting).Select(x => new CategoryVM(x)).ToList();
            }

            //Возврат частичного представления с моделью
            return PartialView("_CategoryMenuPartial", categoryVMList);
        }

        // GET: Agency/Category/name
        public ActionResult Category(string name)
        {
            //Объявление списка типа list
            List<ProductVM> productVMList;

            using (Db db = new Db())
            {
                //Получение id категории
                CategoryDTO categoryDTO = db.Categories.Where(x => x.Slug == name).FirstOrDefault();
                int catId = categoryDTO.Id;

                //Инициализация списка данными
                productVMList = db.Products.ToArray().Where(x => x.CategoryId == catId).Select(x => new ProductVM(x)).ToList();

                //Получение имени категории
                var productCat = db.Products.Where(x => x.CategoryId == catId).FirstOrDefault();

                //Проверка на null
                if (productCat == null)
                {
                    var catName = db.Categories.Where(x => x.Slug == name).Select(x => x.Name).FirstOrDefault();
                    ViewBag.Categoryname = catName;
                }
                else
                {
                    ViewBag.CategoryName = productCat.CategoryName;
                }
            }

            //Возврат представления с моделью
            return View(productVMList);
        }
    }
}