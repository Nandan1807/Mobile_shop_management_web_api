using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mobile_shop_web_api.Data;
using mobile_shop_web_api.Models;

namespace mobile_shop_web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(ProductRepository productRepository, IConfiguration configuration) : ControllerBase
    {

        // GET: api/product
        [Authorize]
        [HttpGet]
        public ActionResult<List<ProductModel>> GetAllProducts([FromQuery] int? categoryId = null, [FromQuery] int? brandId = null)
        {
            try
            {
                var products = productRepository.GetAllProducts(categoryId, brandId);

                if (products == null || products.Count == 0)
                {
                    return NotFound("No products found.");
                }

                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        // GET: api/product/{id}
        [Authorize]
        [HttpGet("{id}")]
        public ActionResult<ProductModel> GetProductById(int id)
        {
            try
            {
                var product = productRepository.GetProductById(id);
                if (product == null)
                {
                    return NotFound($"Product with ID {id} not found.");
                }

                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/product
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<string>> AddProduct([FromForm] ProductModel product)
        {
            try
            {
                if (product == null)
                {
                    return BadRequest("Product data is required.");
                }

                var response = await productRepository.AddProduct(product);
                return CreatedAtAction(nameof(GetProductById), new { id = product.ProductId }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/product/{id}
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<string>> UpdateProduct(int id, [FromForm] ProductModel product)
        {
            try
            {
                if (product == null || product.ProductId != id)
                {
                    return BadRequest("Product data is invalid or ID mismatch.");
                }

                var response = await productRepository.UpdateProduct(product);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/product/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public ActionResult<string> DeleteProduct(int id)
        {
            try
            {
                var response = productRepository.DeleteProduct(id);
                if (response == "Failed to delete product")
                {
                    return NotFound($"Product with ID {id} not found.");
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
