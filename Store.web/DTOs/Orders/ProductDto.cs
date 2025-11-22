using System.ComponentModel.DataAnnotations;

namespace Store.Api.DTOs
{
    // --- 1. Order Read DTO (Used for GET requests) ---
    public record OrderDto(
        int OrderId,
        int UserId,
        string UserName, // Flattened from the User entity
        decimal TotalAmount,
        string Status,
        DateTime CreatedDate,
        IEnumerable<OrderItemDto> Items // Nested DTOs
    );

   
}
