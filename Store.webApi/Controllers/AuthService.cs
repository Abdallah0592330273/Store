
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Store.DataAccess.Entities;
using Store.StoreWebApi.Controllers;
using Store.WebApi.Dtos.User;
using Microsoft.AspNetCore.Identity.Data;
using Store.StoreWebApi.Dtos;

namespace Store.WebApi.Controllers
{

    public class AuthService : IAuthService
    {


        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        // private readonly IJwtTokenGenerator _jwtGenerator; // Assume this is injected

        public AuthService(UserManager<ApplicationUser> userManager, IMapper mapper /*, IJwtTokenGenerator jwtGenerator */)
        {
            _userManager = userManager;
            _mapper = mapper;
            // _jwtGenerator = jwtGenerator;
        }

        public async Task<string?> Register(UserRegisterDto userRegisterDto)
        {
            var user = _mapper.Map<ApplicationUser>(userRegisterDto);

            // 1. Use UserManager to securely hash the password and create the user
            var result = await _userManager.CreateAsync(user, userRegisterDto.Password);

            if (result.Succeeded)
            {
                // Optional: Automatically assign a default role (e.g., "Customer")
                await _userManager.AddToRoleAsync(user, "Customer");
                return null; // Success
            }

            // 2. Aggregate errors and return a single message
            return string.Join(", ", result.Errors.Select(e => e.Description));
        }

        public async Task<AuthResponse> Login(StoreWebApi.Dtos.LoginRequest userLoginDto)
        {
            // 1. Find user by email
            var user = await _userManager.FindByEmailAsync(userLoginDto.Email);

            if (user == null)
            {
                return new AuthResponse { Token = string.Empty, UserId = null }; // Invalid Credentials
            }

            // 2. Check password securely
            var passwordValid = await _userManager.CheckPasswordAsync(user, userLoginDto.Password);

            if (!passwordValid)
            {
                return new AuthResponse { Token = string.Empty, UserId = null }; // Invalid Credentials
            }

            // 3. Generate Token (This logic is usually in a separate service)
            // var token = _jwtGenerator.GenerateToken(user);
            var token = "GENERATED_JWT_TOKEN"; // Placeholder

            return new AuthResponse
            {
                Token = token,
                UserId = _mapper.Map<UserDto>(user).Id
            };
        }
    }
}