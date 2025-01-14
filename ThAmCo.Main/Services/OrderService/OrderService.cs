using ThAmCo.Main.Data;
using ThAmCo.Main.Models;
using Microsoft.EntityFrameworkCore;
using Polly;


namespace ThAmCo.Main.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly CoreServiceContext _context;
        private readonly IAsyncPolicy _resiliencePolicy;

        public OrderService(CoreServiceContext context)
        {
            _context = context;

            // Combine retry and circuit breaker policies
            _resiliencePolicy = Policy
                .Handle<DbUpdateException>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(2))
                .WrapAsync(Policy
                    .Handle<DbUpdateException>()
                    .CircuitBreakerAsync(2, TimeSpan.FromSeconds(30)));
        }

        public async Task<Order> GetOrderById(int id)
        {
            return await _resiliencePolicy.ExecuteAsync(async () =>
            {
                return await _context.Orders.FindAsync(id);
            });
        }

        public async Task<IEnumerable<Order>> GetAllOrders()
        {
            return await _resiliencePolicy.ExecuteAsync(async () =>
            {
                return await Task.FromResult(_context.Orders.ToList());
            });
        }

        public async Task AddOrder(Order order)
        {
            await _resiliencePolicy.ExecuteAsync(async () =>
            {
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
            });
        }

    }
}
