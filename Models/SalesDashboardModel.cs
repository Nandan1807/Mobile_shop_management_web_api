namespace mobile_shop_web_api.Models
{
    public class SalesTrendModel
    {
        public string SalesMonth { get; set; }
        public decimal TotalSales { get; set; }
    }
    public class TopSellingProductModel
    {
        public string ProductName { get; set; }
        public int TotalSold { get; set; }
    }
    
    public class SalesByCategoryModel
    {
        public string CategoryName { get; set; }
        public decimal TotalRevenue { get; set; }
    }
    public class DailySalesModel
    {
        public string SalesDate { get; set; }
        public decimal TotalSales { get; set; }
    }

    public class TopCustomerModel
    {
        public string CustomerName { get; set; }
        public int PurchaseCount { get; set; }
    }

    public class PaymentStatusModel
    {
        public string PaymentStatus { get; set; }
        public int TotalInvoices { get; set; }
    }

}