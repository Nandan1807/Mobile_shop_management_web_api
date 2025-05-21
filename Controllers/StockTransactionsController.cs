using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mobile_shop_web_api.Data;
using mobile_shop_web_api.Models;

namespace mobile_shop_web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockTransactionsController : ControllerBase
    {
        private readonly StockTransactionRepository _stockTransactionRepository;

        public StockTransactionsController(StockTransactionRepository stockTransactionRepository)
        {
            _stockTransactionRepository = stockTransactionRepository;
        }

        // GET: api/StockTransactions
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetAllStockTransactions()
        {
            try
            {
                var stockTransactions = _stockTransactionRepository.GetAllStockTransactions();
                return Ok(stockTransactions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/StockTransactions/{productId}
        [Authorize(Roles = "Admin")]
        [HttpGet("product/{productId}")]
        public IActionResult GetStockTransactionsByProductId(int productId)
        {
            try
            {
                var stockTransactions = _stockTransactionRepository.GetStockTransactionsByProductId(productId);
                if (stockTransactions == null || !stockTransactions.Any())
                {
                    return NotFound($"No transactions found for Product ID {productId}.");
                }
                return Ok(stockTransactions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/StockTransactions
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AddStockTransaction([FromBody] StockTransactionModel stockTransaction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid stock transaction data." });
            }

            try
            {
                var result = _stockTransactionRepository.AddStockTransaction(stockTransaction);

                if (result.StartsWith("Error: Insufficient stock")) 
                {
                    return BadRequest(new { Message = result }); // Return 400 Bad Request for insufficient stock
                }
                else if (result.StartsWith("Database error") || result.StartsWith("Unexpected error"))
                {
                    return StatusCode(500, new { Message = result }); // Return 500 for database or system errors
                }

                return Ok(new { Message = result }); // Success
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Internal server error: {ex.Message}" });
            }
        }

        // PUT: api/StockTransactions/{id}
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult UpdateStockTransaction(int id, [FromBody] StockTransactionModel stockTransaction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid stock transaction data.");
            }

            if (id != stockTransaction.TransactionId)
            {
                return BadRequest("Stock Transaction ID mismatch.");
            }

            try
            {
                var result = _stockTransactionRepository.UpdateStockTransaction(stockTransaction);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/StockTransactions/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteStockTransaction(int id)
        {
            try
            {
                var result = _stockTransactionRepository.DeleteStockTransaction(id);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
