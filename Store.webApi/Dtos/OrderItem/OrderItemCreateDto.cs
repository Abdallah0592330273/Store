using System.ComponentModel.DataAnnotations;

namespace Store.webApi.Dtos.OrderItem
{
    public class OrderItemCreateDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, 100)]
        public int Quantity { get; set; }
    }

}
