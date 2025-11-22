using System.ComponentModel.DataAnnotations;

namespace Store.Api.DTOs.CartItems
{
    public record UpdateCartItemDto(
     [Required] int CartItemId,
     [Range(1, int.MaxValue)] int Quantity // Only quantity is updated
        );
}
