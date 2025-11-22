using System.ComponentModel.DataAnnotations;

namespace Store.Api.DTOs.Users

{
    // --- 3. Update DTO (PUT/PATCH /users/{id}) ---
    // Note: Password changes should typically use a separate DTO/endpoint.
    public record UpdateUserDto(
        [Required] int UserId,
        [Required] string UserName,
        [Required][EmailAddress] string Email
    );
}
