using System.ComponentModel.DataAnnotations;

namespace Store.WebApi.Dtos.Product
{
    public class ProductUpdateDto
    {
        [MaxLength(250)]
        public string? Name { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal? Price { get; set; }

        [Range(0, int.MaxValue)]
        public int? StockQuantity { get; set; }

        [MaxLength(250)]
        public string? SKU { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public int? CategoryId { get; set; }

        public bool? IsActive { get; set; }
        public bool? IsFeatured { get; set; }
    }

}
