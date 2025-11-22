using System.ComponentModel.DataAnnotations;

namespace Store.Api.DTOs.Products
{
    public record UpdateProductDto(
   [Required] int productId,
   [Required] string Name,
   string? Description,
   [Range(0.01, double.MaxValue)] decimal Price,
   int StockQuantity,
   string categoryName // Derived from the Category entity
);
}
