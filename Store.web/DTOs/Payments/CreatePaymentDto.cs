using System.ComponentModel.DataAnnotations;

namespace Store.Api.DTOs.Payments
{
   
    public record CreatePaymentDto(
          [Required] int OrderId,
          [Required] string PaymentMethod // e.g., "CreditCard", "PayPal"
    // The Amount will be calculated server-side based on the OrderId


   );
}
