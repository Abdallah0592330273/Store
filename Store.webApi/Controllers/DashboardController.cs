//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Store.DataAccess.Entities;
//using Store.DataAccess.UnitOfWork;
//using Store.WebApi.Controllers;

//namespace Store.WebApi.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    [Authorize(Roles = "Admin")]
//    public class DashboardController : BaseApiController
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly UserManager<ApplicationUser> _userManager;
//        private readonly ILogger<DashboardController> _logger;

//        public DashboardController(
//            IUnitOfWork unitOfWork,
//            UserManager<ApplicationUser> userManager,
//            ILogger<DashboardController> logger)
//        {
//            _unitOfWork = unitOfWork;
//            _userManager = userManager;
//            _logger = logger;
//        }

//        // GET: api/dashboard/stats
//        [HttpGet("stats")]
//        public async Task<IActionResult> GetDashboardStats([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
//        {
//            try
//            {
//                var now = DateTime.UtcNow;
//                var thirtyDaysAgo = now.AddDays(-30);

//                startDate = startDate ?? thirtyDaysAgo;
//                endDate = endDate ?? now;

//                // Total sales - get all orders first
//                var allOrders = await _unitOfWork.OrderRepo.GetAllAsync();
//                var orders = allOrders
//                    .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
//                    .ToList();

//                var totalSales = orders.Sum(o => o.TotalAmount);

//                // Order counts
//                var totalOrders = orders.Count;
//                var pendingOrders = orders.Count(o => o.Status == "Pending");
//                var completedOrders = orders.Count(o => o.Status == "Delivered");

//                // Product stats
//                var allProducts = await _unitOfWork.ProductRepo.GetAllAsync();
//                var totalProducts = allProducts.Count(p => p.IsActive);
//                var lowStockProducts = allProducts.Count(p => p.IsActive && p.StockQuantity < 10);
//                var outOfStockProducts = allProducts.Count(p => p.IsActive && p.StockQuantity == 0);

//                // User stats
//                var allUsers = await _userManager.Users.ToListAsync();
//                var totalUsers = allUsers.Count;
//                var newUsers = allUsers.Count(u => u.CreatedDate >= startDate && u.CreatedDate <= endDate);

//                // Recent orders
//                var recentOrders = orders
//                    .OrderByDescending(o => o.OrderDate)
//                    .Take(10)
//                    .ToList();

//                // Get users for recent orders
//                var recentOrderDetails = new List<object>();
//                foreach (var order in recentOrders)
//                {
//                    var user = allUsers.FirstOrDefault(u => u.Id == order.UserId);
//                    recentOrderDetails.Add(new
//                    {
//                        order.Id,
//                        OrderNumber = $"ORD-{order.Id:00000}",
//                        order.OrderDate,
//                        order.TotalAmount,
//                        order.Status,
//                        Customer = user != null ? $"{user.FirstName} {user.LastName}" : "Unknown"
//                    });
//                }

//                // Top selling products
//                var allOrderItems = await _unitOfWork.OrderItemRepo.GetAllAsync();
//                var filteredOrderItems = allOrderItems
//                    .Where(oi => orders.Any(o => o.Id == oi.OrderId))
//                    .ToList();

//                var topProducts = filteredOrderItems
//                    .GroupBy(oi => new { oi.ProductId, oi.ProductNameSnapshot })
//                    .Select(g => new
//                    {
//                        ProductId = g.Key.ProductId,
//                        ProductName = g.Key.ProductNameSnapshot,
//                        TotalSold = g.Sum(oi => oi.Quantity),
//                        Revenue = g.Sum(oi => oi.Quantity * oi.UnitPriceSnapshot)
//                    })
//                    .OrderByDescending(x => x.TotalSold)
//                    .Take(10)
//                    .ToList();

//                return Ok(new
//                {
//                    Period = new { StartDate = startDate, EndDate = endDate },
//                    Sales = new
//                    {
//                        Total = totalSales,
//                        AverageOrderValue = totalOrders > 0 ? totalSales / totalOrders : 0
//                    },
//                    Orders = new
//                    {
//                        Total = totalOrders,
//                        Pending = pendingOrders,
//                        Completed = completedOrders,
//                        CompletionRate = totalOrders > 0 ? (double)completedOrders / totalOrders * 100 : 0
//                    },
//                    Products = new
//                    {
//                        Total = totalProducts,
//                        LowStock = lowStockProducts,
//                        OutOfStock = outOfStockProducts
//                    },
//                    Users = new
//                    {
//                        Total = totalUsers,
//                        New = newUsers
//                    },
//                    RecentOrders = recentOrderDetails,
//                    TopProducts = topProducts
//                });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Get dashboard stats failed");
//                return StatusCode(500, new { Message = "Failed to get dashboard stats." });
//            }
//        }
//    }
//}