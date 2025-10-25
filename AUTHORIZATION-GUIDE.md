# Authorization Guide

## ✅ Authorization Implemented

All **write operations** (POST, PUT, PATCH, DELETE) now require authentication.  
**Read operations** (GET) remain public and don't require authentication.

---

## 🔒 Protected Endpoints

### Products
- ✅ **Public**: `GET /api/products` - Get all products
- ✅ **Public**: `GET /api/products/{id}` - Get product by ID
- 🔐 **Protected**: `POST /api/products` - Create product
- 🔐 **Protected**: `PUT /api/products/{id}` - Update product
- 🔐 **Protected**: `DELETE /api/products/{id}` - Delete product

### Categories
- ✅ **Public**: `GET /api/categories` - Get all categories
- ✅ **Public**: `GET /api/categories/{id}` - Get category by ID
- 🔐 **Protected**: `POST /api/categories` - Create category
- 🔐 **Protected**: `PUT /api/categories/{id}` - Update category
- 🔐 **Protected**: `DELETE /api/categories/{id}` - Delete category

### Orders
- ✅ **Public**: `GET /api/orders` - Get all orders
- ✅ **Public**: `GET /api/orders/{id}` - Get order by ID
- 🔐 **Protected**: `POST /api/orders` - Create order
- 🔐 **Protected**: `PATCH /api/orders/{id}/status` - Update order status
- 🔐 **Protected**: `DELETE /api/orders/{id}` - Delete order

### Authentication
- ✅ **Public**: `POST /api/auth/register` - Register new user
- ✅ **Public**: `POST /api/auth/login` - Login
- ✅ **Public**: `GET /api/auth/user/{username}` - Get user info
- ✅ **Public**: `GET /api/auth/test` - Test endpoint

---

## 🚀 How to Use Authorization

### Step 1: Login to Get Token

```http
POST https://localhost:5001/api/auth/login
Content-Type: application/json

{
  "username": "testuser",
  "password": "password123"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Login successful",
  "user": {
    "id": 1,
    "username": "testuser",
    "email": "test@example.com",
    "role": "User"
  },
  "token": "MTp0ZXN0dXNlcjpVc2VyOjYzODY4MjUwMjQ1NDMxMjAwMA=="
}
```

**Copy the `token` value!**

---

### Step 2: Use Token in Protected Requests

Add the token to the `Authorization` header:

```http
POST https://localhost:5001/api/products
Authorization: Bearer MTp0ZXN0dXNlcjpVc2VyOjYzODY4MjUwMjQ1NDMxMjAwMA==
Content-Type: application/json

{
  "name": "New Product",
  "description": "Created with auth",
  "price": 99.99,
  "stockQuantity": 50
}
```

---

## 📝 Complete Example Workflow

### 1. Try Without Authentication (Should Fail)

```http
POST https://localhost:5001/api/products
Content-Type: application/json

{
  "name": "Test Product",
  "description": "This should fail",
  "price": 50.00,
  "stockQuantity": 10
}
```

**Expected Response:** `401 Unauthorized`

---

### 2. Login First

```http
POST https://localhost:5001/api/auth/login
Content-Type: application/json

{
  "username": "testuser",
  "password": "password123"
}
```

**Save the token from response!**

---

### 3. Try Again With Token (Should Succeed)

```http
POST https://localhost:5001/api/products
Authorization: Bearer {YOUR_TOKEN_HERE}
Content-Type: application/json

{
  "name": "Test Product",
  "description": "This should work",
  "price": 50.00,
  "stockQuantity": 10
}
```

**Expected Response:** `201 Created` with product data

---

## 🧪 Testing with HTTP Files

### Visual Studio / VS Code

#### Method 1: Manual Token (Copy-Paste)

```http
### Step 1: Login and copy token
POST https://localhost:5001/api/auth/login
Content-Type: application/json

{
  "username": "testuser",
  "password": "password123"
}

###

### Step 2: Use token in protected request
POST https://localhost:5001/api/products
Authorization: Bearer MTp0ZXN0dXNlcjpVc2VyOjYzODY4MjUwMjQ1NDMxMjAwMA==
Content-Type: application/json

{
  "name": "Authorized Product",
  "description": "Created with auth token",
  "price": 99.99,
  "stockQuantity": 25
}
```

#### Method 2: Variables (Recommended for VS Code REST Client)

Create a file: `ApiTest.Testing/Auth/auth-examples.http`

```http
@baseUrl = https://localhost:5001
@token = MTp0ZXN0dXNlcjpVc2VyOjYzODY4MjUwMjQ1NDMxMjAwMA==

### Login
POST {{baseUrl}}/api/auth/login
Content-Type: application/json

{
  "username": "testuser",
  "password": "password123"
}

###

### Create Product (With Auth)
POST {{baseUrl}}/api/products
Authorization: Bearer {{token}}
Content-Type: application/json

{
  "name": "Authorized Product",
  "description": "Created with token",
  "price": 99.99,
  "stockQuantity": 50
}
```

---

## 🔑 Test Users

### User Account
```
Username: testuser
Password: password123
Role: User
```

### Admin Account
```
Username: admin
Password: admin123
Role: Admin
```

**Note**: Currently, both User and Admin have the same permissions. Role-based authorization can be added later if needed.

---

## 🎯 Testing Scenarios

### Success Scenarios

#### 1. Login → Create Product
```http
# 1. Login
POST /api/auth/login
{ "username": "testuser", "password": "password123" }

# 2. Copy token from response

# 3. Create product
POST /api/products
Authorization: Bearer {token}
{ "name": "Product", "description": "Test", "price": 50, "stockQuantity": 10 }

# Expected: 201 Created
```

#### 2. Login → Update Category
```http
# 1. Login first (get token)

# 2. Update category
PUT /api/categories/1
Authorization: Bearer {token}
{ "name": "Updated", "description": "Changed", "parentCategoryId": null }

# Expected: 200 OK
```

#### 3. Login → Delete Order
```http
# 1. Login first (get token)

# 2. Delete order
DELETE /api/orders/1
Authorization: Bearer {token}

# Expected: 204 No Content or 404 if not found
```

---

### Failure Scenarios

#### 1. Create Without Token
```http
POST /api/products
Content-Type: application/json

{ "name": "Product", "price": 50, "stockQuantity": 10 }

# Expected: 401 Unauthorized
```

#### 2. Invalid Token
```http
POST /api/products
Authorization: Bearer INVALID_TOKEN_HERE
Content-Type: application/json

{ "name": "Product", "price": 50, "stockQuantity": 10 }

# Expected: 401 Unauthorized
```

#### 3. Wrong Format (No "Bearer")
```http
POST /api/products
Authorization: MTp0ZXN0dXNlcjpVc2VyOjYzODY4MjUwMjQ1NDMxMjAwMA==
Content-Type: application/json

{ "name": "Product", "price": 50, "stockQuantity": 10 }

# Expected: 401 Unauthorized
```

---

## 🌐 Swagger UI Testing

### 1. Access Swagger
Navigate to: `https://localhost:5001/`

### 2. Login via Swagger
1. Find the **Auth** section
2. Click on `POST /api/auth/login`
3. Click "Try it out"
4. Enter credentials:
   ```json
   {
     "username": "testuser",
     "password": "password123"
   }
   ```
5. Click "Execute"
6. **Copy the token from the response**

### 3. Authorize in Swagger
1. Click the **"Authorize"** button at the top
2. Enter: `Bearer {YOUR_TOKEN}`
3. Click "Authorize"
4. Click "Close"

### 4. Test Protected Endpoints
Now you can execute any protected endpoint from Swagger!

---

## 📊 Response Status Codes

| Code | Meaning | When |
|------|---------|------|
| 200 | OK | Successful authenticated request |
| 201 | Created | Resource created successfully |
| 204 | No Content | Resource deleted successfully |
| 400 | Bad Request | Invalid request data |
| 401 | Unauthorized | Missing/invalid authentication |
| 404 | Not Found | Resource doesn't exist |

---

## ⚠️ Important Notes

### Security
- **For Testing Only**: This is a simple authentication system
- **Not Production-Ready**: Use JWT tokens in production
- **No Token Expiration**: Tokens don't expire currently
- **No Token Refresh**: No refresh token mechanism
- **Simple Hashing**: SHA256 (use BCrypt/Argon2 in production)

### Current Limitations
- Tokens don't expire
- No role-based permissions (User and Admin are treated the same)
- No password reset functionality
- No email verification
- Tokens are not validated against database (stateless)

### Future Enhancements
- Add JWT tokens with expiration
- Implement role-based authorization (e.g., only Admin can delete)
- Add token refresh mechanism
- Add password reset functionality
- Store sessions in database
- Add rate limiting

---

## 🔧 Troubleshooting

### "401 Unauthorized" Error

**Problem**: Getting 401 even with token

**Solutions**:
1. Check token format: `Authorization: Bearer {token}`
2. Ensure token is not expired (though current implementation doesn't expire)
3. Try logging in again to get a fresh token
4. Check if token was copied correctly (no extra spaces)

### "Missing Authorization header"

**Problem**: Request fails immediately

**Solution**: Add the Authorization header:
```
Authorization: Bearer YOUR_TOKEN_HERE
```

### Token Looks Wrong

**Problem**: Token is not Base64

**Solution**: Make sure you're copying the `token` field from login response, not the entire response

---

## ✅ Quick Test Checklist

- [ ] Login with testuser/password123
- [ ] Copy token from response
- [ ] Try GET /api/products (should work without token)
- [ ] Try POST /api/products without token (should fail with 401)
- [ ] Try POST /api/products WITH token (should succeed with 201)
- [ ] Try PUT /api/products/1 WITH token (should succeed)
- [ ] Try DELETE /api/products/1 WITH token (should succeed)

---

**Your API is now secured with authentication! 🔒**

Read operations are public, write operations require authentication.

