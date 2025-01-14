using ThAmCo.Main.Models;

namespace ThAmCo.Main.Services.UserService
{
    public interface IUserService
    {
        Task<User> GetUserById(int id);
        Task<IEnumerable<User>> GetAllUsers();
        Task AddUser(User user);
        Task<bool> ValidateUserCredentials(string email, string password);
    }
}
