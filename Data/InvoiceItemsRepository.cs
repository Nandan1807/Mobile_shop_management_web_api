using System.Data;
using Microsoft.Data.SqlClient;
using mobile_shop_web_api.Models;

namespace mobile_shop_web_api.Data
{
    public class InvoiceItemRepository
    {
        private readonly IConfiguration _configuration;

        public InvoiceItemRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region GetAllInvoiceItems
        public List<InvoiceItemModel> GetAllInvoiceItems()
        {
            List<InvoiceItemModel> invoiceItemModels = new List<InvoiceItemModel>();
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Invoice_Items_Select_All", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    invoiceItemModels.Add(
                        new InvoiceItemModel
                        {
                            InvoiceItemId = Convert.ToInt32(reader["invoice_item_id"]),
                            InvoiceId = Convert.ToInt32(reader["invoice_id"]),
                            ProductId = Convert.ToInt32(reader["product_id"]),
                            Quantity = Convert.ToInt32(reader["quantity"]),
                            UnitPrice = Convert.ToDecimal(reader["unit_price"]),
                            TotalPrice = Convert.ToDecimal(reader["total_price"]),
                            ProductName = reader["product_name"].ToString()
                        }
                    );
                }
            }

            return invoiceItemModels;
        }
        #endregion

        #region GetInvoiceItemsById
        public InvoiceItemModel? GetInvoiceItemsById(int id)
        {
            InvoiceItemModel? invoiceItem = null;
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Invoice_Items_Select_By_Id", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@InvoiceItemId", id);

                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {

                    invoiceItem = new InvoiceItemModel
                    {
                        InvoiceItemId = Convert.ToInt32(reader["invoice_item_id"]),
                        InvoiceId = Convert.ToInt32(reader["invoice_id"]),
                        ProductId = Convert.ToInt32(reader["product_id"]),
                        Quantity = Convert.ToInt32(reader["quantity"]),
                        UnitPrice = Convert.ToDecimal(reader["unit_price"]),
                        TotalPrice = Convert.ToDecimal(reader["total_price"]),
                        ProductName = reader["product_name"].ToString()
                    };
                }
            }

            return invoiceItem;
        }
        #endregion
        
        #region GetInvoiceItemsByInvoiceId
        public List<InvoiceItemModel> GetInvoiceItemsByInvoiceId(int invoiceId)
        {
            List<InvoiceItemModel> invoiceItemModels = new List<InvoiceItemModel>();
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Invoice_Items_Select_By_InvoiceId", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@InvoiceId", invoiceId);

                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    invoiceItemModels.Add(
                        new InvoiceItemModel
                        {
                            InvoiceItemId = Convert.ToInt32(reader["invoice_item_id"]),
                            InvoiceId = Convert.ToInt32(reader["invoice_id"]),
                            ProductId = Convert.ToInt32(reader["product_id"]),
                            Quantity = Convert.ToInt32(reader["quantity"]),
                            UnitPrice = Convert.ToDecimal(reader["unit_price"]),
                            TotalPrice = Convert.ToDecimal(reader["total_price"]),
                            ProductName = reader["product_name"].ToString()
                        }
                    );
                }
            }

            return invoiceItemModels;
        }
        #endregion

        #region AddInvoiceItem
        public string AddInvoiceItem(InvoiceItemModel invoiceItem)
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Invoice_Items_Insert", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@InvoiceId", invoiceItem.InvoiceId);
                cmd.Parameters.AddWithValue("@ProductId", invoiceItem.ProductId);
                cmd.Parameters.AddWithValue("@Quantity", invoiceItem.Quantity);

                connection.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0 ? "Invoice Item added successfully" : "Failed to add Invoice Item";
            }
        }
        #endregion

        #region UpdateInvoiceItem
        public string UpdateInvoiceItem(InvoiceItemModel invoiceItem)
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Invoice_Items_Update", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@InvoiceItemId", invoiceItem.InvoiceItemId);
                cmd.Parameters.AddWithValue("@InvoiceId", invoiceItem.InvoiceId);
                cmd.Parameters.AddWithValue("@ProductId", invoiceItem.ProductId);
                cmd.Parameters.AddWithValue("@Quantity", invoiceItem.Quantity);

                connection.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0 ? "Invoice Item updated successfully" : "Failed to update Invoice Item";
            }
        }
        #endregion

        #region DeleteInvoiceItem
        public string DeleteInvoiceItem(int invoiceItemId)
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Invoice_Items_Delete", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@InvoiceItemId", invoiceItemId);

                connection.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0 ? "Invoice Item deleted successfully" : "Failed to delete Invoice Item";
            }
        }
        #endregion
    }
}
