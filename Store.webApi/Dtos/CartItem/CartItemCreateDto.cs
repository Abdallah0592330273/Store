using System.ComponentModel.DataAnnotations;

namespace Store.WebApi.Dtos.CartItem
{
    public class CartItemCreateDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, 100)]
        public int Quantity { get; set; } = 1;
    }
}
