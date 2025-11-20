using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Order
    {
        [Key]
        public int orderId { get; set; }
        [ForeignKey("User")]
        public int userId { get; set; }
        public User User { get; set; }
        public int totalAmount { get; set; }
        public string status { get; set; }  
        public DateTime createdDate { get; set; }

    }
}
