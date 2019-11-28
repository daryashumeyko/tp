using AdvertisingAgency.Models.Date;
using AdvertisingAgency.Models.ViewModels.Agency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdvertisingAgency.Areas.Admin.Controllers
{
    public class AgencyController : Controller
    {
        // GET: Admin/Agency
        public ActionResult Categories()
        {
            //Объявляем модель типа List
            List<CategoryVM> categoryVMList;

            //Подключение к дб
            using (Db db = new Db())
            {
                //Инициализируем модель данными
                categoryVMList = db.Categories.ToArray().OrderBy(x => x.Sorting).Select(x => new CategoryVM(x)).ToList();
            }

            //Возвращаем List в представление
            return View(categoryVMList);
        }

        //POST: Admin/Agency/AddNewCategory
        [HttpPost]
        public string AddNewCategory(string catName)
        {
            //Объявляем строковую переменную ID
            string id;

            //Подключение к бд
            using (Db db = new Db())
            {
                //Проверяем имя категории на уникальность
                if (db.Categories.Any(x => x.Name == catName))
                    return "titletaken";

                //Инициализируем модель DTO
                CategoryDTO dto = new CategoryDTO();

                //Добавляем данные в модель
                dto.Name = catName;
                dto.Slug = catName.Replace(" ", "-").ToLower();

                //Сохранить
                db.Categories.Add(dto);
                db.SaveChanges();

                //Получить ID чтобы вернуть в представление
                id = dto.Id.ToString();

            }

            //Возвращаем ID в представление
            return id;
        }

        //Сортировка
        // POST: Admin/Agency/ReorderCategories
        [HttpPost]
        public void ReorderCategories(int[] id)
        {
            //Подключение к бд
            using (Db db = new Db())
            {
                //Реализуем начальный счетчик
                int count = 1;

                //Инициализируем модель данных
                CategoryDTO dto;

                //Устанавливаем сортировку для каждой страницы
                foreach (var catId in id)
                {
                    dto = db.Categories.Find(catId);
                    dto.Sorting = count;
                    db.SaveChanges();
                    count++;
                }
            }
        }

        // GET: Admin/Agency/DeleteCategory/id
        public ActionResult DeleteCategory(int id)
        {
            //Подключение к бд
            using (Db db = new Db())
            {
                //Получение модели категории
                CategoryDTO dto = db.Categories.Find(id);

                //Удаление категории
                db.Categories.Remove(dto);

                //Сохранение изменений в бд
                db.SaveChanges();
            }
            //Оповещение об удалении
            TempData["M"] = "Вы успешно удалили категорию";

            //Переадресация пользователя
            return RedirectToAction("Categories");
        }
    }
}