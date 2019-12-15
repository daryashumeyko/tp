using AdvertisingAgency.Models.Date;
using AdvertisingAgency.Models.ViewModels.Agency;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
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

        // POST: Admin/Agency/RenameCategory/id
        [HttpPost]
        public string RenameCategory(string newCatName, int id)
        {
            //Подключение к бд
            using (Db db = new Db())
            {
                //Проверка имени на уникальность
                if (db.Categories.Any(x => x.Name == newCatName))
                    return "titletaken";

                //Получение всех данных из бд (модель DTO)
                CategoryDTO dto = db.Categories.Find(id);

                //Редактирование модели DTO
                dto.Name = newCatName;
                dto.Slug = newCatName.Replace(" ", "-").ToLower();

                //Сохранение изменений
                db.SaveChanges();
            }

                //Возвращаем слово
                return "Ok";
        }

        //Создание метода добавления товаров
        // GET: Admin/Agency/AddProduct
        [HttpGet]
        public ActionResult AddProduct()
        {
            //Объявление модели данных
            ProductVM model = new ProductVM();

            //Добавление списка категорий из бд в модель
            using (Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), dataValueField: "id", dataTextField: "Name");
            }

            //Возвращение модели в представление
            return View(model);

        }

        //Создание метода добавления товаров
        // POST: Admin/Agency/AddProduct
        [HttpPost]
        public ActionResult AddProduct(ProductVM model, HttpPostedFileBase file)
        {
            //Проверка модели на валидность
            if (!ModelState.IsValid)
            {
                using (Db db = new Db())
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    return View(model);
                }
            }

            //Проверка имени продуктa на уникальность
            using (Db db = new Db())
            {
                if (db.Products.Any(x => x.Name == model.Name))
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    ModelState.AddModelError("", "Такое название рекламы уже существует.");
                    return View(model);
                }
            }

            //Объявление переменной ProductId
            int id;

            //Инициализация и сохранение модели на основе ProductDTO
            using (Db db = new Db())
            {
                ProductDTO product = new ProductDTO();

                product.Name = model.Name;
                product.Slug = model.Name.Replace(" ", "-").ToLower(); ;
                product.Description = model.Description;
                product.Price = model.Price;
                product.CategoryId = model.CategoryId;

                CategoryDTO catDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                product.CategoryName = catDTO.Name;

                db.Products.Add(product);
                db.SaveChanges();

                id = product.Id;
            }

            //Вывод сообщения в TempData
            TempData["M"] = "Вы добавили рекламу";

            #region Upload Image

            //Создание ссылок на директории
            var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

            var pathString1 = Path.Combine(originalDirectory.ToString(), "Products");
            var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
            var pathString3 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Small");
            var pathString4 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
            var pathString5 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Small");

            //Проверка на существование директорий, создание если нужно
            if (!Directory.Exists(pathString1))
                Directory.CreateDirectory(pathString1);

            if (!Directory.Exists(pathString2))
                Directory.CreateDirectory(pathString2);

            if (!Directory.Exists(pathString3))
                Directory.CreateDirectory(pathString3);

            if (!Directory.Exists(pathString4))
                Directory.CreateDirectory(pathString4);

            if (!Directory.Exists(pathString5))
                Directory.CreateDirectory(pathString5);

            //Проверка на загрузку файла
            if (file != null && file.ContentLength > 0)
            {
                //Получение расширения файла
                string ext = file.ContentType.ToLower();
                
                //Проверка расширения файла
                if (ext != "image/jpg" &&
                    ext != "image/png" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png")
                {
                    using (Db db = new Db())
                    {
                        model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                        ModelState.AddModelError("", "Картинка не загружена. Недопустимое расширение.");
                        return View(model);
                    }
                }

                //Объявление переменной с именем изображения
                string imageName = file.FileName;

                //Сохранение имени в модель DTO
                using (Db db = new Db())
                {
                    ProductDTO dto = db.Products.Find(id);
                    dto.ImageName = imageName;

                    db.SaveChanges();
                }

                //Назначение пути к оригинальному и уменьшенному изображению
                var path = string.Format($"{pathString2}\\{imageName}");   //к оригинальному изображению
                var path2 = string.Format($"{pathString3}\\{imageName}");  //к уменьшенному

                //Сохранение оригинального изображения
                file.SaveAs(path);

                //Создание и сохранение уменьшенной картинки
                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200).Crop(1, 1);
                img.Save(path2);
            }

            #endregion

            //Переадресовка пользователя
            return RedirectToAction("AddProduct");
        }

        //Создание метода списка товаров
        // GET: Admin/Agency/Products
        [HttpGet]
        public ActionResult Products(int? page, int? catId)    //?может быть null
        {
            //Объявление модели ProductVM типа List
            List<ProductVM> listOfProductVM;

            //Установка номера страницы
            var pageNumber = page ?? 1;   //если вернется null установится значение 1

            //Подключение к бд
            using (Db db = new Db())
            {
                //Инициализирование list и заполнение данными
                listOfProductVM = db.Products.ToArray()
                    .Where(x => catId == null || catId == 0 || x.CategoryId == catId)
                    .Select(x => new ProductVM(x))
                    .ToList();

                //Заполнение категорий данными для сортировки
                ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

                //Установка выбранной категории
                ViewBag.SelectedCat = catId.ToString();
            }

            //Установка постраничной навигации
            var onePageOfProducts = listOfProductVM.ToPagedList(pageNumber, pageSize: 5);
            ViewBag.onePageOfProducts = onePageOfProducts;

            //Возврат представления с данными
            return View(listOfProductVM);
        }

        //Создание метода редактирования товаров
        // GET: Admin/Agency/EditProduct
        [HttpGet]
        public ActionResult EditProduct(int id)
        {
            //Объявление модели ProductVM
            ProductVM model;

            using (Db db = new Db())
            {
                //Получение рекламы
                ProductDTO dto = db.Products.Find(id);

                //Проверка доступа рекламы
                if (dto == null)
                {
                    return Content("Такая реклама не существует");
                }

                //Инициализация модели данных
                model = new ProductVM(dto);

                //Создание ссписка категорий
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

                //Получение всех изображений из галереи
                model.GalleryImages = Directory
                    .EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Small"))
                    .Select(fn => Path.GetFileName(fn));
            }

            //Возврат модели в представление
            return View(model);
        }

        //Создание метода редактирования товаров
        // POST: Admin/Agency/EditProduct
        [HttpPost]
        public ActionResult EditProduct(ProductVM model, HttpPostedFileBase file)
        {
            //Получение id продукта
            int id = model.Id;

            //Заполнение списка ктегориями и изображениями
            using (Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }

            model.GalleryImages = Directory
                    .EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Small"))
                    .Select(fn => Path.GetFileName(fn));

            //Проверка модели на валидность
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Проверка имени продукта на уникальность
            using (Db db = new Db())
            {
                if (db.Products.Where(x => x.Id != id).Any(x => x.Name == model.Name))
                {
                    ModelState.AddModelError("", "Это название рекламы занято");
                    return View(model);
                }
            }

            //Обновление продукта
            using (Db db = new Db())
            {
                ProductDTO dto = db.Products.Find(id);

                dto.Name = model.Name;
                dto.Slug = model.Name.Replace(" ", "-").ToLower();
                dto.Description = model.Description;
                dto.Price = model.Price;
                dto.CategoryId = model.CategoryId;
                dto.ImageName = model.ImageName;

                CategoryDTO catDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);  //Присваивание текущей категории модели
                dto.CategoryName = catDTO.Name;

                db.SaveChanges();
            }

            //Установка сообщения в TempData
            TempData["M"] = "Вы изменили рекламу";

            //Логика обработки изображений
            #region Image Upload

            //Проверка загрузки файла
            if (file != null && file.ContentLength > 0)
            {
                //Получение расширения файла
                string ext = file.ContentType.ToLower();

                //Проверка расширения
                if (ext != "image/jpg" &&
                    ext != "image/png" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png")
                {
                    using (Db db = new Db())
                    {
                        ModelState.AddModelError("", "Картинка не загружена. Недопустимое расширение.");
                        return View(model);
                    }
                }

                //Установка путей загрузки
                var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

                var pathString1 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
                var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Small");

                //Удаление существующих путей и директорий
                DirectoryInfo di1 = new DirectoryInfo(pathString1);
                DirectoryInfo di2 = new DirectoryInfo(pathString2);

                //удаление в основной директории
                foreach (var file2 in di1.GetFiles())
                {
                    file2.Delete();
                }

                //Удаление цменьшенных изображений
                foreach (var file3 in di1.GetFiles())
                {
                    file3.Delete();
                }

                //Сохранение имени изображения
                string imageName = file.FileName;

                using (Db db = new Db())
                {
                    ProductDTO dto = db.Products.Find(id);
                    dto.ImageName = imageName;

                    db.SaveChanges();
                }

                //Сохранение оригинала и уменьшенной версии
                var path = string.Format($"{pathString1}\\{imageName}");   //к оригинальному изображению
                var path2 = string.Format($"{pathString2}\\{imageName}");  //к уменьшенному

                //Сохранение оригинального изображения
                file.SaveAs(path);

                //Создание и сохранение уменьшенной картинки
                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200).Crop(1, 1);
                img.Save(path2);
            }
            #endregion

            //Переадресация пользователя
            return RedirectToAction("EditProduct");
        }

        //Создание метода удаления товаров
        // POST: Admin/Agency/ DeleteProduct
        public ActionResult DeleteProduct(int id)
        {
            //Удаление рекламы из бд
            using (Db db = new Db())
            {
                ProductDTO dto = db.Products.Find(id);
                db.Products.Remove(dto);

                db.SaveChanges();
            }

            //Удаление директории с изображениями товара
            var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));
            var pathString = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());

            if (Directory.Exists(pathString))
                Directory.Delete(pathString, true);     //удаление папки и всех подкаталогов

            //Переадресация пользователя
            return RedirectToAction("Products");
        }

        //Создание метода добавления изображений в галерею
        // POST: Admin/Agency/SaveGalleryImages/id
        [HttpPost]
        public void SaveGalleryImages(int id)
        {
            //Перебор всех полученных файлов изображения
            foreach (string fileName in Request.Files)
            {
                //Инициализация файлов
                HttpPostedFileBase file = Request.Files[fileName];

                //Проверка на null
                if (file != null && file.ContentLength > 0)
                {
                    //Назначение путей к директориям
                    var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

                    string pathString1 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
                    string pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Small");

                    //Назаначение путей изображений
                    var path = string.Format($"{pathString1}\\{file.FileName}");
                    var path2 = string.Format($"{pathString2}\\{file.FileName}");

                    //Сохранение оригинальных изображений и уменьшенных копий
                    file.SaveAs(path);

                    WebImage img = new WebImage(file.InputStream);
                    img.Resize(200, 200).Crop(1,1);
                    img.Save(path2);
                }
            }
        }

        //Создание метода удаления изображений из галереи
        // POST: Admin/Agency/DeleteImage/id/imageName
        public void DeleteImage(int id, string imageName)
        {
            string fullPath1 = Request.MapPath("~/Images/Uploads/Products/" + id.ToString() + "/Gallery/" + imageName);
            string fullPath2 = Request.MapPath("~/Images/Uploads/Products/" + id.ToString() + "/Gallery/Small/" + imageName);

            if (System.IO.File.Exists(fullPath1))
                System.IO.File.Delete(fullPath1);

            if (System.IO.File.Exists(fullPath2))
                System.IO.File.Delete(fullPath2);
        }
    }
}