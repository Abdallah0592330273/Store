namespace Store.webApi.Dtos.Payment
{
    public class PaymentIntentDto
    {
        public string ClientSecret { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string? PaymentMethod { get; set; }
    }
}
