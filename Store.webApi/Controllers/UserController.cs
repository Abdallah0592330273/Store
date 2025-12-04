using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store.DataAccess.Entities;
using Store.webApi.Dtos.User;
using Store.WebApi.Controllers;
using Store.WebApi.Dtos.User;
using System.ComponentModel.DataAnnotations;
// "DefaultConnection": "Data Source=SQL9001.site4now.net;Initial Catalog=db_ac149d_storedb;User Id=db_ac149d_storedb_admin;Password=abed2020;TrustServerCertificate=True"

namespace Store.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : BaseApiController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserController> _logger;

        public UserController(
            UserManager<ApplicationUser> userManager,
            ILogger<UserController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        // GET: api/user/profile
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = GetCurrentUserId();
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                    return NotFound(new { Message = "User not found." });

                var roles = await _userManager.GetRolesAsync(user);

                return Ok(new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    EmailConfirmed = user.EmailConfirmed,
                    CreatedDate = user.CreatedDate,
                    UpdatedDate = user.UpdatedDate,
                    Roles = roles
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get profile failed");
                return StatusCode(500, new { Message = "Failed to get profile." });
            }
        }

        // PUT: api/user/profile
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserUpdateDto profileDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                    return NotFound(new { Message = "User not found." });

                user.FirstName = profileDto.FirstName ?? user.FirstName;
                user.LastName = profileDto.LastName ?? user.LastName;
                user.PhoneNumber = profileDto.PhoneNumber ?? user.PhoneNumber;
                user.UpdatedDate = DateTime.UtcNow;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                    return BadRequest(new { Errors = result.Errors });

                return Ok(new { Message = "Profile updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update profile failed");
                return StatusCode(500, new { Message = "Failed to update profile." });
            }
        }

        // POST: api/user/change-password
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto passwordDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                    return NotFound(new { Message = "User not found." });

                var result = await _userManager.ChangePasswordAsync(
                    user,
                    passwordDto.CurrentPassword,
                    passwordDto.NewPassword);

                if (!result.Succeeded)
                    return BadRequest(new { Errors = result.Errors });

                return Ok(new { Message = "Password changed successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Change password failed");
                return StatusCode(500, new { Message = "Failed to change password." });
            }
        }

        // ADMIN ONLY ENDPOINTS

        // GET: api/user/all
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var users = await _userManager.Users
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var userDtos = new List<UserDto>();
                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    userDtos.Add(new UserDto
                    {
                        Id = user.Id,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        PhoneNumber = user.PhoneNumber,
                        EmailConfirmed = user.EmailConfirmed,
                        CreatedDate = user.CreatedDate,
                        UpdatedDate = user.UpdatedDate,
                        Roles = roles
                    });
                }

                var totalCount = await _userManager.Users.CountAsync();

                return Ok(new
                {
                    Users = userDtos,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get all users failed");
                return StatusCode(500, new { Message = "Failed to get users." });
            }
        }

        // POST: api/user/{userId}/upgrade-admin
        [HttpPost("{userId}/upgrade-admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpgradeToAdmin([FromRoute] string userId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (userId == currentUserId)
                    return BadRequest(new { Message = "Cannot upgrade yourself." });

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return NotFound(new { Message = "User not found." });

                if (await _userManager.IsInRoleAsync(user, "Admin"))
                    return Ok(new { Message = "User is already an admin." });

                var result = await _userManager.AddToRoleAsync(user, "Admin");
                if (!result.Succeeded)
                    return BadRequest(new { Errors = result.Errors });

                _logger.LogInformation(
                    "User {CurrentUserId} upgraded user {UserId} to Admin",
                    currentUserId, userId);

                return Ok(new { Message = "User upgraded to admin successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Upgrade to admin failed");
                return StatusCode(500, new { Message = "Failed to upgrade user to admin." });
            }
        }
    }
}