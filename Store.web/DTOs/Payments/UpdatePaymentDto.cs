using System.ComponentModel.DataAnnotations;

namespace Store.Api.DTOs.Payments
{
    public record UpdatePaymentDto(
         [Required] int PaymentId,
         [Required] string Status // e.g., "Succeeded", "Failed", "Refunded"

   );

}
