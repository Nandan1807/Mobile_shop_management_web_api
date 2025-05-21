using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mobile_shop_web_api.Data;
using mobile_shop_web_api.Models;

namespace mobile_shop_web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly InvoiceRepository _invoiceRepository;

        public InvoicesController(InvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        // GET: api/Invoices
        [Authorize]
        [HttpGet]
        public IActionResult GetAllInvoices(int? customerId = null, string status = null)
        {
            try
            {
                // Extract UserId from JWT claims (since it's stored as "UserId" instead of NameIdentifier)
                var userIdClaim = User.FindFirst("UserId");
                if (userIdClaim == null)
                {
                    return Unauthorized("User ID not found in token.");
                }

                int userId = Convert.ToInt32(userIdClaim.Value);

                // Fetch invoices based on user role logic
                var invoices = _invoiceRepository.GetAllInvoices(userId, customerId, status);

                return Ok(invoices);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        // GET: api/Invoices/{id}
        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetInvoiceById(int id)
        {
            try
            {
                var invoice = _invoiceRepository.GetInvoiceById(id);
                if (invoice == null)
                {
                    return NotFound($"Invoice with ID {id} not found.");
                }
                return Ok(invoice);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        // POST: api/Invoices
        [HttpPost]
        public IActionResult AddInvoice([FromBody] InvoiceModel invoice)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid invoice data." });
            }

            try
            {
                int invoiceId = _invoiceRepository.AddInvoice(invoice);

                return Ok(new { success = true, message = "Invoice added successfully", InvoiceId = invoiceId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Internal server error: {ex.Message}" });
            }
        }


        // PUT: api/Invoices/{id}
        [Authorize]
        [HttpPut("{id}")]
        public IActionResult UpdateInvoice(int id, [FromBody] InvoiceModel invoice)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid invoice data.");
            }

            if (id != invoice.InvoiceId)
            {
                return BadRequest("Invoice ID mismatch.");
            }

            try
            {
                var result = _invoiceRepository.UpdateInvoice(invoice);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/Invoices/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult DeleteInvoice(int id)
        {
            try
            {
                var result = _invoiceRepository.DeleteInvoice(id);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
