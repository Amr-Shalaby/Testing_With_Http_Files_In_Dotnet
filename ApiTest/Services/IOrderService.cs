using ApiTest.Models;

namespace ApiTest.Services;

public interface IOrderService
{
    Task<IEnumerable<Order>> GetAllAsync(int? customerId = null, OrderStatus? status = null, int? userId = null);
    Task<Order?> GetByIdAsync(int id);
    Task<Order> CreateAsync(CreateOrderRequest request, int userId, string username);
    Task<Order?> UpdateStatusAsync(int id, UpdateOrderStatusRequest request);
    Task<bool> DeleteAsync(int id);
}

