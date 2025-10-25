using ApiTest.Models;

namespace ApiTest.Services;

public class CategoryService : ICategoryService
{
    private readonly List<Category> _categories = new();
    private int _nextId = 1;

    public CategoryService()
    {
        // Seed with sample data
        _categories.AddRange(new[]
        {
            new Category
            {
                Id = _nextId++,
                Name = "Electronics",
                Description = "Electronic devices and accessories",
                ParentCategoryId = null,
                CreatedAt = DateTime.UtcNow
            },
            new Category
            {
                Id = _nextId++,
                Name = "Computers",
                Description = "Desktop and laptop computers",
                ParentCategoryId = 1,
                CreatedAt = DateTime.UtcNow
            },
            new Category
            {
                Id = _nextId++,
                Name = "Books",
                Description = "Physical and digital books",
                ParentCategoryId = null,
                CreatedAt = DateTime.UtcNow
            }
        });
    }

    public Task<IEnumerable<Category>> GetAllAsync()
    {
        return Task.FromResult(_categories.AsEnumerable());
    }

    public Task<Category?> GetByIdAsync(int id)
    {
        var category = _categories.FirstOrDefault(c => c.Id == id);
        return Task.FromResult(category);
    }

    public Task<Category> CreateAsync(CreateCategoryRequest request)
    {
        // Validate parent category exists if specified
        if (request.ParentCategoryId.HasValue)
        {
            var parentExists = _categories.Any(c => c.Id == request.ParentCategoryId.Value);
            if (!parentExists)
            {
                throw new InvalidOperationException($"Parent category with ID {request.ParentCategoryId} not found");
            }
        }

        var category = new Category
        {
            Id = _nextId++,
            Name = request.Name,
            Description = request.Description,
            ParentCategoryId = request.ParentCategoryId,
            CreatedAt = DateTime.UtcNow
        };

        _categories.Add(category);
        return Task.FromResult(category);
    }

    public Task<Category?> UpdateAsync(int id, UpdateCategoryRequest request)
    {
        var category = _categories.FirstOrDefault(c => c.Id == id);

        if (category == null)
        {
            return Task.FromResult<Category?>(null);
        }

        // Validate parent category exists if specified
        if (request.ParentCategoryId.HasValue)
        {
            var parentExists = _categories.Any(c => c.Id == request.ParentCategoryId.Value);
            if (!parentExists)
            {
                throw new InvalidOperationException($"Parent category with ID {request.ParentCategoryId} not found");
            }
        }

        category.Name = request.Name;
        category.Description = request.Description;
        category.ParentCategoryId = request.ParentCategoryId;
        category.UpdatedAt = DateTime.UtcNow;

        return Task.FromResult<Category?>(category);
    }

    public Task<bool> DeleteAsync(int id)
    {
        var category = _categories.FirstOrDefault(c => c.Id == id);

        if (category == null)
        {
            return Task.FromResult(false);
        }

        // Check if any categories have this as parent
        var hasChildren = _categories.Any(c => c.ParentCategoryId == id);
        if (hasChildren)
        {
            throw new InvalidOperationException("Cannot delete category with subcategories");
        }

        _categories.Remove(category);
        return Task.FromResult(true);
    }
}

