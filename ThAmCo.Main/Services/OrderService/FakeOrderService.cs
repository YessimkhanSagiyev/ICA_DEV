using ThAmCo.Main.Models;

namespace ThAmCo.Main.Services.OrderService
{
    public class FakeOrderService : IOrderService
    {
private readonly List<Order> _orders;

        public FakeOrderService()
        {
            // Predefined orders
            _orders = new List<Order>
            {
                new Order { OrderId = 1, UserId = 1, ProductId = 1, Quantity = 1, Status = "Completed", OrderDate = DateTime.UtcNow.AddDays(-3) },
                new Order { OrderId = 2, UserId = 2, ProductId = 2, Quantity = 2, Status = "Pending", OrderDate = DateTime.UtcNow.AddDays(-1) }
            };
        }

        public Task<Order> GetOrderById(int id)
        {
            return Task.FromResult(_orders.FirstOrDefault(o => o.OrderId == id));
        }

        public Task<IEnumerable<Order>> GetAllOrders()
        {
            return Task.FromResult(_orders.AsEnumerable());
        }

        public Task AddOrder(Order order)
        {
            order.OrderId = _orders.Count + 1;
            _orders.Add(order);
            return Task.CompletedTask;
        }
    }
}
