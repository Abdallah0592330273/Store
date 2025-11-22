using System.ComponentModel.DataAnnotations;

namespace Store.Api.DTOs
{
    // --- 3. Update Order DTO (Used for PUT/PATCH requests) ---
    // Note: Usually only status is mutable by a manager/system.
    public record UpdateOrderDto(
        [Required] int OrderId,
        [Required] string Status // e.g., 'Shipped', 'Delivered', 'Cancelled'
    );
}
