using System.ComponentModel.DataAnnotations;

namespace Store.Api.DTOs.Users
{
    
    // --- 2. Registration DTO (POST /users) ---
    public record CreateUserDto(
        [Required] string UserName,
        [Required][EmailAddress] string Email,
        [Required][MinLength(6)] string Password // Password is required for creation
    );

   

}
