using System.ComponentModel.DataAnnotations;

namespace Store.Api.DTOs
{
   

    // --- Create OrderItem DTO (used inside CreateOrderDto) ---
    // Note: Price is calculated by the server, not provided by the client.
    public record CreateOrderItemDto(
        [Required] int ProductId,
        [Range(1, int.MaxValue)] int Quantity
    );
}
