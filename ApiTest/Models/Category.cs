namespace ApiTest.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentCategoryId { get; set; }
    public Category? ParentCategory { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public record CreateCategoryRequest(
    string Name,
    string? Description,
    int? ParentCategoryId
);

public record UpdateCategoryRequest(
    string Name,
    string? Description,
    int? ParentCategoryId
);

