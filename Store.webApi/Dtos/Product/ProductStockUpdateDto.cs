using System.ComponentModel.DataAnnotations;

namespace Store.webApi.Dtos.Product
{
    public class ProductStockUpdateDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [MaxLength(50)]
        public string? Reason { get; set; } // "restock", "sale", "damage", "return"
    }
}
