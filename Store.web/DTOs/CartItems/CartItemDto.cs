namespace Store.Api.DTOs.CartItems
{
    public record CartItemDto(
    int CartItemId,
    int ProductId,
    string ProductName, // Flattened from Product entity
    decimal ProductPrice, // Flattened from Product entity
    int Quantity,
    DateTime DateAdded
);
}
