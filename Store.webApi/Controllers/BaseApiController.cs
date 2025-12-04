using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Store.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        protected string GetCurrentUserId()
        {
            // الحل: ابحث عن جميع الأشكال الممكنة لـ UserId
            var userId = User.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier") // هذا الموجود في التوكن
                        ?? User.FindFirstValue(ClaimTypes.NameIdentifier) // القصير
                        ?? User.FindFirstValue("nameidentifier") // بدون URI
                        ?? User.FindFirstValue("sub") // JWT standard
                        ?? User.FindFirstValue("UserId") // Custom claim
                        ?? User.FindFirstValue(ClaimTypes.Name); // كحل أخير

            if (string.IsNullOrEmpty(userId))
            {
                // للتصحيح
                Console.WriteLine("\n=== USER CLAIMS FOR DEBUGGING ===");
                foreach (var claim in User.Claims)
                {
                    Console.WriteLine($"{claim.Type} = {claim.Value}");
                }
                Console.WriteLine($"IsAuthenticated: {User.Identity?.IsAuthenticated}");
                Console.WriteLine("=================================\n");

                throw new UnauthorizedAccessException("User ID not found in claims");
            }

            Console.WriteLine($"✅ UserId found: {userId}");
            return userId;
        }

        // دالة مساعدة للحصول على البريد
        protected string GetCurrentUserEmail()
        {
            return User.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")
                   ?? User.FindFirstValue(ClaimTypes.Email)
                   ?? User.FindFirstValue("email");
        }

        // دالة مساعدة للحصول على الأدوار
        protected List<string> GetCurrentUserRoles()
        {
            return User.Claims
                .Where(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" ||
                           c.Type == ClaimTypes.Role ||
                           c.Type == "role")
                .Select(c => c.Value)
                .ToList();
        }
    }
}