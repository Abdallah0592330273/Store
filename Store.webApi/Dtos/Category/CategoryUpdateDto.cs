using Store.WebApi.Dtos.Product;
using System.ComponentModel.DataAnnotations;

namespace Store.WebApi.Dtos.Category
{
    public class CategoryUpdateDto
    {
        [MaxLength(100)]
        public string? Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }
    }

    public class CategoryWithProductsDto : CategoryDto
    {
        public List<ProductDto> Products { get; set; } = new List<ProductDto>();
    }
}
