# Quick API Reference

## 🚀 Start the API
```powershell
cd ApiTest
dotnet run
```
API runs at: `https://localhost:5001`  
Swagger UI: `https://localhost:5001/`

---

## 📚 All Available Endpoints

### 🔐 Authentication

#### Register New User
```http
POST /api/auth/register
Content-Type: application/json

{
  "username": "johndoe",
  "email": "john@example.com",
  "password": "secure123"
}
```

#### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "testuser",
  "password": "password123"
}
```

#### Get User Info
```http
GET /api/auth/user/testuser
```

#### Test Endpoint
```http
GET /api/auth/test
```

---

### 📦 Products

#### Get All Products
```http
GET /api/products
```

#### Get Product by ID
```http
GET /api/products/1
```

#### Create Product
```http
POST /api/products
Content-Type: application/json

{
  "name": "New Product",
  "description": "Product description",
  "price": 99.99,
  "stockQuantity": 50
}
```

#### Update Product
```http
PUT /api/products/1
Content-Type: application/json

{
  "name": "Updated Product",
  "description": "Updated description",
  "price": 149.99,
  "stockQuantity": 25
}
```

#### Delete Product
```http
DELETE /api/products/1
```

---

### 🏷️ Categories

#### Get All Categories
```http
GET /api/categories
```

#### Get Category by ID
```http
GET /api/categories/1
```

#### Create Category
```http
POST /api/categories
Content-Type: application/json

{
  "name": "Electronics",
  "description": "Electronic devices",
  "parentCategoryId": null
}
```

#### Create Subcategory
```http
POST /api/categories
Content-Type: application/json

{
  "name": "Laptops",
  "description": "Laptop computers",
  "parentCategoryId": 1
}
```

#### Update Category
```http
PUT /api/categories/1
Content-Type: application/json

{
  "name": "Updated Category",
  "description": "Updated description",
  "parentCategoryId": null
}
```

#### Delete Category
```http
DELETE /api/categories/1
```

---

### 📋 Orders

#### Get All Orders
```http
GET /api/orders
```

#### Get Orders by Customer
```http
GET /api/orders?customerId=1
```

#### Get Orders by Status
```http
GET /api/orders?status=1
```
Status values: 0=Pending, 1=Processing, 2=Shipped, 3=Delivered, 4=Cancelled

#### Get Order by ID
```http
GET /api/orders/1
```

#### Create Order
```http
POST /api/orders
Content-Type: application/json

{
  "customerId": 1,
  "orderDate": "2024-10-25T10:00:00Z",
  "items": [
    {
      "productId": 1,
      "quantity": 2,
      "unitPrice": 99.99
    },
    {
      "productId": 2,
      "quantity": 1,
      "unitPrice": 29.99
    }
  ],
  "shippingAddress": {
    "street": "123 Main St",
    "city": "New York",
    "state": "NY",
    "zipCode": "10001",
    "country": "USA"
  },
  "notes": "Please deliver after 5 PM"
}
```

#### Update Order Status
```http
PATCH /api/orders/1/status
Content-Type: application/json

{
  "status": 1,
  "trackingNumber": "TRACK123456789"
}
```

#### Delete Order (Pending Only)
```http
DELETE /api/orders/1
```

---

### 🌤️ Weather Forecast (Sample)

#### Get Weather Forecast
```http
GET /api/weatherforecast
```

---

## 🧪 Test Data

### Pre-seeded Products
1. **Laptop** - $999.99 (10 in stock)
2. **Mouse** - $29.99 (50 in stock)
3. **Keyboard** - $79.99 (30 in stock)

### Pre-seeded Categories
1. **Electronics** (parent)
2. **Computers** (child of Electronics)
3. **Books** (parent)

### Pre-seeded Orders
1. Order #1 - Customer 1, Status: Delivered
2. Order #2 - Customer 1, Status: Processing

### Pre-seeded Users
1. **Username:** `testuser` | **Password:** `password123` | **Role:** User
2. **Username:** `admin` | **Password:** `admin123` | **Role:** Admin

---

## 🎯 Quick Testing Flow

### 1. Test Authentication
```http
# 1. Register
POST /api/auth/register
{ "username": "demo", "email": "demo@test.com", "password": "demo123" }

# 2. Login
POST /api/auth/login
{ "username": "demo", "password": "demo123" }
```

### 2. Test Products
```http
# 1. Get all
GET /api/products

# 2. Create one
POST /api/products
{ "name": "Test Item", "description": "Test", "price": 19.99, "stockQuantity": 5 }

# 3. Update it
PUT /api/products/4
{ "name": "Updated Item", "description": "Updated", "price": 24.99, "stockQuantity": 10 }

# 4. Delete it
DELETE /api/products/4
```

### 3. Test Categories
```http
# 1. Get all
GET /api/categories

# 2. Create parent
POST /api/categories
{ "name": "Clothing", "description": "Apparel items", "parentCategoryId": null }

# 3. Create child
POST /api/categories
{ "name": "T-Shirts", "description": "T-shirt items", "parentCategoryId": 4 }
```

### 4. Test Orders
```http
# 1. Get all
GET /api/orders

# 2. Create order
POST /api/orders
{
  "customerId": 1,
  "orderDate": "2024-10-25T10:00:00Z",
  "items": [{ "productId": 1, "quantity": 1, "unitPrice": 99.99 }],
  "shippingAddress": {
    "street": "123 Test St",
    "city": "TestCity",
    "state": "TS",
    "zipCode": "12345",
    "country": "TestLand"
  }
}

# 3. Update status
PATCH /api/orders/3/status
{ "status": 1, "trackingNumber": "TEST123" }
```

---

## 📊 HTTP Status Codes

| Code | Meaning | When Used |
|------|---------|-----------|
| 200 | OK | Successful GET, PUT, PATCH |
| 201 | Created | Successful POST |
| 204 | No Content | Successful DELETE |
| 400 | Bad Request | Validation errors, invalid data |
| 401 | Unauthorized | Invalid login credentials |
| 404 | Not Found | Resource doesn't exist |
| 409 | Conflict | Duplicate resource (e.g., username) |

---

## 🔧 Testing with Tools

### Visual Studio
1. Open any `.http` file in `ApiTest.Testing/`
2. Click green play button (▶)
3. View response

### VS Code (REST Client)
1. Install REST Client extension
2. Open `.http` files
3. Click "Send Request"

### Swagger UI
1. Navigate to `https://localhost:5001/`
2. Expand any endpoint
3. Click "Try it out"
4. Fill in parameters
5. Click "Execute"

---

## ⚠️ Important Notes

1. **In-Memory Storage**: All data is lost on restart
2. **Simple Auth**: Not production-ready (use JWT for production)
3. **No Authorization Middleware**: Endpoints are not protected
4. **Test Only**: Designed for development/testing purposes

---

## 📁 HTTP Test Files Location

Comprehensive test scenarios are in:
```
ApiTest.Testing/
├── Products/
│   ├── GetAllProducts/
│   │   ├── GetAllProducts.success.http
│   │   └── GetAllProducts.fail.http
│   ├── CreateProduct/
│   └── ...
├── Categories/
│   └── ...
└── Orders/
    └── ...
```

---

**Happy Testing! 🎉**

