using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Payment
    {
        [Key]
        public int paymentId { get; set; }
        [ForeignKey("Order")]
        public int orderId { get; set; }
        public Order Order { get; set; }
        public string method { get; set; }
        public decimal amount { get; set; }
        public string status { get; set; }
        public DateTime createdAt { get; set; }


    }
}
