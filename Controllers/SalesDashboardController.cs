using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mobile_shop_web_api.Data;
using System;

namespace mobile_shop_web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesDashboardController : ControllerBase
    {
        private readonly SalesDashboardRepository _salesDashboardRepository;

        public SalesDashboardController(SalesDashboardRepository salesDashboardRepository)
        {
            _salesDashboardRepository = salesDashboardRepository;
        }
        
        [Authorize]
        [HttpGet("sales-trend/{userId}")]
        public IActionResult GetSalesTrend(int userId)
        {
            try
            {
                var salesTrend = _salesDashboardRepository.GetSalesTrend(userId);
                return Ok(salesTrend);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        
        [Authorize]
        [HttpGet("top-selling-products/{userId}")]
        public IActionResult GetTopSellingProducts(int userId)
        {
            try
            {
                var products = _salesDashboardRepository.GetTopSellingProducts(userId);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        
        [Authorize]
        [HttpGet("sales-by-category/{userId}")]
        public IActionResult GetSalesByCategory(int userId)
        {
            try
            {
                var categories = _salesDashboardRepository.GetSalesByCategory(userId);
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        
        [Authorize]
        [HttpGet("daily-sales/{userId}")]
        public IActionResult GetDailySales(int userId)
        {
            try
            {
                var dailySales = _salesDashboardRepository.GetDailySales(userId);
                return Ok(dailySales);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        
        [Authorize]
        [HttpGet("top-customers/{userId}")]
        public IActionResult GetTopCustomers(int userId)
        {
            try
            {
                var customers = _salesDashboardRepository.GetTopCustomers(userId);
                return Ok(customers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        
        [Authorize]
        [HttpGet("payment-status/{userId}")]
        public IActionResult GetPaymentStatus(int userId)
        {
            try
            {
                var payments = _salesDashboardRepository.GetPaymentStatus(userId);
                return Ok(payments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}