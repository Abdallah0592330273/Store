namespace Store.webApi.Dtos.Product
{
    public class ProductReviewDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Title { get; set; }
        public string Body { get; set; } = string.Empty;
        public DateTime ReviewDate { get; set; }
        public bool IsVerifiedPurchase { get; set; }
    }
}
