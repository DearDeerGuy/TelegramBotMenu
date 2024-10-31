using CoreTelegramLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramLibrary.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = null!;
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<Cart> Carts { get; set; }
        public Category()
        {
            Products = new HashSet<Product>();
            Carts = new HashSet<Cart>();
        }
    }
}
