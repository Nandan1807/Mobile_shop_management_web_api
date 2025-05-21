using System.Data;
using Microsoft.Data.SqlClient;
using mobile_shop_web_api.Models;

namespace mobile_shop_web_api.Data
{
    public class StockTransactionRepository
    {
        private readonly IConfiguration _configuration;

        public StockTransactionRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region GetAllStockTransactions
        public List<StockTransactionModel> GetAllStockTransactions()
        {
            List<StockTransactionModel> stockTransactionModels = new List<StockTransactionModel>();
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Stock_Transactions_Select_All", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    stockTransactionModels.Add(
                        new StockTransactionModel
                        {
                            TransactionId = Convert.ToInt32(reader["transaction_id"]),
                            ProductId = Convert.ToInt32(reader["product_id"]),
                            StockQuantity = Convert.ToInt32(reader["stock_quantity"]),
                            Date = Convert.ToDateTime(reader["date"]),
                            TransactionState = reader["transaction_state"].ToString(),
                            TransactionDescription = reader["transaction_description"].ToString(),
                            UserId = Convert.ToInt32(reader["user_id"]),
                            ProductName = reader["product_name"].ToString(),
                            UserName = reader["user_name"].ToString()
                        }
                    );
                }
            }

            return stockTransactionModels;
        }
        #endregion

        #region GetStockTransactionsByProductId
        public List<StockTransactionModel> GetStockTransactionsByProductId(int productId)
        {
            List<StockTransactionModel> stockTransactionModels = new List<StockTransactionModel>();
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Stock_Transactions_Select_By_ProductId", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@ProductId", productId);

                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    stockTransactionModels.Add(
                        new StockTransactionModel
                        {
                            TransactionId = Convert.ToInt32(reader["transaction_id"]),
                            ProductId = Convert.ToInt32(reader["product_id"]),
                            StockQuantity = Convert.ToInt32(reader["stock_quantity"]),
                            Date = Convert.ToDateTime(reader["date"]),
                            TransactionState = reader["transaction_state"].ToString(),
                            TransactionDescription = reader["transaction_description"].ToString(),
                            UserId = Convert.ToInt32(reader["user_id"]),
                            UserName = reader["user_name"].ToString()
                        }
                    );
                }
            }

            return stockTransactionModels;
        }
        #endregion

        #region AddStockTransaction
        public string AddStockTransaction(StockTransactionModel stockTransaction)
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Stock_Transactions_Insert", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@ProductId", stockTransaction.ProductId);
                cmd.Parameters.AddWithValue("@StockQuantity", stockTransaction.StockQuantity);
                cmd.Parameters.AddWithValue("@TransactionState", stockTransaction.TransactionState);
                cmd.Parameters.AddWithValue("@TransactionDescription", stockTransaction.TransactionDescription ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@UserId", stockTransaction.UserId);

                try
                {
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    return "Stock Transaction added successfully";
                }
                catch (SqlException ex)
                {
                    // Detect if the error is related to insufficient stock
                    if (ex.Message.Contains("Insufficient stock"))
                    {
                        return "Error: " + ex.Message; // Return error message to controller
                    }
                    else
                    {
                        return "Database error: " + ex.Message;
                    }
                }
                catch (Exception ex)
                {
                    return "Unexpected error: " + ex.Message;
                }
            }
        }
        #endregion


        #region UpdateStockTransaction
        public string UpdateStockTransaction(StockTransactionModel stockTransaction)
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Stock_Transactions_Update", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@TransactionId", stockTransaction.TransactionId);
                cmd.Parameters.AddWithValue("@ProductId", stockTransaction.ProductId);
                cmd.Parameters.AddWithValue("@StockQuantity", stockTransaction.StockQuantity);
                cmd.Parameters.AddWithValue("@TransactionState", stockTransaction.TransactionState);
                cmd.Parameters.AddWithValue("@TransactionDescription", stockTransaction.TransactionDescription);
                cmd.Parameters.AddWithValue("@UserId", stockTransaction.UserId);

                connection.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0 ? "Stock Transaction updated successfully" : "Failed to update Stock Transaction";
            }
        }
        #endregion

        #region DeleteStockTransaction
        public string DeleteStockTransaction(int transactionId)
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Stock_Transactions_Delete", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@TransactionId", transactionId);

                connection.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0 ? "Stock Transaction deleted successfully" : "Failed to delete Stock Transaction";
            }
        }
        #endregion
    }
}
