using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ApiTest.Models;
using ApiTest.Services;
using System.Security.Claims;

namespace ApiTest.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all orders with optional filtering
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Order>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Order>>> GetAll(
        [FromQuery] int? customerId = null,
        [FromQuery] OrderStatus? status = null,
        [FromQuery] int? userId = null)
    {
        _logger.LogInformation("Getting orders - CustomerId: {CustomerId}, Status: {Status}, UserId: {UserId}",
            customerId, status, userId);
        var orders = await _orderService.GetAllAsync(customerId, status, userId);
        return Ok(orders);
    }

    /// <summary>
    /// Gets a specific order by id
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Order>> GetById(int id)
    {
        _logger.LogInformation("Getting order with id {OrderId}", id);
        var order = await _orderService.GetByIdAsync(id);

        if (order == null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    /// <summary>
    /// Creates a new order (authenticated user becomes the customer)
    /// </summary>
    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(Order), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Order>> Create([FromBody] CreateOrderRequest request)
    {
        // Get authenticated user info from claims
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            _logger.LogWarning("Could not extract user ID from claims");
            return Unauthorized(new { error = "Invalid authentication token" });
        }

        _logger.LogInformation("Creating new order for user {UserId} ({Username})", userId, username);

        try
        {
            var order = await _orderService.CreateAsync(request, userId, username);
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to create order");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Updates the status of an order
    /// </summary>
    [Authorize]
    [HttpPatch("{id}/status")]
    [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Order>> UpdateStatus(int id, [FromBody] UpdateOrderStatusRequest request)
    {
        _logger.LogInformation("Updating order {OrderId} status to {Status}", id, request.Status);

        try
        {
            var order = await _orderService.UpdateStatusAsync(id, request);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to update order status");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Deletes an order (only pending orders)
    /// </summary>
    [Authorize]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> Delete(int id)
    {
        _logger.LogInformation("Deleting order with id {OrderId}", id);

        try
        {
            var success = await _orderService.DeleteAsync(id);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to delete order");
            return BadRequest(new { error = ex.Message });
        }
    }
}

