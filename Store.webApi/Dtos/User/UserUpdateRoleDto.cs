using System.ComponentModel.DataAnnotations;

namespace Store.webApi.Dtos.User
{
    public class UserRoleUpdateDto
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty; // "Admin", "Customer", etc.
    }
}
