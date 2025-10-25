using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ApiTest.Models;
using ApiTest.Services;

namespace ApiTest.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(ICategoryService categoryService, ILogger<CategoriesController> logger)
    {
        _categoryService = categoryService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all categories
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Category>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Category>>> GetAll()
    {
        _logger.LogInformation("Getting all categories");
        var categories = await _categoryService.GetAllAsync();
        return Ok(categories);
    }

    /// <summary>
    /// Gets a specific category by id
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Category), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Category>> GetById(int id)
    {
        _logger.LogInformation("Getting category with id {CategoryId}", id);
        var category = await _categoryService.GetByIdAsync(id);

        if (category == null)
        {
            return NotFound();
        }

        return Ok(category);
    }

    /// <summary>
    /// Creates a new category
    /// </summary>
    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(Category), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Category>> Create([FromBody] CreateCategoryRequest request)
    {
        _logger.LogInformation("Creating new category: {CategoryName}", request.Name);

        try
        {
            var category = await _categoryService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to create category");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Updates an existing category
    /// </summary>
    [Authorize]
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Category), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Category>> Update(int id, [FromBody] UpdateCategoryRequest request)
    {
        _logger.LogInformation("Updating category with id {CategoryId}", id);

        try
        {
            var category = await _categoryService.UpdateAsync(id, request);

            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to update category");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Deletes a category
    /// </summary>
    [Authorize]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> Delete(int id)
    {
        _logger.LogInformation("Deleting category with id {CategoryId}", id);

        try
        {
            var success = await _categoryService.DeleteAsync(id);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to delete category");
            return BadRequest(new { error = ex.Message });
        }
    }
}

