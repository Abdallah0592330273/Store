using System.ComponentModel.DataAnnotations;

namespace Store.WebApi.Dtos.Address
{
    public class AddressDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Line1 { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Line2 { get; set; }

        [Required]
        [MaxLength(50)]
        public string City { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string StateProvince { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string ZipCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Country { get; set; } = string.Empty;

        public bool IsShippingDefault { get; set; } = false;
        public bool IsBillingDefault { get; set; } = false;

        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
