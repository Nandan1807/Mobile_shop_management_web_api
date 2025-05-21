using System.Data;
using Microsoft.Data.SqlClient;
using mobile_shop_web_api.Models;

namespace mobile_shop_web_api.Data
{
    public class SalesDashboardRepository
    {
        private readonly IConfiguration _configuration;

        public SalesDashboardRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region GetSalesTrend
        public List<SalesTrendModel> GetSalesTrend(int userId)
        {
            List<SalesTrendModel> salesTrend = new();
            string connectionString = _configuration.GetConnectionString("ConnectionString");

            using (SqlConnection connection = new(connectionString))
            {
                SqlCommand cmd = new("GetSalesTrend", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@user_id", userId);

                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    salesTrend.Add(new SalesTrendModel
                    {
                        SalesMonth = reader["SalesMonth"].ToString(),
                        TotalSales = Convert.ToDecimal(reader["TotalSales"])
                    });
                }
            }

            return salesTrend;
        }
        #endregion

        #region GetTopSellingProducts
        public List<TopSellingProductModel> GetTopSellingProducts(int userId)
        {
            List<TopSellingProductModel> topProducts = new();
            string connectionString = _configuration.GetConnectionString("ConnectionString");

            using (SqlConnection connection = new(connectionString))
            {
                SqlCommand cmd = new("GetTopSellingProducts", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@user_id", userId);

                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    topProducts.Add(new TopSellingProductModel
                    {
                        ProductName = reader["product_name"].ToString(),
                        TotalSold = Convert.ToInt32(reader["TotalSold"])
                    });
                }
            }

            return topProducts;
        }
        #endregion

        #region GetSalesByCategory
        public List<SalesByCategoryModel> GetSalesByCategory(int userId)
        {
            List<SalesByCategoryModel> salesByCategory = new();
            string connectionString = _configuration.GetConnectionString("ConnectionString");

            using (SqlConnection connection = new(connectionString))
            {
                SqlCommand cmd = new("GetSalesByCategory", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@user_id", userId);

                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    salesByCategory.Add(new SalesByCategoryModel
                    {
                        CategoryName = reader["category_name"].ToString(),
                        TotalRevenue = Convert.ToDecimal(reader["TotalRevenue"])
                    });
                }
            }

            return salesByCategory;
        }
        #endregion

        #region GetDailySales
        public List<DailySalesModel> GetDailySales(int userId)
        {
            List<DailySalesModel> dailySales = new();
            string connectionString = _configuration.GetConnectionString("ConnectionString");

            using (SqlConnection connection = new(connectionString))
            {
                SqlCommand cmd = new("GetDailySales", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@user_id", userId);

                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    dailySales.Add(new DailySalesModel
                    {
                        SalesDate = reader["SalesDate"].ToString(),
                        TotalSales = Convert.ToDecimal(reader["TotalSales"])
                    });
                }
            }

            return dailySales;
        }
        #endregion

        #region GetTopCustomers
        public List<TopCustomerModel> GetTopCustomers(int userId)
        {
            List<TopCustomerModel> topCustomers = new();
            string connectionString = _configuration.GetConnectionString("ConnectionString");

            using (SqlConnection connection = new(connectionString))
            {
                SqlCommand cmd = new("GetTopCustomers", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@user_id", userId);

                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    topCustomers.Add(new TopCustomerModel
                    {
                        CustomerName = reader["customer_name"].ToString(),
                        PurchaseCount = Convert.ToInt32(reader["PurchaseCount"])
                    });
                }
            }

            return topCustomers;
        }
        #endregion

        #region GetPaymentStatus
        public List<PaymentStatusModel> GetPaymentStatus(int userId)
        {
            List<PaymentStatusModel> paymentStatus = new();
            string connectionString = _configuration.GetConnectionString("ConnectionString");

            using (SqlConnection connection = new(connectionString))
            {
                SqlCommand cmd = new("GetPaymentStatus", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@user_id", userId);

                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    paymentStatus.Add(new PaymentStatusModel
                    {
                        PaymentStatus = reader["payment_status"].ToString(),
                        TotalInvoices = Convert.ToInt32(reader["TotalInvoices"])
                    });
                }
            }

            return paymentStatus;
        }
        #endregion
    }
}
