using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mobile_shop_web_api.Data;
using mobile_shop_web_api.Models;

namespace mobile_shop_web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceItemsController : ControllerBase
    {
        private readonly InvoiceItemRepository _invoiceItemRepository;

        public InvoiceItemsController(InvoiceItemRepository invoiceItemRepository)
        {
            _invoiceItemRepository = invoiceItemRepository;
        }

        // GET: api/InvoiceItems
        [Authorize]
        [HttpGet]
        public IActionResult GetAllInvoiceItems()
        {
            try
            {
                var invoiceItems = _invoiceItemRepository.GetAllInvoiceItems();
                return Ok(invoiceItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/InvoiceItems/{invoiceId}
        [Authorize]
        [HttpGet("invoice/{invoiceId}")]
        public IActionResult GetInvoiceItemsByInvoiceId(int invoiceId)
        {
            try
            {
                var invoiceItems = _invoiceItemRepository.GetInvoiceItemsByInvoiceId(invoiceId);
                if (invoiceItems == null || !invoiceItems.Any())
                {
                    return NotFound($"No items found for Invoice ID {invoiceId}.");
                }
                return Ok(invoiceItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        [Authorize]
        [HttpGet("{invoiceItemId}")]
        public IActionResult GetInvoiceItemsByInvoiceItemId(int invoiceItemId)
        {
            try
            {
                var invoiceItems = _invoiceItemRepository.GetInvoiceItemsById(invoiceItemId);
                if (invoiceItems == null)
                {
                    return NotFound($"No items found for Invoice Item ID {invoiceItemId}.");
                }
                return Ok(invoiceItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/InvoiceItems
        [Authorize]
        [HttpPost]
        public IActionResult AddInvoiceItem([FromBody] InvoiceItemModel invoiceItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid invoice item data.");
            }

            try
            {
                var result = _invoiceItemRepository.AddInvoiceItem(invoiceItem);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/InvoiceItems/{id}
        [Authorize]
        [HttpPut("{id}")]
        public IActionResult UpdateInvoiceItem(int id, [FromBody] InvoiceItemModel invoiceItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid invoice item data.");
            }

            if (id != invoiceItem.InvoiceItemId)
            {
                return BadRequest("Invoice Item ID mismatch.");
            }

            try
            {
                var result = _invoiceItemRepository.UpdateInvoiceItem(invoiceItem);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/InvoiceItems/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult DeleteInvoiceItem(int id)
        {
            try
            {
                var result = _invoiceItemRepository.DeleteInvoiceItem(id);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
