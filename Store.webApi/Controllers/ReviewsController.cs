using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store.DataAccess.Context;
using Store.DataAccess.Entities;
using Store.webApi.Dtos.Review;
using Store.WebApi.Controllers;
using Store.WebApi.Dtos.Review;

namespace Store.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReviewsController : BaseApiController
    {
        private readonly StoreContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ReviewsController> _logger;

        public ReviewsController(
            StoreContext context,
            IMapper mapper,
            ILogger<ReviewsController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: api/reviews/my
        [HttpGet("my")]
        public async Task<IActionResult> GetMyReviews()
        {
            try
            {
                var userId = GetCurrentUserId();
                var reviews = await _context.Reviews
                    .Include(r => r.Product)
                    .Where(r => r.UserId == userId)
                    .ToListAsync();

                var reviewDtos = _mapper.Map<List<ReviewDto>>(reviews);
                return Ok(reviewDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get my reviews failed");
                return StatusCode(500, new { Message = "Failed to get reviews." });
            }
        }

        // GET: api/reviews/product/{productId}
        [HttpGet("product/{productId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductReviews(int productId, [FromQuery] ReviewFilterDto filter)
        {
            try
            {
                // Check product exists
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                    return NotFound(new { Message = "Product not found." });

                var query = _context.Reviews
                    .Include(r => r.User)
                    .Where(r => r.ProductId == productId && r.Status == "Approved");

                if (filter.MinRating.HasValue)
                {
                    query = query.Where(r => r.Rating >= filter.MinRating.Value);
                }

                if (filter.MaxRating.HasValue)
                {
                    query = query.Where(r => r.Rating <= filter.MaxRating.Value);
                }

                if (filter.IsVerifiedPurchase.HasValue)
                {
                    query = query.Where(r => r.IsVerifiedPurchase == filter.IsVerifiedPurchase.Value);
                }

                // Apply sorting
                query = filter.SortBy?.ToLower() switch
                {
                    "newest" => query.OrderByDescending(r => r.ReviewDate),
                    "highest_rating" => query.OrderByDescending(r => r.Rating),
                    "lowest_rating" => query.OrderBy(r => r.Rating),
                    "most_helpful" => query.OrderByDescending(r => r.HelpfulVotes),
                    _ => query.OrderByDescending(r => r.ReviewDate)
                };

                var totalCount = await query.CountAsync();
                var reviews = await query
                    .Skip((filter.Page - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .ToListAsync();

                var reviewDtos = _mapper.Map<List<ReviewDto>>(reviews);

                // Calculate average rating
                var averageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;

                // Get rating distribution
                var ratingDistribution = reviews
                    .GroupBy(r => r.Rating)
                    .Select(g => new { Rating = g.Key, Count = g.Count() })
                    .OrderBy(x => x.Rating)
                    .ToList();

                return Ok(new
                {
                    Reviews = reviewDtos,
                    TotalCount = totalCount,
                    AverageRating = Math.Round(averageRating, 2),
                    RatingDistribution = ratingDistribution,
                    Page = filter.Page,
                    PageSize = filter.PageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get product reviews failed");
                return StatusCode(500, new { Message = "Failed to get reviews." });
            }
        }

        // POST: api/reviews
        [HttpPost]
        public async Task<IActionResult> CreateReview([FromBody] ReviewCreateDto reviewDto)
        {
            try
            {
                var userId = GetCurrentUserId();

                // Check if product exists
                var product = await _context.Products.FindAsync(reviewDto.ProductId);
                if (product == null)
                    return NotFound(new { Message = "Product not found." });

                // Check if user already reviewed this product
                var existingReview = await _context.Reviews
                    .FirstOrDefaultAsync(r => r.UserId == userId && r.ProductId == reviewDto.ProductId);

                if (existingReview != null)
                    return BadRequest(new { Message = "You have already reviewed this product." });

                var review = _mapper.Map<Review>(reviewDto);
                review.UserId = userId;
                review.Status = "Pending";
                review.ReviewDate = DateTime.UtcNow;
                review.CreatedDate = DateTime.UtcNow;

                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Review created for product {ProductId} by {UserId}",
                    reviewDto.ProductId, userId);

                return Ok(new
                {
                    Message = "Review submitted successfully.",
                    ReviewId = review.Id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create review failed");
                return StatusCode(500, new { Message = "Failed to create review." });
            }
        }
    }
}