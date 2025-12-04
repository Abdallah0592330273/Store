using System.ComponentModel.DataAnnotations;

namespace Store.WebApi.Dtos.User
{
    public class UserUpdateDto
    {
        [MaxLength(100)]
        public string? FirstName { get; set; }

        [MaxLength(100)]
        public string? LastName { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }
    }


}
