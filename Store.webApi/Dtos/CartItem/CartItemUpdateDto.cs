using System.ComponentModel.DataAnnotations;

namespace Store.WebApi.Dtos.CartItem
{
    public class CartItemUpdateDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(0, 100)]
        public int Quantity { get; set; } // 0 to remove
    }
}
