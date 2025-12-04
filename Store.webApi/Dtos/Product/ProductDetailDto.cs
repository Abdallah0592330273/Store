using Store.WebApi.Dtos.Product;

namespace Store.webApi.Dtos.Product
{
    public class ProductDetailDto : ProductDto
    {
        public List<ProductReviewDto> Reviews { get; set; } = new List<ProductReviewDto>();
        public List<ProductRelatedDto> RelatedProducts { get; set; } = new List<ProductRelatedDto>();
    }
}
