using System.ComponentModel.DataAnnotations;

namespace Store.Api.DTOs
{
    // --- 2. Create Order DTO (Used for POST requests) ---
    // Note: Client only specifies the items they want to order.
    public record CreateOrderDto(
        [Required] int UserId,
        [Required] IEnumerable<CreateOrderItemDto> Items
    );

   
}
