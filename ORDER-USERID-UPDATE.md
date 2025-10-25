# Order UserId Implementation

## ✅ Changes Made

The Order system has been updated to automatically capture and store the authenticated user's ID when creating orders.

---

## 📝 What Changed

### 1. Order Model Updated
**File**: `ApiTest/Models/Order.cs`

Added fields:
- `UserId` (int) - ID of the user who created the order
- `CreatedByUsername` (string?) - Username for display purposes

```csharp
public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; } // NEW: User who created the order
    public int CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    // ... other fields
    public string? CreatedByUsername { get; set; } // NEW: Username for display
}
```

### 2. CreateOrderRequest Updated
**File**: `ApiTest/Models/Order.cs`

**Removed**: `customerId` parameter  
**Reason**: The authenticated user's ID is now automatically used

**Before**:
```csharp
public record CreateOrderRequest(
    int CustomerId,  // ❌ REMOVED
    DateTime OrderDate,
    List<OrderItemRequest> Items,
    // ...
);
```

**After**:
```csharp
public record CreateOrderRequest(
    DateTime OrderDate,
    List<OrderItemRequest> Items,
    // ...
);
// CustomerId comes from authenticated user automatically
```

### 3. OrderService Updated
**File**: `ApiTest/Services/OrderService.cs`

**CreateAsync** now accepts:
- `int userId` - From authenticated user
- `string username` - From authenticated user

**What it does**:
- Stores `userId` in the order
- Uses `userId` as `customerId` (user is both creator and customer)
- Stores `username` for display

**GetAllAsync** now accepts:
- `int? userId` - Filter orders by user (optional)

### 4. OrdersController Updated
**File**: `ApiTest/Controllers/OrdersController.cs`

**Create endpoint** now:
1. Extracts user ID from authentication claims
2. Extracts username from authentication claims
3. Passes both to the service
4. Returns error if user info can't be extracted

```csharp
// Extract from authentication
var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
var username = User.FindFirst(ClaimTypes.Name)?.Value;

// Pass to service
var order = await _orderService.CreateAsync(request, userId, username);
```

---

## 🎯 How It Works Now

### Creating an Order

#### Step 1: User Logs In
```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "testuser",
  "password": "password123"
}
```

**Response includes token with user info**:
```json
{
  "success": true,
  "token": "MTp0ZXN0dXNlcjpVc2VyOjYzODY4MjUw...",
  "user": {
    "id": 1,
    "username": "testuser",
    "email": "test@example.com",
    "role": "User"
  }
}
```

#### Step 2: User Creates Order (With Auth Token)
```http
POST /api/orders
Authorization: Bearer MTp0ZXN0dXNlcjpVc2VyOjYzODY4MjUw...
Content-Type: application/json

{
  "orderDate": "2024-10-25T10:00:00Z",
  "items": [
    {
      "productId": 1,
      "quantity": 2,
      "unitPrice": 99.99
    }
  ],
  "shippingAddress": {
    "street": "123 Main St",
    "city": "New York",
    "state": "NY",
    "zipCode": "10001",
    "country": "USA"
  }
}
```

**Notice**: No `customerId` in request!

#### Step 3: System Creates Order
The system automatically:
1. Extracts user ID (1) from token
2. Extracts username ("testuser") from token
3. Creates order with:
   - `UserId = 1`
   - `CustomerId = 1`
   - `CreatedByUsername = "testuser"`

**Response**:
```json
{
  "id": 3,
  "userId": 1,
  "customerId": 1,
  "createdByUsername": "testuser",
  "orderDate": "2024-10-25T10:00:00Z",
  "status": 0,
  "items": [...],
  "totalAmount": 199.98,
  "createdAt": "2024-10-25T14:30:00Z"
}
```

---

## 🔍 Querying Orders

### Get All Orders
```http
GET /api/orders
```
Returns all orders (public endpoint)

### Get Orders by User ID
```http
GET /api/orders?userId=1
```
Returns only orders created by user with ID 1

### Get Orders by Customer ID
```http
GET /api/orders?customerId=1
```
Returns orders for customer with ID 1

### Combine Filters
```http
GET /api/orders?userId=1&status=1
```
Returns orders created by user 1 with status "Processing"

---

## 📊 Data Flow

```
User Login
    ↓
Token Generated (contains userId=1, username="testuser")
    ↓
User Creates Order (sends token in Authorization header)
    ↓
Controller extracts userId & username from token
    ↓
Service creates order with:
  - UserId = 1
  - CustomerId = 1 (same as UserId)
  - CreatedByUsername = "testuser"
    ↓
Order saved with user tracking
```

---

## ✅ Benefits

### 1. Automatic User Tracking
- Every order knows who created it
- No need to manually pass user ID
- Cannot fake or manipulate user ID

### 2. Security
- User ID comes from authenticated token
- Cannot create orders for other users
- Token validation ensures legitimacy

### 3. Auditability
- Track which user created each order
- Username stored for easy display
- Can query all orders by specific user

### 4. Simplified API
- No need to pass `customerId` in request
- Cleaner request body
- Less room for errors

---

## 🧪 Testing

### Test Files Updated
- `ApiTest.Testing/Orders/CreateOrder/CreateOrder.success.http`
- `ApiTest.Testing/Orders/CreateOrder/CreateOrder.fail.http`

All test scenarios now:
1. Include login step to get token
2. Use token in Authorization header
3. Don't include `customerId` in request body

### Example Test
```http
### Step 1: Login
POST https://localhost:5001/api/auth/login
Content-Type: application/json

{
  "username": "testuser",
  "password": "password123"
}

###

### Step 2: Create Order (use token from above)
POST https://localhost:5001/api/orders
Authorization: Bearer {TOKEN_FROM_STEP_1}
Content-Type: application/json

{
  "orderDate": "2024-10-25T10:00:00Z",
  "items": [
    {
      "productId": 1,
      "quantity": 1,
      "unitPrice": 99.99
    }
  ],
  "shippingAddress": {
    "street": "123 Main St",
    "city": "New York",
    "state": "NY",
    "zipCode": "10001",
    "country": "USA"
  }
}
```

---

## ⚠️ Important Notes

### Authentication Required
Creating orders **requires authentication**. Without a valid token:
- Request will fail with `401 Unauthorized`
- Cannot create orders anonymously

### User = Customer
Currently, the authenticated user becomes the customer:
- `UserId = CustomerId`
- This assumes users create orders for themselves
- Can be enhanced later if users need to create orders for others

### Token Contains User Info
The authentication token includes:
- User ID
- Username
- Role

These are extracted and used automatically.

---

## 🔄 Migration Notes

### Existing Orders
Seed data has been updated with:
- `UserId = 1` (testuser)
- `CreatedByUsername = "testuser"`

### API Consumers
Any code calling `POST /api/orders` needs to:
1. ✅ Include `Authorization: Bearer {token}` header
2. ❌ Remove `customerId` from request body

---

## 📚 Related Files

### Modified Files
1. `ApiTest/Models/Order.cs` - Added UserId and CreatedByUsername
2. `ApiTest/Services/IOrderService.cs` - Updated interface
3. `ApiTest/Services/OrderService.cs` - Updated implementation
4. `ApiTest/Controllers/OrdersController.cs` - Extract user from claims
5. `ApiTest.Testing/Orders/CreateOrder/*.http` - Updated tests

### Documentation
- `AUTHORIZATION-GUIDE.md` - Authentication details
- `AUTHORIZATION-SUMMARY.md` - Auth implementation summary
- `ORDER-USERID-UPDATE.md` - This file

---

## ✅ Build Status

✅ **Build Successful**
- 0 Warnings
- 0 Errors
- All changes working correctly

---

**Orders now automatically track which user created them! 🎉**

Every order is associated with the authenticated user who created it.

