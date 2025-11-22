using System.ComponentModel.DataAnnotations;

namespace Store.Api.DTOs.Users
{
    // --- 1. Read DTO (Used for GET requests) ---
    // Note: Excludes the sensitive 'password' field.
    public record UserDto(
        int UserId,
        string UserName,
        [EmailAddress] string Email,
        DateTime CreatedAt,
        DateTime UpdatedAt
    );
}
