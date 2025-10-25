using ApiTest.Models;

namespace ApiTest.Services;

public class OrderService : IOrderService
{
    private readonly List<Order> _orders = new();
    private readonly IProductService _productService;
    private int _nextId = 1;

    public OrderService(IProductService productService)
    {
        _productService = productService;

        // Seed with sample data (UserId=1 represents testuser from seed data)
        _orders.AddRange(new[]
        {
            new Order
            {
                Id = _nextId++,
                UserId = 1,
                CustomerId = 1,
                OrderDate = DateTime.UtcNow.AddDays(-5),
                Status = OrderStatus.Delivered,
                Items = new List<OrderItem>
                {
                    new OrderItem { ProductId = 1, ProductName = "Laptop", Quantity = 1, UnitPrice = 999.99m }
                },
                ShippingAddress = new ShippingAddress
                {
                    Street = "123 Main St",
                    City = "New York",
                    State = "NY",
                    ZipCode = "10001",
                    Country = "USA"
                },
                TotalAmount = 999.99m,
                TrackingNumber = "TRACK12345",
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                CreatedByUsername = "testuser"
            },
            new Order
            {
                Id = _nextId++,
                UserId = 1,
                CustomerId = 1,
                OrderDate = DateTime.UtcNow.AddDays(-2),
                Status = OrderStatus.Processing,
                Items = new List<OrderItem>
                {
                    new OrderItem { ProductId = 2, ProductName = "Mouse", Quantity = 2, UnitPrice = 29.99m },
                    new OrderItem { ProductId = 3, ProductName = "Keyboard", Quantity = 1, UnitPrice = 79.99m }
                },
                ShippingAddress = new ShippingAddress
                {
                    Street = "456 Oak Ave",
                    City = "Los Angeles",
                    State = "CA",
                    ZipCode = "90001",
                    Country = "USA"
                },
                TotalAmount = 139.97m,
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                CreatedByUsername = "testuser"
            }
        });
    }

    public Task<IEnumerable<Order>> GetAllAsync(int? customerId = null, OrderStatus? status = null, int? userId = null)
    {
        var query = _orders.AsEnumerable();

        if (customerId.HasValue)
        {
            query = query.Where(o => o.CustomerId == customerId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(o => o.Status == status.Value);
        }

        if (userId.HasValue)
        {
            query = query.Where(o => o.UserId == userId.Value);
        }

        return Task.FromResult(query);
    }

    public Task<Order?> GetByIdAsync(int id)
    {
        var order = _orders.FirstOrDefault(o => o.Id == id);
        return Task.FromResult(order);
    }

    public async Task<Order> CreateAsync(CreateOrderRequest request, int userId, string username)
    {
        // Validate items
        if (request.Items == null || !request.Items.Any())
        {
            throw new InvalidOperationException("Order must contain at least one item");
        }

        // Validate products exist and get their names
        var orderItems = new List<OrderItem>();
        foreach (var item in request.Items)
        {
            var product = await _productService.GetByIdAsync(item.ProductId);
            if (product == null)
            {
                throw new InvalidOperationException($"Product with ID {item.ProductId} not found");
            }

            orderItems.Add(new OrderItem
            {
                ProductId = item.ProductId,
                ProductName = product.Name,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            });
        }

        var totalAmount = orderItems.Sum(i => i.Subtotal);

        var order = new Order
        {
            Id = _nextId++,
            UserId = userId,
            CustomerId = userId, // Use authenticated user's ID as customer ID
            OrderDate = request.OrderDate,
            Status = OrderStatus.Pending,
            Items = orderItems,
            ShippingAddress = request.ShippingAddress,
            Notes = request.Notes,
            TotalAmount = totalAmount,
            CreatedAt = DateTime.UtcNow,
            CreatedByUsername = username
        };

        _orders.Add(order);
        return order;
    }

    public Task<Order?> UpdateStatusAsync(int id, UpdateOrderStatusRequest request)
    {
        var order = _orders.FirstOrDefault(o => o.Id == id);

        if (order == null)
        {
            return Task.FromResult<Order?>(null);
        }

        // Validate status transition (simple validation)
        if (order.Status == OrderStatus.Delivered || order.Status == OrderStatus.Cancelled)
        {
            throw new InvalidOperationException($"Cannot change status of {order.Status} order");
        }

        order.Status = request.Status;
        order.UpdatedAt = DateTime.UtcNow;

        if (!string.IsNullOrEmpty(request.TrackingNumber))
        {
            order.TrackingNumber = request.TrackingNumber;
        }

        return Task.FromResult<Order?>(order);
    }

    public Task<bool> DeleteAsync(int id)
    {
        var order = _orders.FirstOrDefault(o => o.Id == id);

        if (order == null)
        {
            return Task.FromResult(false);
        }

        // Only allow deletion of pending orders
        if (order.Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException("Can only delete pending orders");
        }

        _orders.Remove(order);
        return Task.FromResult(true);
    }
}

