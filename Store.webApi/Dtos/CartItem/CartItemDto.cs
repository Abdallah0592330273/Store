namespace Store.WebApi.Dtos.CartItem
{
    public class CartItemDto
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ProductImage { get; set; }
        public int Quantity { get; set; }
        public decimal PriceSnapshot { get; set; }
        public decimal TotalPrice => Quantity * PriceSnapshot;
        public DateTime DateAdded { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
