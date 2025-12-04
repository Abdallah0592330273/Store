using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store.DataAccess.Context;
using Store.DataAccess.Entities;
using Store.webApi.Dtos.Product;
using Store.WebApi.Controllers;
using Store.WebApi.Dtos.Product;

namespace Store.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : BaseApiController
    {
        private readonly StoreContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(
            StoreContext context,
            IMapper mapper,
            ILogger<ProductsController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: api/products
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllProducts([FromQuery] ProductFilterDto filter)
        {
            try
            {
                var query = _context.Products
                    .Include(p => p.Category)
                    .Where(p => p.IsActive);

                // Apply filters
                if (!string.IsNullOrEmpty(filter.SearchTerm))
                {
                    query = query.Where(p =>
                        p.Name.Contains(filter.SearchTerm) ||
                        (p.Description != null && p.Description.Contains(filter.SearchTerm)));
                }

                if (filter.CategoryId.HasValue)
                {
                    query = query.Where(p => p.CategoryId == filter.CategoryId.Value);
                }

                if (filter.MinPrice.HasValue)
                {
                    query = query.Where(p => p.Price >= filter.MinPrice.Value);
                }

                if (filter.MaxPrice.HasValue)
                {
                    query = query.Where(p => p.Price <= filter.MaxPrice.Value);
                }

                if (filter.InStock.HasValue && filter.InStock.Value)
                {
                    query = query.Where(p => p.StockQuantity > 0);
                }

                if (filter.IsFeatured.HasValue)
                {
                    query = query.Where(p => p.IsFeatured == filter.IsFeatured.Value);
                }

                // Apply sorting
                query = filter.SortBy?.ToLower() switch
                {
                    "price_asc" => query.OrderBy(p => p.Price),
                    "price_desc" => query.OrderByDescending(p => p.Price),
                    "newest" => query.OrderByDescending(p => p.CreatedDate),
                    "rating" => query.OrderByDescending(p => p.AverageRating ?? 0),
                    _ => query.OrderBy(p => p.Name)
                };

                // Get total count
                var totalCount = await query.CountAsync();

                // Apply pagination
                var products = await query
                    .Skip((filter.Page - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .ToListAsync();

                var productDtos = _mapper.Map<List<ProductDto>>(products);

                return Ok(new
                {
                    Products = productDtos,
                    TotalCount = totalCount,
                    Page = filter.Page,
                    PageSize = filter.PageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get all products failed");
                return StatusCode(500, new { Message = "Failed to get products." });
            }
        }

        // GET: api/products/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

                if (product == null)
                    return NotFound(new { Message = "Product not found." });

                var productDto = _mapper.Map<ProductDto>(product);

                // Get reviews for this product
                var reviews = await _context.Reviews
                    .Where(r => r.ProductId == id && r.Status == "Approved")
                    .ToListAsync();

                productDto.TotalReviews = reviews.Count;
                if (reviews.Any())
                {
                    productDto.AverageRating = (decimal?)Math.Round(reviews.Average(r => r.Rating), 2);
                }

                return Ok(productDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get product failed");
                return StatusCode(500, new { Message = "Failed to get product." });
            }
        }

        // POST: api/products
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductCreateDto productDto)
        {
            try
            {
                // Validate category exists
                var categoryExists = await _context.Categories.AnyAsync(c => c.Id == productDto.CategoryId);
                if (!categoryExists)
                    return BadRequest(new { Message = "Invalid category." });

                var product = _mapper.Map<Product>(productDto);
                product.CreatedDate = DateTime.UtcNow;

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Product {ProductName} created by {UserId}",
                    product.Name, GetCurrentUserId());

                // Load category for response
                await _context.Entry(product).Reference(p => p.Category).LoadAsync();
                var createdDto = _mapper.Map<ProductDto>(product);

                return CreatedAtAction(nameof(GetProductById),
                    new { id = product.Id },
                    createdDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create product failed");
                return StatusCode(500, new { Message = "Failed to create product." });
            }
        }

        // PUT: api/products/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductUpdateDto productDto)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                    return NotFound(new { Message = "Product not found." });

                if (productDto.CategoryId.HasValue)
                {
                    var categoryExists = await _context.Categories.AnyAsync(c => c.Id == productDto.CategoryId.Value);
                    if (!categoryExists)
                        return BadRequest(new { Message = "Invalid category." });
                    product.CategoryId = productDto.CategoryId.Value;
                }

                // Manual mapping
                if (!string.IsNullOrEmpty(productDto.Name))
                    product.Name = productDto.Name;

                if (productDto.Description != null)
                    product.Description = productDto.Description;

                if (productDto.Price.HasValue)
                    product.Price = productDto.Price.Value;

                if (productDto.StockQuantity.HasValue)
                    product.StockQuantity = productDto.StockQuantity.Value;

                if (productDto.SKU != null)
                    product.SKU = productDto.SKU;

                if (productDto.ImageUrl != null)
                    product.ImageUrl = productDto.ImageUrl;

                if (productDto.IsActive.HasValue)
                    product.IsActive = productDto.IsActive.Value;

                if (productDto.IsFeatured.HasValue)
                    product.IsFeatured = productDto.IsFeatured.Value;

                product.UpdatedDate = DateTime.UtcNow;

                _context.Products.Update(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Product {ProductId} updated by {UserId}",
                    id, GetCurrentUserId());

                // Load category for response
                await _context.Entry(product).Reference(p => p.Category).LoadAsync();
                var updatedDto = _mapper.Map<ProductDto>(product);

                return Ok(updatedDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update product failed");
                return StatusCode(500, new { Message = "Failed to update product." });
            }
        }

        // DELETE: api/products/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                    return NotFound(new { Message = "Product not found." });

                // Soft delete
                product.IsActive = false;
                product.UpdatedDate = DateTime.UtcNow;

                _context.Products.Update(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Product {ProductId} deactivated by {UserId}",
                    id, GetCurrentUserId());

                return Ok(new { Message = "Product deactivated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete product failed");
                return StatusCode(500, new { Message = "Failed to delete product." });
            }
        }
    }
}