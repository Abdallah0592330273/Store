using System.ComponentModel.DataAnnotations;

namespace Store.Api.DTOs.Caregories
{
    public record UpdateCategoryDto(
      [Required] int CategoryId,
      [Required] string CategoryName,
      string CategoryDescription
  );
}
