using AdvertisingAgency.Models.Date;
using System.ComponentModel.DataAnnotations;

namespace AdvertisingAgency.Models.ViewModels.Agency
{
    public class CategoryVM
    {
        public CategoryVM()   //конструктор по умолчанию
        {
        }

        public CategoryVM(CategoryDTO row)   //конструктор с параметрами
        {
            Id = row.Id;
            Name = row.Name;
            Slug = row.Slug;
            Sorting = row.Sorting;
        }

        public int Id { get; set; }
        [Display(Name = "Название")]
        public string Name { get; set; }
        public string Slug { get; set; }
        public int Sorting { get; set; }
    }
}