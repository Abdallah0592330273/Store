using System.ComponentModel.DataAnnotations;

namespace Store.Api.DTOs.Caregories
{
    public record CreateCategoryDto(
     [Required] string CategoryName,
     string CategoryDescription
 );
}
