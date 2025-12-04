

using Microsoft.AspNetCore.Identity;
using Store.DataAccess.Context;
using Store.DataAccess.Entities;
using Store.DataAccess.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;

// Adjusted namespace for consistency
namespace Store.DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly StoreContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        // IConfiguration and secretKey removed, as they are not needed for user data access.
        public UserRepository(StoreContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // NOTE: Using .Result to match the synchronous interface. 
        // Best practice is to use async/await and return Task<T>.

        public ApplicationUser GetUser(string id)
        {
            // Use FindByIdAsync from UserManager, optimized for Identity lookups
            return _userManager.FindByIdAsync(id).Result;
        }

        public ApplicationUser GetUserByEmail(string email)
        {
            // Use FindByEmailAsync from UserManager
            return _userManager.FindByEmailAsync(email).Result;
        }

        public List<ApplicationUser> GetUsers()
        {
            // Use UserManager.Users which provides an IQueryable to all users
            return _userManager.Users.ToList();
        }

        public void DeleteUser(string id)
        {
            var userToDelete = _userManager.FindByIdAsync(id).Result;

            if (userToDelete != null)
            {
                // Use UserManager to ensure all related Identity tables are cleaned up
                var result = _userManager.DeleteAsync(userToDelete).Result;

                if (!result.Succeeded)
                {
                    // Throw exception on failure
                    throw new InvalidOperationException($"Failed to delete user {id}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }

        public void AddUser(ApplicationUser user)
        {
            // WARNING: This implementation assumes the password has ALREADY been set and hashed 
            // on the ApplicationUser object by a separate service layer (highly unusual) 
            // or that the user object is being created without a password for non-login purposes.

            // In 99% of cases, you should use: await _userManager.CreateAsync(user, password) 
            // in a Service Layer, not directly in a synchronous repository method.

            var result = _userManager.CreateAsync(user).Result;

            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
    }
}