# API with Testing

A .NET 9.0 Web API project with comprehensive HTTP-based testing approach for maintainable, long-term API testing.

## 🚀 Projects

### ApiTest
The main Web API project featuring:
- **RESTful API** with Controllers
- **Swagger/OpenAPI** documentation (available at root `/` in development)
- **Minimal API** endpoints
- **Dependency Injection** setup
- **Sample ProductsController** with full CRUD operations
- **Logging** configuration

### ApiTest.Testing
The modular HTTP testing collection featuring:
- **98+ HTTP request scenarios** organized by module/action/scenario
- **Modular structure** - Products, Categories (example), Orders (example)
- **Action-based organization** - Each endpoint operation in its own folder
- **Success/Fail separation** - `.success.http` and `.fail.http` files
- **Tool-agnostic** - works with Visual Studio, VS Code (REST Client), Rider
- **Environment configuration** support for dev/local/staging/production
- **No package dependencies** - pure HTTP request files
- **Scalable design** for long-term maintenance and growth

## 📋 Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Visual Studio 2022 / VS Code / Rider (optional)

## 🛠️ Getting Started

### 1. Restore Dependencies

```powershell
dotnet restore
```

### 2. Build the Solution

```powershell
dotnet build
```

### 3. Run the API

```powershell
cd ApiTest
dotnet run
```

The API will start at:
- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`
- Swagger UI: `https://localhost:5001/` (in development mode)

### 4. Test the API

Navigate to the `ApiTest.Testing` folder structure:

```powershell
# Open the testing project
cd ApiTest.Testing

# View available modules
dir

# Explore Products module (fully implemented)
dir Products
```

**Quick Start**: Open `ApiTest.Testing/QUICK-START.md` for a 5-minute walkthrough.

**Full Documentation**: See `ApiTest.Testing/README.md` for comprehensive guide.

Use your preferred HTTP client tool:
- **Visual Studio**: Open any `.http` file and click the green play button
- **VS Code**: Install REST Client extension and click "Send Request"
- **Rider/IntelliJ**: Use built-in HTTP client
- **Swagger UI**: Navigate to `https://localhost:5001/` when API is running

#### Modular Structure Example
```
Products/
├── GetAllProducts/
│   ├── GetAllProducts.success.http    (happy path tests)
│   └── GetAllProducts.fail.http       (error handling tests)
├── CreateProduct/
│   ├── CreateProduct.success.http
│   └── CreateProduct.fail.http
└── ...
```

## 📚 API Endpoints

### Products API

- `GET /api/products` - Get all products
- `GET /api/products/{id}` - Get a specific product
- `POST /api/products` - Create a new product
- `PUT /api/products/{id}` - Update an existing product
- `DELETE /api/products/{id}` - Delete a product

### Weather Forecast (Sample Minimal API)

- `GET /api/weatherforecast` - Get weather forecast data

## 🧪 Testing Structure

### Modular HTTP Test Collection (`ApiTest.Testing/`)
Organized by Module → Action → Success/Fail scenarios:

```
ApiTest.Testing/
├── Products/                   # Products Module (58 tests - IMPLEMENTED)
│   ├── GetAllProducts/         # Action folders
│   ├── GetProductById/
│   ├── CreateProduct/
│   ├── UpdateProduct/
│   ├── DeleteProduct/
│   └── README.md
├── Categories/                 # Example module (13 tests)
│   └── README.md
├── Orders/                     # Example module (27 tests)
│   └── README.md
├── http-client.env.json        # Environment configs
├── QUICK-START.md              # 5-minute tutorial
└── README.md                   # Full documentation
```

### Test Organization
- **Modules**: Feature areas (Products, Orders, Categories)
- **Actions**: Endpoint operations (GetAll, Create, Update, Delete)
- **Success files** (`.success.http`): Happy path scenarios (200, 201, 204)
- **Fail files** (`.fail.http`): Error scenarios (400, 404, 409, 500)

### Total Coverage: 98+ Test Scenarios
- Products: 58 tests (20 success, 38 failure)
- Categories: 13 tests (example)
- Orders: 27 tests (example)

## 📦 Key Dependencies

### ApiTest
- Microsoft.AspNetCore.OpenApi (9.0.9)
- Swashbuckle.AspNetCore (7.0.0)

### ApiTest.Testing
- **No package dependencies** - contains only HTTP request files
- Compatible with any HTTP client tool

## 🏗️ Project Structure

```
ApiWithTesting/
├── ApiTest/                          # Main API Project
│   ├── Controllers/
│   │   └── ProductsController.cs
│   ├── Models/
│   │   └── Product.cs
│   ├── Services/
│   │   ├── IProductService.cs
│   │   └── ProductService.cs
│   ├── Program.cs
│   └── appsettings.json
├── ApiTest.Testing/                  # Modular HTTP Test Collection
│   ├── Products/                     # Products Module
│   │   ├── GetAllProducts/
│   │   │   ├── GetAllProducts.success.http
│   │   │   └── GetAllProducts.fail.http
│   │   ├── CreateProduct/
│   │   │   ├── CreateProduct.success.http
│   │   │   └── CreateProduct.fail.http
│   │   ├── ...                      # (5 actions total)
│   │   └── README.md
│   ├── Categories/                   # Example Module
│   │   └── README.md
│   ├── Orders/                       # Example Module
│   │   └── README.md
│   ├── http-client.env.json
│   ├── QUICK-START.md
│   └── README.md
└── README.md                         # This file
```

## 🔧 Development Tips

### Adding New Endpoints

1. Create a new controller in `ApiTest/Controllers/`
2. Create or update module in `ApiTest.Testing/{ModuleName}/`
3. Create action folder: `{ModuleName}/{ActionName}/`
4. Add HTTP test files: `{ActionName}.success.http` and `{ActionName}.fail.http`
5. Register any new services in `Program.cs`

### Adding New Modules

1. Create module folder: `ApiTest.Testing/{ModuleName}/`
2. Create action folder: `{ModuleName}/{ActionName}/`
3. Add test files: `{ActionName}.success.http` and `{ActionName}.fail.http`
4. Create module README: `{ModuleName}/README.md`
5. Update main testing README with new module stats

### Using Swagger

Navigate to `https://localhost:5001/` when running in development mode to access the interactive Swagger UI documentation.

## 📊 Testing Best Practices

### Daily Smoke Testing (5 minutes)
Run success scenarios from critical modules:
1. `Products/GetAllProducts/GetAllProducts.success.http`
2. `Products/CreateProduct/CreateProduct.success.http`
3. Other module success tests as needed

### Full Regression (Before Release)
Execute all `.success.http` AND `.fail.http` files across all modules

### Modular Test Structure
```
Module → Action → Success/Fail
   ↓        ↓          ↓
Products → Create → CreateProduct.success.http (happy paths)
                  → CreateProduct.fail.http (error cases)
```

### Adding New Modules
1. Create folder: `ApiTest.Testing/{ModuleName}/`
2. Add actions with success/fail files
3. Create module README
4. Update main README stats

### Maintenance Schedule
- **Daily**: Run smoke tests
- **Weekly**: Review and update tests
- **Per Release**: Full regression suite
- **Monthly**: Clean up test data, update docs

## 🐛 Debugging

### Visual Studio
1. Set `ApiTest` as the startup project
2. Press F5 to start debugging

### VS Code
1. Open the solution folder
2. Use the provided launch configurations
3. Press F5 to start debugging

### Command Line
```powershell
cd ApiTest
dotnet run --launch-profile https
```

## 📝 Notes

- The `ProductService` currently uses in-memory storage for demonstration purposes
- In production, you would replace this with a proper database implementation
- The API uses singleton service lifetime for `ProductService` to maintain state during runtime
- All tests are isolated and don't affect each other

## 🤝 Contributing

1. Create a new branch for your feature
2. Write tests first (TDD approach recommended)
3. Implement the feature
4. Ensure all tests pass
5. Submit a pull request

## 📄 License

This is a sample project for demonstration purposes.

---

**Happy Coding! 🎉**

