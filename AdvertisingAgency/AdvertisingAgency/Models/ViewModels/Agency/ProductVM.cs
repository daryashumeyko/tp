using AdvertisingAgency.Models.Date;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdvertisingAgency.Models.ViewModels.Agency
{
    public class ProductVM
    {
        //Конструктор без параметра по умолчанию
        public ProductVM() { }

        //Конструктор с параметром
        public ProductVM(ProductDTO row)
        {
            Id = row.Id;
            Name = row.Name;
            Slug = row.Slug;
            Description = row.Description;
            Price = row.Price;
            CategoryName = row.CategoryName;
            ImageName = row.ImageName;
        }

        public int Id { get; set; }
        [Required]   //Поле обязательно
        [DisplayName("Название")]
        public string Name { get; set; }
        public string Slug { get; set; }
        [Required]
        [DisplayName("Описание")]
        public string Description { get; set; }
        [DisplayName("Цена")]
        public int Price { get; set; }
        public string CategoryName { get; set; }
        [Required]
        [DisplayName("Категория")]
        public int CategoryId { get; set; }
        public string ImageName { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }
        public IEnumerable<string> GalleryImages { get; set; }
    }
}