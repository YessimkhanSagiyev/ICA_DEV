using ThAmCo.Main.Data;
using ThAmCo.Main.Models;
using Microsoft.EntityFrameworkCore;
using Polly;

namespace ThAmCo.Main.Services.UserService
{
    public class UserService : IUserService
    {
         private readonly CoreServiceContext _context;
        private readonly IAsyncPolicy _resiliencePolicy;

        public UserService(CoreServiceContext context)
        {
            _context = context;

            // Combine retry and circuit breaker policies
            _resiliencePolicy = Policy
                .Handle<DbUpdateException>() // Handle database update exceptions
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(2))
                .WrapAsync(Policy
                    .Handle<DbUpdateException>()
                    .CircuitBreakerAsync(2, TimeSpan.FromSeconds(30)));
        }

        public async Task<User> GetUserById(int id)
        {
            return await _resiliencePolicy.ExecuteAsync(async () =>
            {
                return await _context.Users.FindAsync(id);
            });
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _resiliencePolicy.ExecuteAsync(async () =>
            {
                return await Task.FromResult(_context.Users.ToList());
            });
        }

        public async Task AddUser(User user)
        {
            await _resiliencePolicy.ExecuteAsync(async () =>
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            });
        }

        public async Task<bool> ValidateUserCredentials(string email, string password)
        {
            return await _resiliencePolicy.ExecuteAsync(async () =>
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                return user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            });
        }
    }
}
