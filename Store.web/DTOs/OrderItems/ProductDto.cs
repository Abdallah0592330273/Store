namespace Store.Api.DTOs
{
    // --- OrderItem Read DTO ---
    public record OrderItemDto(
        int OrderItemId,
        int ProductId,
        int Quantity,
        decimal Price,
        string ProductName // Include name for client display
    );
}
