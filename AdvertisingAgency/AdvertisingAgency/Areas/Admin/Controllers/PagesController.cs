using AdvertisingAgency.Models.Date;
using AdvertisingAgency.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdvertisingAgency.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            //Объявляем список сущностей для представления (PageVM)
            List<PageVM> PageList;

            //Инициализировать список (Db)
            using (Db db = new Db())
            {
                //Присваиваем объявленному списоку объекты из БД (все сортируем в массиве)
                PageList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            }

                //Возвращаем список в представление

                return View(PageList);
        }
    }
} 