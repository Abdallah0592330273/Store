using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class CartItem
    {
        public int cartItemId { get; set; }
        [ForeignKey("User")]
        public int userId { get; set; }
        public User User { get; set; }
        [ForeignKey("Product")]
        public int productId { get; set; }
        public Product Product { get; set; }
        public int quantity { get; set; }
        public DateTime dateAdded { get; set; }
    }
}
