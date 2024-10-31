using CoreTelegramLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramLibrary.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Units { get; set; } = null!;
        public int Quantity { get; set; }
        public double Price { get; set; }
        public int CategoryId { get; set; }
        public string Description { get; set; }
        public Category Category { get; set; } = null!;
        public virtual ICollection<Cart> Carts { get; set; }
        public Product() => Carts = new HashSet<Cart>();
    }
}
