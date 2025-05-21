using System.Data;
using Microsoft.Data.SqlClient;
using mobile_shop_web_api.Models;

namespace mobile_shop_web_api.Data
{
    public class DashboardRepository
    {
        private readonly IConfiguration _configuration;

        public DashboardRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region FetchDashboardStatistics
        public DashboardStatisticsModel FetchDashboardStatistics()
        {
            DashboardStatisticsModel statistics = new();
            string connectionString = _configuration.GetConnectionString("ConnectionString");

            using (SqlConnection connection = new(connectionString))
            {
                SqlCommand cmd = new("FetchDashboardStatistics", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    statistics.TotalCustomers = Convert.ToInt32(reader["TotalCustomers"]);
                    statistics.TotalUsers = Convert.ToInt32(reader["TotalUsers"]);
                    statistics.TotalProducts = Convert.ToInt32(reader["TotalProducts"]);
                    statistics.TotalSales = Convert.ToDecimal(reader["TotalSales"]);
                }
            }

            return statistics;
        }
        #endregion

        #region GetLowStockProducts
        public List<LowStockProductModel> GetLowStockProducts(int threshold)
        {
            List<LowStockProductModel> lowStockProducts = new();
            string connectionString = _configuration.GetConnectionString("ConnectionString");

            using (SqlConnection connection = new(connectionString))
            {
                SqlCommand cmd = new("GetLowStockProducts", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@threshold", threshold);

                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lowStockProducts.Add(new LowStockProductModel
                    {
                        ProductId = Convert.ToInt32(reader["product_id"]),
                        ProductName = reader["product_name"].ToString(),
                        StockQuantity = Convert.ToInt32(reader["stock_quantity"])
                    });
                }
            }

            return lowStockProducts;
        }
        #endregion

        #region GetCustomerPurchaseHistory
        public List<CustomerPurchaseHistoryModel> GetCustomerPurchaseHistory(int customerId)
        {
            List<CustomerPurchaseHistoryModel> history = new();
            string connectionString = _configuration.GetConnectionString("ConnectionString");

            using (SqlConnection connection = new(connectionString))
            {
                SqlCommand cmd = new("GetCustomerPurchaseHistory", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@customer_id", customerId);

                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    history.Add(new CustomerPurchaseHistoryModel
                    {
                        InvoiceId = Convert.ToInt32(reader["invoice_id"]),
                        Date = Convert.ToDateTime(reader["date"]),
                        ProductId = Convert.ToInt32(reader["product_id"]),
                        ProductName = reader["product_name"].ToString(),
                        Quantity = Convert.ToInt32(reader["quantity"]),
                        TotalPrice = Convert.ToDecimal(reader["total_price"])
                    });
                }
            }

            return history;
        }
        #endregion

        #region GetSalesReport
        public List<SalesReportModel> GetSalesReport(DateTime startDate, DateTime endDate)
        {
            List<SalesReportModel> salesReport = new();
            string connectionString = _configuration.GetConnectionString("ConnectionString");

            using (SqlConnection connection = new(connectionString))
            {
                SqlCommand cmd = new("GetSalesReport", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@start_date", startDate);
                cmd.Parameters.AddWithValue("@end_date", endDate);

                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    salesReport.Add(new SalesReportModel
                    {
                        InvoiceId = Convert.ToInt32(reader["invoice_id"]),
                        Date = Convert.ToDateTime(reader["date"]),
                        CustomerName = reader["customer_name"].ToString(),
                        TotalAmount = Convert.ToDecimal(reader["total_amount"])
                    });
                }
            }

            return salesReport;
        }
        #endregion
    }
}
