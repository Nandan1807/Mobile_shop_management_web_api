using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mobile_shop_web_api.Data;
using mobile_shop_web_api.Models;

namespace mobile_shop_web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly CategoryRepository _categoryRepository;

        public CategoriesController(CategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        // GET: api/Categories
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetAllCategories()
        {
            try
            {
                var categories = _categoryRepository.GetAllCategories();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Categories/{id}
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public IActionResult GetCategoryById(int id)
        {
            try
            {
                var category = _categoryRepository.GetCategoryById(id);
                if (category == null)
                {
                    return NotFound($"Category with ID {id} not found.");
                }
                return Ok(category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/Categories
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AddCategory([FromBody] CategoryModel category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid category data.");
            }

            try
            {
                var result = _categoryRepository.AddCategory(category);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/Categories/{id}
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult UpdateCategory(int id, [FromBody] CategoryModel category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid category data.");
            }

            if (id != category.CategoryId)
            {
                return BadRequest("Category ID mismatch.");
            }

            try
            {
                var result = _categoryRepository.UpdateCategory(category);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/Categories/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(int id)
        {
            try
            {
                var result = _categoryRepository.DeleteCategory(id);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
