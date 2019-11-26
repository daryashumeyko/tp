using AdvertisingAgency.Models.Date;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdvertisingAgency.Models.ViewModels.Pages
{
    public class PageVM
    {
    //Page View Model получает все данные от DTO, разделяем БД и представление, 
    //промежуточное представление для отображения, не имеет прямых связей с БД
    //такой подход гарантирует большую безопасность чем обращение с контроллера или отсюда к БД
    
        public PageVM()   //конструктор по умолчанию, если не сработает конструктор с пааметром
        {
        }

        //Создаём конструктор с параметром
        public PageVM(PagesDTO row)    //получаем данные из DTO
        {
            Id = row.Id;
            Title = row.Title;
            Slug = row.Slug;
            Body = row.Body;
            Sorting = row.Sorting;
            HasSidebar = row.HasSidebar;
        }

        public int Id { get; set; }
        [Required]     //Заголовок обязательный
        [StringLength(50, MinimumLength = 3)]   //Длина заголовка от 3 до 50
        [Display(Name = "Название")]
        public string Title { get; set; }
        [Display(Name = "Краткое описание")]
        public string Slug { get; set; }
        [Display(Name = "Описание")]
        [Required]
        [StringLength(int.MaxValue, MinimumLength = 3)]
        [AllowHtml]
        public string Body { get; set; }
        [Display(Name = "Сортировка")]
        public int Sorting { get; set; }
        [Display(Name = "Боковое меню")]
        public bool HasSidebar { get; set; }
    }
}