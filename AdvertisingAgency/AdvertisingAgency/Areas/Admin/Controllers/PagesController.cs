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

        // GET: Admin/Pages/AddPage
        [HttpPost]
        public ActionResult AddPage()
        {
            return View();
        }

        // POST: Admin/Pages/AddPage
        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
            //Проверка модели на валидность
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                //Объявляем переменную для краткого описания (slug)
                string slug;

                //Инициализируем класс PageDTO
                PagesDTO dto = new PagesDTO();

                //Присваиваем заголовок модели (c большой буквы)
                dto.Title = model.Title.ToUpper();

                //Проверяем есть ли краткое описание, если нет - присваиваем описанию название с мал.буквы
                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }

                //Прверяем, что заголовок и краткое описание уникальны
                if (db.Pages.Any(x => x.Title == model.Title))
                {
                    ModelState.AddModelError("", "Этот заголовок уже существует.");
                    return View(model);
                }
                else if (db.Pages.Any(x => x.Slug == model.Slug))
                {
                    ModelState.AddModelError("", "Это описание уже существует.");
                    return View(model);
                }

                //Присваиваем оставшиеся значения модели
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                dto.Sorting = 100;

                //Сохраняем модель в БД
                db.Pages.Add(dto);
                db.SaveChanges();
            }

            //Передам сообщение через TenyData (Временные данные)
            TempData["M"] = "Вы добавили новую страницу:";

            //Переадресовываем пользователя на страницу Index
            return RedirectToAction("Index");
        }
    }
} 