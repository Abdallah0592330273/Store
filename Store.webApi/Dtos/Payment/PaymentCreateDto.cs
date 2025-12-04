using System.ComponentModel.DataAnnotations;

namespace Store.WebApi.Dtos.Payment
{
    public class PaymentCreateDto
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Method { get; set; } = string.Empty;

        [MaxLength(250)]
        public string? TransactionId { get; set; }

        [MaxLength(500)]
        public string? PaymentGatewayResponse { get; set; }
    }
}
