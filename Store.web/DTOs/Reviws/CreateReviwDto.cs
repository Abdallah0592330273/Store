using Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store.Api.DTOs.Reviws
{
    public record CreateReviwDto(

      [Required] int ProductId,
    // userId is excluded (taken from context)
    [Range(1, 5)] int Rating,
    [Required][MaxLength(500)] string Comment
        );
}
