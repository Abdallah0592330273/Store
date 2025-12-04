using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Store.DataAccess.Entities;
using Store.StoreWebApi.Dtos;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Store.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            IConfiguration config,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _config = config;
            _logger = logger;
        }

        private string GenerateJwtToken(ApplicationUser user, List<string> roles)
        {
            var jwtSettings = _config.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expiryInMinutes = jwtSettings.GetValue<int>("ExpiryInMinutes", 59);

            if (string.IsNullOrEmpty(secretKey) || secretKey.Length < 32)
                throw new Exception("Invalid JWT secret key configuration");

            Console.WriteLine($"\n=== GENERATING JWT TOKEN ===");
            Console.WriteLine($"User: {user.Email}");
            Console.WriteLine($"User ID: {user.Id}");
            Console.WriteLine($"Roles: {string.Join(", ", roles)}");
            Console.WriteLine($"Expiry: {expiryInMinutes} minutes");
            Console.WriteLine($"Issuer: {issuer}");
            Console.WriteLine($"Audience: {audience}");
            Console.WriteLine("============================\n");

            // Create claims - Use ClaimTypes for consistency
            var claims = new List<Claim>
            {
                // Primary identifier - مهم جداً
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                
                // JWT standard claims
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                
                // ASP.NET Identity standard claims
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
                
                // Custom claims
                new Claim("FullName", $"{user.FirstName} {user.LastName}".Trim()),
                new Claim("UserId", user.Id)
            };

            // Add roles
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Token expires in 59 minutes
            var expires = DateTime.UtcNow.AddMinutes(expiryInMinutes);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            Console.WriteLine($"\n=== TOKEN GENERATED ===");
            Console.WriteLine($"Token Length: {tokenString.Length}");
            Console.WriteLine($"Expires (UTC): {expires:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"Expires (Local): {expires.ToLocalTime():yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine("=======================\n");

            return tokenString;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser != null)
                    return BadRequest(new { Message = "User already exists." });

                var user = new ApplicationUser
                {
                    UserName = request.Email,
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PhoneNumber = request.PhoneNumber,
                    CreatedDate = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                    return BadRequest(new { Errors = result.Errors.Select(e => e.Description) });

                // Assign default roles
                await _userManager.AddToRoleAsync(user, "User");
                await _userManager.AddToRoleAsync(user, "Customer");

                var roles = await _userManager.GetRolesAsync(user);
                var token = GenerateJwtToken(user, roles.ToList());

                return Ok(new
                {
                    UserId = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Token = token,
                    TokenExpiry = DateTime.UtcNow.AddMinutes(_config.GetValue<int>("JwtSettings:ExpiryInMinutes", 59)),
                    Roles = roles
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed");
                return StatusCode(500, new { Message = "Registration failed.", Details = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                Console.WriteLine($"\n=== LOGIN ATTEMPT ===");
                Console.WriteLine($"Email: {request.Email}");
                Console.WriteLine($"Time: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
                Console.WriteLine("=====================\n");

                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    Console.WriteLine("✗ User not found");
                    return Unauthorized(new { Message = "Invalid credentials." });
                }

                Console.WriteLine($"✓ User found: {user.Id}");
                Console.WriteLine($"Password hash exists: {!string.IsNullOrEmpty(user.PasswordHash)}");

                var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
                Console.WriteLine($"Password valid: {passwordValid}");

                if (!passwordValid)
                {
                    Console.WriteLine("✗ Invalid password");
                    return Unauthorized(new { Message = "Invalid credentials." });
                }

                var roles = await _userManager.GetRolesAsync(user);
                Console.WriteLine($"User roles: {string.Join(", ", roles)}");

                var token = GenerateJwtToken(user, roles.ToList());

                return Ok(new
                {
                    UserId = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Token = token,
                    TokenExpiry = DateTime.UtcNow.AddMinutes(_config.GetValue<int>("JwtSettings:ExpiryInMinutes", 59)),
                    Roles = roles,
                    Message = "Login successful"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed");
                return StatusCode(500, new
                {
                    Message = "Login failed.",
                    Details = ex.Message,
                    StackTrace = ex.StackTrace
                });
            }
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Console.WriteLine($"\n=== GET CURRENT USER ===");
                Console.WriteLine($"User ID from claims: {userId}");
                Console.WriteLine($"Is Authenticated: {User.Identity?.IsAuthenticated}");
                Console.WriteLine("========================\n");

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return Unauthorized();

                var roles = await _userManager.GetRolesAsync(user);

                return Ok(new
                {
                    user.Id,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    user.PhoneNumber,
                    user.EmailConfirmed,
                    Roles = roles,
                    TokenValid = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get current user failed");
                return StatusCode(500, new { Message = "Failed to get user info." });
            }
        }

      

     
    }
}