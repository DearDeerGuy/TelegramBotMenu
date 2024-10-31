using CoreTelegramLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace TelegramLibrary.Models
{
    public class StorageContext : DbContext
    {
        public DbSet<Category> Categories { get; set; } = null;
        public DbSet<Product> Products { get; set; } = null;
        public DbSet<Cart> Carts { get; set; } = null;
        public StorageContext()
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            object val = optionsBuilder.UseSqlite("Data Source = Sklad.db");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Category cat1 = new Category { Id = 1, CategoryName = "Продукты" };
            Category cat2 = new Category { Id = 2, CategoryName = "Химия" };
            Category cat3 = new Category { Id = 3, CategoryName = "Напитки" };
            Product pr1 = new Product { Id = 1, Name = "Молоко", Units = "шт.", Quantity = 75, Price = 35.30, CategoryId = 1, Description = "<a href = \"https://lactantia.ca/wp-content/uploads/2023/01/lactosefree.jpg\"> </a>" };
            Product pr2 = new Product { Id = 2, Name = "Сметана", Units = "шт.", Quantity = 65, Price = 25.20, CategoryId = 1, Description = "<a href = \"https://img2.zakaz.ua/src.1633356554.ad72436478c_2021-10-04_Tatiana/src.1633356554.SNCPSG10.obj.0.1.jpg.oe.jpg.pf.jpg.1350nowm.jpg.1350x.jpg\"> </a>" };
            Product pr3 = new Product { Id = 3, Name = "Шампунь", Units = "шт.", Quantity = 32, Price = 120.00, CategoryId = 2, Description = "<a href = \"https://u.makeup.com.ua/g/gl/gla8v5cgd3qy.png\"> </a>" };
            Product pr4 = new Product { Id = 4, Name = "Мыло", Units = "шт.", Quantity = 50, Price = 22.00, CategoryId = 2, Description = "<a href = \"https://himopt.com.ua/image/cache/catalog/image/cache/catalog/products/palmolive/656-700x700.webp\"> </a>" };
            Product pr5 = new Product { Id = 5, Name = "Латте", Units = "шт.", Quantity = 50, Price = 35.00, CategoryId = 3, Description = "<a href = \"https://agropererobka.com.ua/content/recipes/show/ice_late_tiramisu_1482001009.jpg\"> </a>" };
            Product pr6 = new Product { Id = 6, Name = "Раф", Units = "шт.", Quantity = 35, Price = 50.00, CategoryId = 3, Description = "<a href = \"https://hotcup.shop/storage/app/media/uploaded-files/1608124500020.jpeg\"> </a>" };
            modelBuilder.Entity<Category>().HasData(cat1, cat2, cat3);
            modelBuilder.Entity<Product>().HasData(pr1, pr2, pr3, pr4, pr5, pr6);
        }
    }
}
