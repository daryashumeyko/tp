using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace AdvertisingAgency.Models.Date
{
    public class Db : DbContext
        //наследуем DbContext для связи с Entity Framework
    {
        public DbSet<PagesDTO> Pages { get; set; } //связь между сущностью и БД

        public DbSet<SidebarDTO> Sidebars { get; set; } //связь c БД

        //подключение таблицы Categories
        public DbSet<CategoryDTO> Categories { get; set; }
    }
}