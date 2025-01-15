
using ThAmCo.Main.Models;

namespace ThAmCo.Main.Services.UserService
{
    public class FakeUserService : IUserService
    {
        private readonly List<User> _users;

        public FakeUserService()
        {
            // Predefined users
            _users = new List<User>
            {
                new User { UserId = 1, Name = "John Doe", Email = "john.doe@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"), Address = "123 Main St", CreatedAt = DateTime.UtcNow },
                new User { UserId = 2, Name = "Jane Smith", Email = "jane.smith@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("mypassword"), Address = "456 Maple Ave", CreatedAt = DateTime.UtcNow }
            };
        }

        public Task<User> GetUserById(int id)
        {
            return Task.FromResult(_users.FirstOrDefault(u => u.UserId == id));
        }

        public Task<IEnumerable<User>> GetAllUsers()
        {
            return Task.FromResult(_users.AsEnumerable());
        }

        public Task AddUser(User user)
        {
            user.UserId = _users.Count + 1;
            _users.Add(user);
            return Task.CompletedTask;
        }

        public Task<bool> ValidateUserCredentials(string email, string password)
        {
            var user = _users.FirstOrDefault(u => u.Email == email);
            return Task.FromResult(user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash));
        }
    }
}
