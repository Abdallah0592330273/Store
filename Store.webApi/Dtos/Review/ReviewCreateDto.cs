using System.ComponentModel.DataAnnotations;

namespace Store.WebApi.Dtos.Review
{
    public class ReviewCreateDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(200)]
        public string? Title { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Body { get; set; } = string.Empty;
    }
}
