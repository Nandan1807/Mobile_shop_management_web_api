using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mobile_shop_web_api.Data;

namespace mobile_shop_web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly DashboardRepository _dashboardRepository;

        public DashboardController(DashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }
        
        [Authorize(Roles = "Admin")]
        [HttpGet("statistics")]
        public IActionResult FetchDashboardStatistics()
        {
            try
            {
                var statistics = _dashboardRepository.FetchDashboardStatistics();
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("low-stock-products/{threshold}")]
        public IActionResult GetLowStockProducts(int threshold)
        {
            try
            {
                var products = _dashboardRepository.GetLowStockProducts(threshold);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("customer-purchase-history/{customerId}")]
        public IActionResult GetCustomerPurchaseHistory(int customerId)
        {
            try
            {
                var history = _dashboardRepository.GetCustomerPurchaseHistory(customerId);
                return Ok(history);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("sales-report")]
        public IActionResult GetSalesReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var salesReport = _dashboardRepository.GetSalesReport(startDate, endDate);
                return Ok(salesReport);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
