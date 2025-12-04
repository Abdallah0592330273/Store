namespace Store.webApi.Dtos.Product
{
    public class ProductFilterDto
    {
        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool? InStock { get; set; }
        public bool? IsFeatured { get; set; }
        public string? SortBy { get; set; } // "price_asc", "price_desc", "newest", "rating"
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
