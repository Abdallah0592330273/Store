using System.ComponentModel.DataAnnotations;

namespace Store.WebApi.Dtos.Payment
{
    public class PaymentUpdateDto
    {
        [MaxLength(50)]
        public string? Status { get; set; }

        [MaxLength(250)]
        public string? TransactionId { get; set; }

        [MaxLength(500)]
        public string? PaymentGatewayResponse { get; set; }
    }
}
