using System.ComponentModel.DataAnnotations;

namespace Store.WebApi.Dtos.Address
{
    public class AddressUpdateDto
    {
        [MaxLength(100)]
        public string? Line1 { get; set; }

        [MaxLength(100)]
        public string? Line2 { get; set; }

        [MaxLength(50)]
        public string? City { get; set; }

        [MaxLength(50)]
        public string? StateProvince { get; set; }

        [MaxLength(20)]
        public string? ZipCode { get; set; }

        [MaxLength(50)]
        public string? Country { get; set; }

        public bool? IsShippingDefault { get; set; }
        public bool? IsBillingDefault { get; set; }
    }
}
