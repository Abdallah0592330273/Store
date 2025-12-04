

using Store.DataAccess.Entities;

namespace Store.DataAccess.Repositories.Interfaces
{
    public interface IUserRepository
    {
        ApplicationUser GetUser(string id);
        ApplicationUser GetUserByEmail(string email);
        List<ApplicationUser> GetUsers();
        void DeleteUser(string id);

        // Note: For Identity users, this method is almost always handled by a Service 
        // using UserManager.CreateAsync(user, password)
        void AddUser(ApplicationUser user);
    }
}