using System.ComponentModel.DataAnnotations;

namespace Store.WebApi.Dtos.Order
{
    public class OrderUpdateDto
    {
        [MaxLength(50)]
        public string? Status { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }

        public string? TrackingNumber { get; set; }
        public DateTime? ShippedDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
    }
}
