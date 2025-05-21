namespace mobile_shop_web_api.Models
{
    public class StockTransactionModel
    {
        public int TransactionId { get; set; }
        public int ProductId { get; set; }
        public int StockQuantity { get; set; }
        public DateTime Date { get; set; }
        public string TransactionState { get; set; }
        public string TransactionDescription { get; set; }
        public int UserId { get; set; }
        public string? ProductName { get; set; }
        public string? UserName { get; set; }
    }
}