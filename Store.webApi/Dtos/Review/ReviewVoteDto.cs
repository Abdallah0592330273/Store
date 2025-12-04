using System.ComponentModel.DataAnnotations;

namespace Store.webApi.Dtos.Review
{
    public class ReviewVoteDto
    {
        [Required]
        public int ReviewId { get; set; }

        [Required]
        public bool IsHelpful { get; set; } // true for helpful, false for unhelpful
    }
}
