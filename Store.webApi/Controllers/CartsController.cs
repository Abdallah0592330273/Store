using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store.DataAccess.Context;
using Store.DataAccess.Entities;
using Store.WebApi.Controllers;
using Store.WebApi.Dtos.Cart;
using Store.WebApi.Dtos.CartItem;

namespace Store.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : BaseApiController
    {
        private readonly StoreContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<CartController> _logger;

        public CartController(
            StoreContext context,
            IMapper mapper,
            ILogger<CartController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        private async Task<Cart> GetOrCreateCart()
        {
            var userId = GetCurrentUserId();
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.Status == "Active");

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    Status = "Active",
                    CreatedDate = DateTime.UtcNow
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }

        // GET: api/cart
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            try
            {
                var cart = await GetOrCreateCart();
                var cartDto = _mapper.Map<CartDto>(cart);

                return Ok(cartDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get cart failed");
                return StatusCode(500, new { Message = "Failed to get cart." });
            }
        }

        // POST: api/cart/items
        [HttpPost("items")]
        public async Task<IActionResult> AddToCart([FromBody] CartItemCreateDto itemDto)
        {
            try
            {
                var cart = await GetOrCreateCart();

                // Check product exists and is active
                var product = await _context.Products.FindAsync(itemDto.ProductId);
                if (product == null || !product.IsActive)
                    return NotFound(new { Message = "Product not found." });

                // Check stock
                if (product.StockQuantity < itemDto.Quantity)
                    return BadRequest(new { Message = "Insufficient stock." });

                // Check if item already in cart
                var existingItem = await _context.CartItems
                    .FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.ProductId == itemDto.ProductId);

                if (existingItem != null)
                {
                    // Update quantity
                    existingItem.Quantity += itemDto.Quantity;
                    existingItem.UpdatedDate = DateTime.UtcNow;
                    existingItem.PriceSnapshot = product.Price;

                    _context.CartItems.Update(existingItem);
                }
                else
                {
                    // Add new item
                    var cartItem = new CartItem
                    {
                        CartId = cart.Id,
                        ProductId = itemDto.ProductId,
                        Quantity = itemDto.Quantity,
                        PriceSnapshot = product.Price,
                        DateAdded = DateTime.UtcNow
                    };
                    _context.CartItems.Add(cartItem);
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Product {ProductId} added to cart by {UserId}",
                    itemDto.ProductId, GetCurrentUserId());

                return Ok(new { Message = "Item added to cart." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Add to cart failed");
                return StatusCode(500, new { Message = "Failed to add item to cart." });
            }
        }

        // PUT: api/cart/items/{productId}
        [HttpPut("items/{productId}")]
        public async Task<IActionResult> UpdateCartItem(int productId, [FromBody] CartItemUpdateDto itemDto)
        {
            try
            {
                var cart = await GetOrCreateCart();
                var cartItem = await _context.CartItems
                    .FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.ProductId == productId);

                if (cartItem == null)
                    return NotFound(new { Message = "Item not found in cart." });

                if (itemDto.Quantity <= 0)
                {
                    // Remove item if quantity is 0 or less
                    _context.CartItems.Remove(cartItem);
                }
                else
                {
                    // Check product stock
                    var product = await _context.Products.FindAsync(productId);
                    if (product == null || !product.IsActive)
                        return NotFound(new { Message = "Product not found." });

                    if (product.StockQuantity < itemDto.Quantity)
                        return BadRequest(new { Message = "Insufficient stock." });

                    cartItem.Quantity = itemDto.Quantity;
                    cartItem.UpdatedDate = DateTime.UtcNow;
                    cartItem.PriceSnapshot = product.Price;

                    _context.CartItems.Update(cartItem);
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Cart item {ProductId} updated by {UserId}",
                    productId, GetCurrentUserId());

                return Ok(new { Message = "Cart updated." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update cart item failed");
                return StatusCode(500, new { Message = "Failed to update cart." });
            }
        }

        // DELETE: api/cart/items/{productId}
        [HttpDelete("items/{productId}")]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            try
            {
                var cart = await GetOrCreateCart();
                var cartItem = await _context.CartItems
                    .FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.ProductId == productId);

                if (cartItem == null)
                    return NotFound(new { Message = "Item not found in cart." });

                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Product {ProductId} removed from cart by {UserId}",
                    productId, GetCurrentUserId());

                return Ok(new { Message = "Item removed from cart." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Remove from cart failed");
                return StatusCode(500, new { Message = "Failed to remove item from cart." });
            }
        }

        // DELETE: api/cart
        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            try
            {
                var cart = await GetOrCreateCart();

                var cartItems = await _context.CartItems
                    .Where(ci => ci.CartId == cart.Id)
                    .ToListAsync();

                if (cartItems.Any())
                {
                    _context.CartItems.RemoveRange(cartItems);
                    await _context.SaveChangesAsync();
                }

                _logger.LogInformation("Cart cleared by {UserId}", GetCurrentUserId());

                return Ok(new { Message = "Cart cleared." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Clear cart failed");
                return StatusCode(500, new { Message = "Failed to clear cart." });
            }
        }
    }
}