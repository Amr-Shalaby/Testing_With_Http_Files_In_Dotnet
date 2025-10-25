using ApiTest.Models;

namespace ApiTest.Services;

public interface ICategoryService
{
    Task<IEnumerable<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(int id);
    Task<Category> CreateAsync(CreateCategoryRequest request);
    Task<Category?> UpdateAsync(int id, UpdateCategoryRequest request);
    Task<bool> DeleteAsync(int id);
}

