using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramLibrary.Models;

namespace CoreTelegramLibrary.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int ProductId { get; set; }
        public Category Category { get; set; }
        public Product Product { get; set; }
        public string UserName { get; set; } = null!;
    }
    public class ShopCart
    {
        public string user;
        public int Count { get; private set; }
        public ShopCart(string user)
        {
            this.user = user;
            using (StorageContext context = new StorageContext())
            {
                var carts = context.Carts.Where(t => t.UserName == user).Select(t => new { t.Id, t.UserName, t.Category.CategoryName, t.Product.Name }).ToList();
                Count = carts.Count;
            }
        }
        public void AddProdCart(string nameCat, string prodName)
        {
            using (StorageContext context = new StorageContext())
            {
                var category = context.Categories.FirstOrDefault(t => t.CategoryName == nameCat);
                var product = context.Products.FirstOrDefault(t => t.Name == prodName);
                Cart cart = new Cart { CategoryId = category.Id, ProductId = product.Id, UserName = user };
                var a = context.Products.Where(t => t.Category.CategoryName == nameCat && t.Name == prodName).FirstOrDefault();
                if(a.Quantity - 1 >= 0)
                {
                    context.Carts.Add(cart);
                    a.Quantity--;  
                }
                context.SaveChanges();
            }
        }
        public void DeleteAllProdCart()
        {
            using (StorageContext context = new StorageContext())
            {
                var cart = context.Carts.Where(t => t.UserName == user).ToList();
                if (cart != null)
                {
                    foreach(var item in cart)
                        context.Carts.Remove(item);
                    context.SaveChanges();
                }
            }
        }
        public void DeleteProdCart(string nameCat, string prodName)
        {
            using (StorageContext context = new StorageContext())
            {
                var category = context.Categories.FirstOrDefault(t => t.CategoryName == nameCat);
                var product = context.Products.FirstOrDefault(t => t.Name == prodName);
                Cart? cart = context.Carts.Where(t => t.UserName == user).Where(t => t.CategoryId == category.Id).Where(t => t.ProductId == product.Id).FirstOrDefault();
                if (cart != null)
                {
                    context.Carts.Remove(cart);
                    var a = context.Products.Where(t => t.Category.CategoryName == nameCat && t.Name == prodName).FirstOrDefault();
                    a.Quantity++;
                    context.SaveChanges();
                }
                Count = context.Carts.Where(t => t.UserName == user).Select(t => new { t.Id, t.UserName, t.Category.CategoryName, t.Product.Name }).Count();
            }
        }
    }
}
