using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThAmCo.Main.Data;
using ThAmCo.Main.Models;
using ThAmCo.Main.Services.OrderService;
using System.Linq;
using System.Threading.Tasks;

namespace ThAmCo.Main.Test.Services
{
    [TestClass]
    public class OrderServiceTests
    {
        private CoreServiceContext _context;
        private OrderService _orderService;

        [TestInitialize]
        public void TestInitialize()
        {
            // Setup InMemory database
            var options = new DbContextOptionsBuilder<CoreServiceContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Use a unique database name for each test
                .Options;

            _context = new CoreServiceContext(options);
            _orderService = new OrderService(_context);

            // Seed the database with unique keys
            _context.Orders.Add(new Order { OrderId = 1, UserId = 1, ProductId = 1, Quantity = 1, Status = "Completed" });
            _context.Orders.Add(new Order { OrderId = 2, UserId = 2, ProductId = 2, Quantity = 2, Status = "Pending" });
            _context.SaveChanges();
        }


        [TestMethod]
        public async Task GetOrderById_ReturnsCorrectOrder()
        {
            // Act
            var order = await _orderService.GetOrderById(1);

            // Assert
            Assert.IsNotNull(order);
            Assert.AreEqual(1, order.UserId);
        }

        [TestMethod]
        public async Task GetAllOrders_ReturnsAllOrders()
        {
            // Act
            var orders = await _orderService.GetAllOrders();

            // Assert
            Assert.AreEqual(2, orders.Count());
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _context.Dispose();
        }
    }
}
