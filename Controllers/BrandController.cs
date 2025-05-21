using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mobile_shop_web_api.Data;
using mobile_shop_web_api.Models;

namespace mobile_shop_web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly BrandRepository _brandRepository;

        public BrandsController(BrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }

        // GET: api/Brands
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetAllBrands()
        {
            try
            {
                var brands = _brandRepository.GetAllBrands();
                return Ok(brands);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Brands/{id}
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public IActionResult GetBrandById(int id)
        {
            try
            {
                var brand = _brandRepository.GetBrandById(id);
                if (brand == null)
                {
                    return NotFound($"Brand with ID {id} not found.");
                }
                return Ok(brand);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/Brands
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AddBrand([FromBody] BrandModel brand)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid brand data.");
            }

            try
            {
                var result = _brandRepository.AddBrand(brand);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/Brands/{id}
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult UpdateBrand(int id, [FromBody] BrandModel brand)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid brand data.");
            }

            if (id != brand.BrandId)
            {
                return BadRequest("Brand ID mismatch.");
            }

            try
            {
                var result = _brandRepository.UpdateBrand(brand);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/Brands/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteBrand(int id)
        {
            try
            {
                var result = _brandRepository.DeleteBrand(id);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
