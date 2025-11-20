using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class OrderItem
    {
        [Key]
        public int orderItemId { get; set; }
        [ForeignKey("Order")]
        public int orderId { get; set; }
        public Order Order { get; set; }
        [ForeignKey("Product")]
        public int productId { get; set; }
        public Product Product { get; set; }
        public int quantity { get; set; }
        public decimal price { get; set; }
    }
}
