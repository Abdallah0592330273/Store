using System.ComponentModel.DataAnnotations;

namespace Srtpre.WebApi.Dtos.Order
{
    public class OrderCreateDto
    {
        [Required]
        public int ShippingAddressId { get; set; }

        [Required]
        [MaxLength(50)]
        public string PaymentMethod { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? ShippingMethod { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }

        public bool UseShippingAsBilling { get; set; } = true;
        public int? BillingAddressId { get; set; }
    }
}
