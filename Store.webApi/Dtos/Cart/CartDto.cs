using Store.WebApi.Dtos.CartItem;

namespace Store.WebApi.Dtos.Cart
{
    // no need to have create and update DTOs for Cart as they are managed internally   in cart item operations
    public class CartDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public decimal TotalPrice { get; set; }
        public int TotalItems { get; set; }
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
    }
}
