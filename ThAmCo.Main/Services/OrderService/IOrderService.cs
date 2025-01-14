using ThAmCo.Main.Models;

namespace ThAmCo.Main.Services.OrderService
{
    public interface IOrderService
    {
        Task<Order> GetOrderById(int id);
        Task<IEnumerable<Order>> GetAllOrders();
        Task AddOrder(Order order);
    }
}
