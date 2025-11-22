using System.ComponentModel.DataAnnotations;

namespace Store.Api.DTOs.Products
{
    public record CreateProductDto(
   [Required] int ProductId,
   [Required] string Name,
   string Description,
   [Range(0.01, double.MaxValue)] decimal Price,
   int StockQuantity,
   string CategoryName // Derived from the Category entity
);
}
