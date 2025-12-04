using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store.DataAccess.Context;
using Store.DataAccess.Entities;
using Store.WebApi.Controllers;
using Store.WebApi.Dtos.Category;

namespace Store.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : BaseApiController
    {
        private readonly StoreContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(
            StoreContext context,
            IMapper mapper,
            ILogger<CategoryController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: api/categories
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var categories = await _context.Categories.ToListAsync();
                var categoryDtos = _mapper.Map<List<CategoryDto>>(categories);

                // Get product count for each category
                foreach (var categoryDto in categoryDtos)
                {
                    categoryDto.ProductCount = await _context.Products
                        .CountAsync(p => p.CategoryId == categoryDto.Id && p.IsActive);
                }

                return Ok(categoryDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get all categories failed");
                return StatusCode(500, new { Message = "Failed to get categories." });
            }
        }

        // GET: api/categories/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                    return NotFound(new { Message = "Category not found." });

                var categoryDto = _mapper.Map<CategoryDto>(category);
                categoryDto.ProductCount = await _context.Products
                    .CountAsync(p => p.CategoryId == id && p.IsActive);

                return Ok(categoryDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get category failed");
                return StatusCode(500, new { Message = "Failed to get category." });
            }
        }

        // POST: api/categories
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateDto categoryDto)
        {
            try
            {
                // Check if category with same name exists
                var existingCategory = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Name.ToLower() == categoryDto.Name.ToLower().Trim());

                if (existingCategory != null)
                    return BadRequest(new { Message = "Category with this name already exists." });

                var category = _mapper.Map<Category>(categoryDto);
                category.CreatedDate = DateTime.UtcNow;

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Category {CategoryName} created by {UserId}",
                    category.Name, GetCurrentUserId());

                var createdDto = _mapper.Map<CategoryDto>(category);
                return CreatedAtAction(nameof(GetCategoryById),
                    new { id = category.Id },
                    createdDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create category failed");
                return StatusCode(500, new { Message = "Failed to create category." });
            }
        }

        // PUT: api/categories/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryUpdateDto categoryDto)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                    return NotFound(new { Message = "Category not found." });

                // Check if new name conflicts with existing category
                if (!string.IsNullOrEmpty(categoryDto.Name))
                {
                    var existingCategory = await _context.Categories
                        .FirstOrDefaultAsync(c => c.Name.ToLower() == categoryDto.Name.ToLower().Trim() && c.Id != id);

                    if (existingCategory != null)
                        return BadRequest(new { Message = "Category with this name already exists." });
                }

                _mapper.Map(categoryDto, category);
                category.UpdatedDate = DateTime.UtcNow;

                _context.Categories.Update(category);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Category {CategoryId} updated by {UserId}",
                    id, GetCurrentUserId());

                var updatedDto = _mapper.Map<CategoryDto>(category);
                return Ok(updatedDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update category failed");
                return StatusCode(500, new { Message = "Failed to update category." });
            }
        }

        // DELETE: api/categories/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                    return NotFound(new { Message = "Category not found." });

                // Check if category has products
                var productCount = await _context.Products.CountAsync(p => p.CategoryId == id);
                if (productCount > 0)
                    return BadRequest(new
                    {
                        Message = "Cannot delete category with products. Move products first."
                    });

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Category {CategoryId} deleted by {UserId}",
                    id, GetCurrentUserId());

                return Ok(new { Message = "Category deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete category failed");
                return StatusCode(500, new { Message = "Failed to delete category." });
            }
        }
    }
}