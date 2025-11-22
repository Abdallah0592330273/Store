using System.ComponentModel.DataAnnotations;

namespace Store.Api.DTOs.CartItems
{
    public record CreateCartItemDto(
    [Required] int ProductId,
    [Range(1, int.MaxValue)] int Quantity
    // userId is intentionally excluded; it comes from the API user's authentication context.
);
}
