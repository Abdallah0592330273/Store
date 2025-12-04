namespace Store.WebApi.Dtos.OrderItem
{
    public class OrderItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ProductImage { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPriceSnapshot { get; set; }
        public decimal TotalPrice => Quantity * UnitPriceSnapshot;
        public string? ProductDescriptionSnapshot { get; set; }
        public DateTime CreatedDate { get; set; }
    }

}
