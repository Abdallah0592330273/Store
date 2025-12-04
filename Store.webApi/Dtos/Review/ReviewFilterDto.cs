namespace Store.webApi.Dtos.Review
{
    public class ReviewFilterDto
    {
        public int? ProductId { get; set; }
        public string? UserId { get; set; }
        public int? MinRating { get; set; }
        public int? MaxRating { get; set; }
        public bool? IsVerifiedPurchase { get; set; }
        public string? Status { get; set; }
        public string? SortBy { get; set; } // "newest", "highest_rating", "lowest_rating", "most_helpful"
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
