using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Reviw
    {
        public int reviewId { get; set;}
        [ForeignKey("productId")]
        public int productId { get; set; }
        public Product Product { get; set; }
        [ForeignKey("userId")]
        public int userId { get; set; }
        public User User { get; set; }

        public int rating { get; set; }
        public string comment { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
