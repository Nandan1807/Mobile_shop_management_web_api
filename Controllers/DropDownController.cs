using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mobile_shop_web_api.Data;

namespace mobile_shop_web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DropdownController : ControllerBase
    {
        private readonly DropdownRepository _dropdownRepository;

        public DropdownController(DropdownRepository dropdownRepository)
        {
            _dropdownRepository = dropdownRepository;
        }

        // GET: api/Dropdown/Categories
        [Authorize]
        [HttpGet("Categories")]
        public IActionResult GetCategories()
        {
            try
            {
                var categories = _dropdownRepository.GetCategoriesForDropdown();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Dropdown/Brands
        [Authorize]
        [HttpGet("Brands")]
        public IActionResult GetBrands()
        {
            try
            {
                var brands = _dropdownRepository.GetBrandsForDropdown();
                return Ok(brands);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Dropdown/Customers
        [Authorize]
        [HttpGet("Customers")]
        public IActionResult GetCustomers()
        {
            try
            {
                var customers = _dropdownRepository.GetCustomersForDropdown();
                return Ok(customers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Dropdown/Users
        [Authorize]
        [HttpGet("Users")]
        public IActionResult GetUsers()
        {
            try
            {
                var users = _dropdownRepository.GetUsersForDropdown();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Dropdown/Products
        [Authorize]
        [HttpGet("Products")]
        public IActionResult GetProducts()
        {
            try
            {
                var products = _dropdownRepository.GetProductsForDropdown();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
