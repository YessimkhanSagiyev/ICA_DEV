using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ThAmCo.Main.DTOs;
using ThAmCo.Main.Models;
using ThAmCo.Main.Services.OrderService;
using System;
using System.Threading.Tasks;


[ApiController]
[Route("api/orders")]
public class OrderController : ControllerBase
{
    // Simulated in-memory data storage
    private static readonly Dictionary<Guid, string> Orders = new();
    private static readonly Dictionary<Guid, string> Users = new();

    public OrderController()
    {
        if (!Orders.Any())
        {
            Orders[Guid.NewGuid()] = "Pending";
            Orders[Guid.NewGuid()] = "Shipped";
        }

        if (!Users.Any())
        {
            Users[Guid.NewGuid()] = "John Doe";
            Users[Guid.NewGuid()] = "Jane Smith";
        }
    }

    [HttpPut("change-status")]
    public async Task<IActionResult> ChangeOrderStatusAsync([FromBody] ChangeStatusRequest request)
    {
        if (!Orders.ContainsKey(request.OrderId))
        {
            return NotFound("Order not found.");
        }

        Orders[request.OrderId] = request.NewStatus;

        await Task.CompletedTask;
        return Ok($"Order status updated to '{request.NewStatus}'.");
    }

}

// DTOs
public class ChangeStatusRequest
{
    public Guid OrderId { get; set; }
    public string NewStatus { get; set; }
}