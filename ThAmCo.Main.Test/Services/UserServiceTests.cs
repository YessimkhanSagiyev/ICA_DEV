using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThAmCo.Main.Data;
using ThAmCo.Main.Models;
using ThAmCo.Main.Services.UserService;
using System.Linq;
using System.Threading.Tasks;

namespace ThAmCo.Main.Test.Services
{
    [TestClass]
    public class UserServiceTests
    {
        private CoreServiceContext _context;
        private UserService _userService;

        [TestInitialize]
        public void TestInitialize()
        {
            // Setup InMemory database
            var options = new DbContextOptionsBuilder<CoreServiceContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Use a unique database name for each test
                .Options;

            _context = new CoreServiceContext(options);
            _userService = new UserService(_context);

            // Seed the database with unique keys
            _context.Users.Add(new User { UserId = 1, Name = "John Doe", Email = "john@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"), Address = "123 Main St" });
            _context.Users.Add(new User { UserId = 2, Name = "Jane Smith", Email = "jane@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("mypassword"), Address = "456 Maple Ave" });
            _context.SaveChanges();
        }


        [TestMethod]
        public async Task GetUserById_ReturnsCorrectUser()
        {
            // Act
            var user = await _userService.GetUserById(1);

            // Assert
            Assert.IsNotNull(user);
            Assert.AreEqual("John Doe", user.Name);
        }

        [TestMethod]
        public async Task GetAllUsers_ReturnsAllUsers()
        {
            // Act
            var users = await _userService.GetAllUsers();

            // Assert
            Assert.AreEqual(2, users.Count());
        }

        [TestMethod]
        public async Task ValidateUserCredentials_ReturnsTrueForValidCredentials()
        {
            // Act
            var isValid = await _userService.ValidateUserCredentials("john@example.com", "password123");

            // Assert
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public async Task ValidateUserCredentials_ReturnsFalseForInvalidCredentials()
        {
            // Act
            var isValid = await _userService.ValidateUserCredentials("john@example.com", "wrongpassword");

            // Assert
            Assert.IsFalse(isValid);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _context.Dispose();
        }
    }
}
