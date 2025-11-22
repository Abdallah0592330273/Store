using System;
using System.ComponentModel.DataAnnotations;

namespace Store.Api.DTOs.Reviws
{
    public record UpdateReviwDto(
        [Required]int ReviwId,
        [Required] int ProductId,
        // userId is excluded (taken from context)
        [Range(1, 5)] int Rating,
        [Required][MaxLength(500)] string Comment

        );
}
