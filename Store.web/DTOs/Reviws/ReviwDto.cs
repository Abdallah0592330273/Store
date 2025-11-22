namespace Store.Api.DTOs.Reviws
{
    public record ReviewDto(
     int ReviewId,
     int ProductId,
     string UserName, // Flattened from User entity
     int Rating,
     string Comment,
     DateTime CreatedAt
 );
}
