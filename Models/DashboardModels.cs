namespace mobile_shop_web_api.Models
{
    public class DashboardStatisticsModel
    {
        public int TotalCustomers { get; set; }
        public int TotalUsers { get; set; }
        public int TotalProducts { get; set; }
        public decimal TotalSales { get; set; }
    }

    public class LowStockProductModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int StockQuantity { get; set; }
    }

    public class CustomerPurchaseHistoryModel
    {
        public int InvoiceId { get; set; }
        public DateTime Date { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class SalesReportModel
    {
        public int InvoiceId { get; set; }
        public DateTime Date { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
    }
}