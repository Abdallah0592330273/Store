using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Srtpre.WebApi.Dtos.Order;
using Store.DataAccess.Context;
using Store.DataAccess.Entities;
using Store.webApi.Dtos.Order;
using Store.WebApi.Controllers;
using Store.WebApi.Dtos.Order;

namespace Store.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : BaseApiController
    {
        private readonly StoreContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(
            StoreContext context,
            IMapper mapper,
            ILogger<OrdersController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: api/orders
        [HttpGet]
        public async Task<IActionResult> GetUserOrders([FromQuery] OrderFilterDto filter)
        {
            try
            {
                var userId = GetCurrentUserId();

                var query = _context.Orders
                    .Include(o => o.OrderItems)
                    .Include(o => o.Payment)
                    .Where(o => o.UserId == userId);

                if (!string.IsNullOrEmpty(filter.Status))
                {
                    query = query.Where(o => o.Status == filter.Status);
                }

                if (filter.StartDate.HasValue)
                {
                    query = query.Where(o => o.OrderDate >= filter.StartDate.Value);
                }

                if (filter.EndDate.HasValue)
                {
                    query = query.Where(o => o.OrderDate <= filter.EndDate.Value);
                }

                var totalCount = await query.CountAsync();
                var orders = await query
                    .OrderByDescending(o => o.OrderDate)
                    .Skip((filter.Page - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .ToListAsync();

                var orderDtos = _mapper.Map<List<OrderDto>>(orders);

                return Ok(new
                {
                    Orders = orderDtos,
                    TotalCount = totalCount,
                    Page = filter.Page,
                    PageSize = filter.PageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get user orders failed");
                return StatusCode(500, new { Message = "Failed to get orders." });
            }
        }

        // GET: api/orders/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .Include(o => o.Payment)
                    .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

                if (order == null)
                    return NotFound(new { Message = "Order not found." });

                var orderDto = _mapper.Map<OrderDto>(order);
                return Ok(orderDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get order failed");
                return StatusCode(500, new { Message = "Failed to get order." });
            }
        }

        // POST: api/orders/checkout
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] OrderCreateDto orderDto)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var userId = GetCurrentUserId();

                // Get user's cart
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.Status == "Active");

                if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
                    return BadRequest(new { Message = "Cart is empty." });

                // Get shipping address
                var shippingAddress = await _context.Addresses
                    .FirstOrDefaultAsync(a => a.Id == orderDto.ShippingAddressId && a.UserId == userId);

                if (shippingAddress == null)
                    return BadRequest(new { Message = "Invalid shipping address." });

                // Check stock for all items
                var outOfStockItems = new List<string>();
                foreach (var cartItem in cart.CartItems)
                {
                    if (cartItem.Product == null || cartItem.Product.StockQuantity < cartItem.Quantity)
                    {
                        outOfStockItems.Add(cartItem.Product?.Name ?? $"Product ID: {cartItem.ProductId}");
                    }
                }

                if (outOfStockItems.Any())
                {
                    return BadRequest(new
                    {
                        Message = "Some items are out of stock.",
                        OutOfStockItems = outOfStockItems
                    });
                }

                // Create order
                var order = new Order
                {
                    UserId = userId,
                    OrderDate = DateTime.UtcNow,
                    Status = "Pending",
                    ShippingAddress = $"{shippingAddress.Line1}, {shippingAddress.City}, {shippingAddress.ZipCode}, {shippingAddress.Country}",
                    ShippingMethod = orderDto.ShippingMethod,
                    Notes = orderDto.Notes,
                    CreatedDate = DateTime.UtcNow
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync(); // Save to get Order ID

                decimal subtotal = 0;
                var orderItems = new List<OrderItem>();

                // Process each cart item
                foreach (var cartItem in cart.CartItems)
                {
                    if (cartItem.Product == null) continue;

                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        UnitPriceSnapshot = cartItem.PriceSnapshot,
                        ProductNameSnapshot = cartItem.Product.Name,
                        ProductDescriptionSnapshot = cartItem.Product.Description,
                        CreatedDate = DateTime.UtcNow
                    };

                    orderItems.Add(orderItem);
                    subtotal += cartItem.Quantity * cartItem.PriceSnapshot;

                    // Reduce product stock
                    cartItem.Product.StockQuantity -= cartItem.Quantity;
                    cartItem.Product.UpdatedDate = DateTime.UtcNow;
                    _context.Products.Update(cartItem.Product);
                }

                // Calculate totals
                order.Subtotal = subtotal;
                order.TaxAmount = subtotal * 0.1m; // 10% tax example
                order.ShippingCost = orderDto.ShippingMethod == "Express" ? 15.00m : 5.00m;
                order.TotalAmount = order.Subtotal + order.TaxAmount + order.ShippingCost;

                // Update order with totals
                _context.Orders.Update(order);

                // Add order items
                _context.OrderItems.AddRange(orderItems);

                // Create payment record
                var payment = new Payment
                {
                    OrderId = order.Id,
                    Amount = order.TotalAmount,
                    Method = orderDto.PaymentMethod,
                    Status = "Pending",
                    PaymentDate = DateTime.UtcNow,
                    CreatedDate = DateTime.UtcNow
                };

                _context.Payments.Add(payment);

                // Clear cart
                _context.CartItems.RemoveRange(cart.CartItems);
                cart.Status = "Converted";
                cart.UpdatedDate = DateTime.UtcNow;
                _context.Carts.Update(cart);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Order {OrderId} created by {UserId}", order.Id, userId);

                // Return order summary
                var orderSummary = new
                {
                    OrderId = order.Id,
                    OrderNumber = $"ORD-{order.Id:00000}",
                    TotalAmount = order.TotalAmount,
                    Status = order.Status,
                    PaymentStatus = payment.Status,
                    Items = orderItems.Select(oi => new
                    {
                        oi.ProductNameSnapshot,
                        oi.Quantity,
                        oi.UnitPriceSnapshot,
                        Total = oi.Quantity * oi.UnitPriceSnapshot
                    })
                };

                return Ok(new
                {
                    Message = "Order created successfully.",
                    Order = orderSummary
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Checkout failed");
                return StatusCode(500, new { Message = "Checkout failed." });
            }
        }
    }
}