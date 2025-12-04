using System.ComponentModel.DataAnnotations;

namespace Store.WebApi.Dtos.User
{
    public class UserCreateDto
    {
        [Required]
        public string FirstName { get; set; } = null!;
        [Required]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!; // Plain text password for hashing
    }
}
