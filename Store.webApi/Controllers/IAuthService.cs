using Microsoft.AspNetCore.Identity.Data;
using Store.StoreWebApi.Dtos;
using Store.WebApi.Dtos.User;
using LoginRequest = Store.StoreWebApi.Dtos.LoginRequest;

namespace Store.StoreWebApi.Controllers
{
    public interface IAuthService
    {
        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="userRegisterDto">The DTO containing new user details.</param>
        /// <returns>A Task resolving to a string: either null (success) or an error message.</returns>
        Task<string?> Register(UserRegisterDto userRegisterDto);

        /// <summary>
        /// Attempts to log in a user and returns a token and user details on success.
        /// </summary>
        /// <param name="userLoginDto">The DTO containing user login credentials.</param>
        /// <returns>A Task resolving to a LoginResponseDto on success, or an appropriate indicator of failure.</returns>
        Task<AuthResponse> Login(LoginRequest userLoginDto);
    }
}