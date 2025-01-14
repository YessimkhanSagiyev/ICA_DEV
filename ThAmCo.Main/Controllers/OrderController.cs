using Microsoft.AspNetCore.Mvc;
using ThAmCo.Main.DTOs;
using ThAmCo.Main.Models;
using ThAmCo.Main.Services.OrderService;

namespace ThAmCo.Main.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrders();

            var response = orders.Select(o => new OrderResponseDto
            {
                OrderId = o.OrderId,
                UserId = o.UserId,
                ProductId = o.ProductId,
                Quantity = o.Quantity,
                Status = o.Status,
                OrderDate = o.OrderDate
            });

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderById(id);
            if (order == null)
                return NotFound();

            var response = new OrderResponseDto
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                ProductId = order.ProductId,
                Quantity = order.Quantity,
                Status = order.Status,
                OrderDate = order.OrderDate
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> AddOrder([FromBody] OrderRequestDto orderDto)
        {
            var order = new Order
            {
                UserId = orderDto.UserId,
                ProductId = orderDto.ProductId,
                Quantity = orderDto.Quantity,
                Status = "Pending",
                OrderDate = DateTime.UtcNow
            };

            await _orderService.AddOrder(order);
            return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderId }, order);
        }
    }
}
