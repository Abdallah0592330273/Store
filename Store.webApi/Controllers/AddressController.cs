using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store.DataAccess.Context;
using Store.DataAccess.Entities;
using Store.WebApi.Dtos.Address;

namespace Store.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AddressController : BaseApiController
    {
        private readonly StoreContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<AddressController> _logger;

        public AddressController(
            StoreContext context,
            IMapper mapper,
            ILogger<AddressController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

     

       

        // GET: api/address
        [HttpGet]
        public async Task<IActionResult> GetUserAddresses()
        {
            try
            {
                var userId = GetCurrentUserId();
                var email = GetCurrentUserEmail();
                var roles = GetCurrentUserRoles();

                // Log for debugging
                Console.WriteLine($"\n=== GET ADDRESSES ===");
                Console.WriteLine($"User ID: {userId}");
                Console.WriteLine($"Email: {email}");
                Console.WriteLine($"Roles: {string.Join(", ", roles)}");
                Console.WriteLine($"Is Authenticated: {User.Identity?.IsAuthenticated}");
                Console.WriteLine($"Auth Type: {User.Identity?.AuthenticationType}");
                Console.WriteLine("=====================\n");

                var addresses = await _context.Addresses
                    .Where(a => a.UserId == userId)
                    .OrderBy(a => a.IsShippingDefault)
                    .ThenBy(a => a.IsBillingDefault)
                    .ThenBy(a => a.CreatedDate)
                    .ToListAsync();

                Console.WriteLine($"Found {addresses.Count} addresses for user {userId}");

                var addressDtos = _mapper.Map<List<AddressDto>>(addresses);

                return Ok(new
                {
                    Success = true,
                    Count = addressDtos.Count,
                    UserId = userId,
                    Email = email,
                    Addresses = addressDtos,
                    Message = addresses.Count > 0
                        ? $"Found {addresses.Count} addresses"
                        : "No addresses found"
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt to addresses");
                return Unauthorized(new
                {
                    Success = false,
                    Message = "Authentication failed",
                    Error = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get user addresses failed");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Failed to get addresses.",
                    Error = ex.Message
                });
            }
        }

        // GET: api/address/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAddressById(int id)
        {
            try
            {
                var userId = GetCurrentUserId();

                Console.WriteLine($"\n=== GET ADDRESS BY ID ===");
                Console.WriteLine($"User ID: {userId}");
                Console.WriteLine($"Address ID: {id}");
                Console.WriteLine("=========================\n");

                var address = await _context.Addresses
                    .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

                if (address == null)
                {
                    Console.WriteLine($"Address {id} not found for user {userId}");
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Address not found.",
                        UserId = userId,
                        AddressId = id
                    });
                }

                var addressDto = _mapper.Map<AddressDto>(address);

                return Ok(new
                {
                    Success = true,
                    Address = addressDto,
                    Message = "Address found successfully"
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    Success = false,
                    Message = "Authentication failed",
                    Error = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get address by ID failed");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Failed to get address.",
                    Error = ex.Message
                });
            }
        }

        // POST: api/address
        [HttpPost]
        public async Task<IActionResult> CreateAddress([FromBody] AddressCreateDto addressDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var email = GetCurrentUserEmail();

                Console.WriteLine($"\n=== CREATE ADDRESS ===");
                Console.WriteLine($"User ID: {userId}");
                Console.WriteLine($"Email: {email}");
                Console.WriteLine($"Address Line1: {addressDto.Line1}");
                Console.WriteLine($"IsShippingDefault: {addressDto.IsShippingDefault}");
                Console.WriteLine($"IsBillingDefault: {addressDto.IsBillingDefault}");
                Console.WriteLine("======================\n");

                // If setting as default shipping, update existing defaults
                if (addressDto.IsShippingDefault)
                {
                    var existingShippingDefaults = await _context.Addresses
                        .Where(a => a.UserId == userId && a.IsShippingDefault)
                        .ToListAsync();

                    foreach (var addr in existingShippingDefaults)
                    {
                        addr.IsShippingDefault = false;
                        _context.Addresses.Update(addr);
                    }
                }

                // If setting as default billing, update existing defaults
                if (addressDto.IsBillingDefault)
                {
                    var existingBillingDefaults = await _context.Addresses
                        .Where(a => a.UserId == userId && a.IsBillingDefault)
                        .ToListAsync();

                    foreach (var addr in existingBillingDefaults)
                    {
                        addr.IsBillingDefault = false;
                        _context.Addresses.Update(addr);
                    }
                }

                var address = _mapper.Map<Address>(addressDto);
                address.UserId = userId;
                address.CreatedDate = DateTime.UtcNow;

                _context.Addresses.Add(address);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Address created by {UserId} ({Email})", userId, email);
                Console.WriteLine($"✓ Address created with ID: {address.Id}");

                var createdDto = _mapper.Map<AddressDto>(address);

                return CreatedAtAction(nameof(GetAddressById),
                    new { id = address.Id },
                    new
                    {
                        Success = true,
                        Address = createdDto,
                        Message = "Address created successfully",
                        CreatedId = address.Id
                    });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    Success = false,
                    Message = "Authentication failed",
                    Error = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create address failed");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Failed to create address.",
                    Error = ex.Message
                });
            }
        }

        // PUT: api/address/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAddress(int id, [FromBody] AddressUpdateDto addressDto)
        {
            try
            {
                var userId = GetCurrentUserId();

                Console.WriteLine($"\n=== UPDATE ADDRESS ===");
                Console.WriteLine($"User ID: {userId}");
                Console.WriteLine($"Address ID: {id}");
                Console.WriteLine("======================\n");

                var address = await _context.Addresses
                    .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

                if (address == null)
                {
                    Console.WriteLine($"Address {id} not found for user {userId}");
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Address not found.",
                        UserId = userId,
                        AddressId = id
                    });
                }

                // Handle default flags
                if (addressDto.IsShippingDefault.HasValue && addressDto.IsShippingDefault.Value)
                {
                    var existingShippingDefaults = await _context.Addresses
                        .Where(a => a.UserId == userId && a.IsShippingDefault && a.Id != id)
                        .ToListAsync();

                    foreach (var addr in existingShippingDefaults)
                    {
                        addr.IsShippingDefault = false;
                        _context.Addresses.Update(addr);
                    }
                }

                if (addressDto.IsBillingDefault.HasValue && addressDto.IsBillingDefault.Value)
                {
                    var existingBillingDefaults = await _context.Addresses
                        .Where(a => a.UserId == userId && a.IsBillingDefault && a.Id != id)
                        .ToListAsync();

                    foreach (var addr in existingBillingDefaults)
                    {
                        addr.IsBillingDefault = false;
                        _context.Addresses.Update(addr);
                    }
                }

                _mapper.Map(addressDto, address);
                address.UpdatedDate = DateTime.UtcNow;

                _context.Addresses.Update(address);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Address {AddressId} updated by {UserId}", id, userId);
                Console.WriteLine($"✓ Address {id} updated successfully");

                var updatedDto = _mapper.Map<AddressDto>(address);

                return Ok(new
                {
                    Success = true,
                    Address = updatedDto,
                    Message = "Address updated successfully"
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    Success = false,
                    Message = "Authentication failed",
                    Error = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update address failed");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Failed to update address.",
                    Error = ex.Message
                });
            }
        }

        // DELETE: api/address/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            try
            {
                var userId = GetCurrentUserId();

                Console.WriteLine($"\n=== DELETE ADDRESS ===");
                Console.WriteLine($"User ID: {userId}");
                Console.WriteLine($"Address ID: {id}");
                Console.WriteLine("======================\n");

                var address = await _context.Addresses
                    .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

                if (address == null)
                {
                    Console.WriteLine($"Address {id} not found for user {userId}");
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Address not found.",
                        UserId = userId,
                        AddressId = id
                    });
                }

                // Check if address is used in any orders
                var ordersWithAddress = await _context.Orders
                    .Where(o => o.UserId == userId && o.ShippingAddress.Contains(address.Line1))
                    .ToListAsync();

                if (ordersWithAddress.Any())
                {
                    Console.WriteLine($"✗ Cannot delete address {id} - used in {ordersWithAddress.Count} orders");
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Cannot delete address used in orders.",
                        OrdersCount = ordersWithAddress.Count
                    });
                }

                _context.Addresses.Remove(address);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Address {AddressId} deleted by {UserId}", id, userId);
                Console.WriteLine($"✓ Address {id} deleted successfully");

                return Ok(new
                {
                    Success = true,
                    Message = "Address deleted successfully.",
                    DeletedId = id
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    Success = false,
                    Message = "Authentication failed",
                    Error = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete address failed");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Failed to delete address.",
                    Error = ex.Message
                });
            }
        }

     
     

     
    }
}