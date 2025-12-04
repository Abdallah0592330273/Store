using System.ComponentModel.DataAnnotations;

namespace Store.WebApi.Dtos.Review
{
    public class ReviewUpdateDto
    {
        [Range(1, 5)]
        public int? Rating { get; set; }

        [MaxLength(200)]
        public string? Title { get; set; }

        [MaxLength(2000)]
        public string? Body { get; set; }

        [MaxLength(50)]
        public string? Status { get; set; }
    }
}
