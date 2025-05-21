using System.Data;
using Microsoft.Data.SqlClient;
using mobile_shop_web_api.Models;

namespace mobile_shop_web_api.Data
{
    public class InvoiceRepository
    {
        private readonly IConfiguration _configuration;

        public InvoiceRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region GetAllInvoices
        public List<InvoiceModel> GetAllInvoices(int userId, int? customerId = null, string status = null)
        {
            List<InvoiceModel> invoiceModels = new List<InvoiceModel>();
            string connectionString = _configuration.GetConnectionString("ConnectionString");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("PR_Invoices_Select_All", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add parameters for stored procedure
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@CustomerId", customerId.HasValue ? (object)customerId.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@Status", !string.IsNullOrEmpty(status) ? (object)status : DBNull.Value);

                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            invoiceModels.Add(new InvoiceModel
                            {
                                InvoiceId = Convert.ToInt32(reader["invoice_id"]),
                                CustomerId = Convert.ToInt32(reader["customer_id"]),
                                UserId = Convert.ToInt32(reader["user_id"]),
                                Date = Convert.ToDateTime(reader["date"]),
                                TotalAmount = Convert.ToDecimal(reader["total_amount"]),
                                CustomerName = reader["customer_name"].ToString(),
                                UserName = reader["user_name"].ToString(),
                                PaymentMethod = reader["payment_method"].ToString(),
                                PaymentStatus = reader["payment_status"].ToString()
                            });
                        }
                    }
                }
            }

            return invoiceModels;
        }
        #endregion

        #region GetInvoiceById
        public InvoiceModel GetInvoiceById(int invoiceId)
        {
            InvoiceModel invoice = null;
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Invoices_Select_By_Id", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@InvoiceId", invoiceId);

                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    invoice = new InvoiceModel
                    {
                        InvoiceId = Convert.ToInt32(reader["invoice_id"]),
                        CustomerId = Convert.ToInt32(reader["customer_id"]),
                        UserId = Convert.ToInt32(reader["user_id"]),
                        Date = Convert.ToDateTime(reader["date"]),
                        CustomerName = reader["customer_name"].ToString(),
                        UserName = reader["user_name"].ToString(),
                        TotalAmount = Convert.ToDecimal(reader["total_amount"]),
                        PaymentMethod = reader["payment_method"].ToString(),
                        PaymentStatus = reader["payment_status"].ToString()
                    };
                }
            }

            return invoice;
        }
        #endregion

        #region AddInvoice
        public int AddInvoice(InvoiceModel invoice)
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Invoices_Insert", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@CustomerId", invoice.CustomerId);
                cmd.Parameters.AddWithValue("@UserId", invoice.UserId);
                cmd.Parameters.AddWithValue("@TotalAmount", invoice.TotalAmount);
                cmd.Parameters.AddWithValue("@PaymentMethod", invoice.PaymentMethod);
                cmd.Parameters.AddWithValue("@PaymentStatus", invoice.PaymentStatus);

                SqlParameter outputIdParam = new SqlParameter("@InvoiceId", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outputIdParam);

                connection.Open();
                cmd.ExecuteNonQuery();

                // Retrieve the output InvoiceId
                int newInvoiceId = (int)outputIdParam.Value;
                return newInvoiceId;
            }
        }
        #endregion

        #region UpdateInvoice
        public string UpdateInvoice(InvoiceModel invoice)
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Invoices_Update", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@InvoiceId", invoice.InvoiceId);
                cmd.Parameters.AddWithValue("@CustomerId", invoice.CustomerId);
                cmd.Parameters.AddWithValue("@UserId", invoice.UserId);
                cmd.Parameters.AddWithValue("@TotalAmount", invoice.TotalAmount);
                cmd.Parameters.AddWithValue("@PaymentMethod", invoice.PaymentMethod);
                cmd.Parameters.AddWithValue("@PaymentStatus", invoice.PaymentStatus);

                connection.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0 ? "Invoice updated successfully" : "Failed to update invoice";
            }
        }
        #endregion

        #region DeleteInvoice
        public string DeleteInvoice(int invoiceId)
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Invoices_Delete", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@InvoiceId", invoiceId);

                connection.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0 ? "Invoice deleted successfully" : "Failed to delete invoice";
            }
        }
        #endregion
    }
}
