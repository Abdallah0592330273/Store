namespace Store.webApi.Dtos.Product
{
    public class ProductRelatedDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public decimal? AverageRating { get; set; }
    }
}
