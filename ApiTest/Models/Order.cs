namespace ApiTest.Models;

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; } // User who created the order
    public int CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public List<OrderItem> Items { get; set; } = new();
    public ShippingAddress? ShippingAddress { get; set; }
    public string? Notes { get; set; }
    public string? TrackingNumber { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedByUsername { get; set; } // Username for display
}

public class OrderItem
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal => Quantity * UnitPrice;
}

public class ShippingAddress
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}

public enum OrderStatus
{
    Pending = 0,
    Processing = 1,
    Shipped = 2,
    Delivered = 3,
    Cancelled = 4
}

public record CreateOrderRequest(
    DateTime OrderDate,
    List<OrderItemRequest> Items,
    ShippingAddress? ShippingAddress,
    string? Notes
);
// Note: CustomerId is removed - will use authenticated user's ID

public record OrderItemRequest(
    int ProductId,
    int Quantity,
    decimal UnitPrice
);

public record UpdateOrderStatusRequest(
    OrderStatus Status,
    string? TrackingNumber = null,
    string? Reason = null
);

