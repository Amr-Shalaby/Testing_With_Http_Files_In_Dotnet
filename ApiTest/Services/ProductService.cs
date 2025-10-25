using ApiTest.Models;

namespace ApiTest.Services;

public class ProductService : IProductService
{
    private readonly List<Product> _products = new();
    private int _nextId = 1;

    public ProductService()
    {
        // Seed with some sample data
        _products.AddRange(new[]
        {
            new Product
            {
                Id = _nextId++,
                Name = "Laptop",
                Description = "High-performance laptop",
                Price = 999.99m,
                StockQuantity = 10,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Id = _nextId++,
                Name = "Mouse",
                Description = "Wireless mouse",
                Price = 29.99m,
                StockQuantity = 50,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Id = _nextId++,
                Name = "Keyboard",
                Description = "Mechanical keyboard",
                Price = 79.99m,
                StockQuantity = 30,
                CreatedAt = DateTime.UtcNow
            }
        });
    }

    public Task<IEnumerable<Product>> GetAllAsync()
    {
        return Task.FromResult(_products.AsEnumerable());
    }

    public Task<Product?> GetByIdAsync(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(product);
    }

    public Task<Product> CreateAsync(CreateProductRequest request)
    {
        var product = new Product
        {
            Id = _nextId++,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            CreatedAt = DateTime.UtcNow
        };

        _products.Add(product);
        return Task.FromResult(product);
    }

    public Task<Product?> UpdateAsync(int id, UpdateProductRequest request)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);

        if (product == null)
        {
            return Task.FromResult<Product?>(null);
        }

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.StockQuantity = request.StockQuantity;
        product.UpdatedAt = DateTime.UtcNow;

        return Task.FromResult<Product?>(product);
    }

    public Task<bool> DeleteAsync(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);

        if (product == null)
        {
            return Task.FromResult(false);
        }

        _products.Remove(product);
        return Task.FromResult(true);
    }
}

