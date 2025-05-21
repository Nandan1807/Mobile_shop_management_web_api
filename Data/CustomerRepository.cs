using System.Data;
using Microsoft.Data.SqlClient;
using mobile_shop_web_api.Models;

namespace mobile_shop_web_api.Data
{
    public class CustomerRepository
    {
        private readonly IConfiguration _configuration;

        public CustomerRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region GetAllCustomers
        public List<CustomerModel> GetAllCustomers()
        {
            List<CustomerModel> customerModels = new List<CustomerModel>();
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Customers_Select_All", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    customerModels.Add(
                        new CustomerModel
                        {
                            CustomerId = Convert.ToInt32(reader["customer_id"]),
                            CustomerName = reader["customer_name"].ToString(),
                            CustomerEmail = reader["customer_email"].ToString(),
                            CustomerPhone = reader["customer_phone"].ToString(),
                            CustomerAddress = reader["customer_address"].ToString(),
                            CreatedDate = Convert.ToDateTime(reader["created_date"]),
                            ModifiedDate = reader["modified_date"] as DateTime?
                        }
                    );
                }
            }

            return customerModels;
        }
        #endregion

        #region GetCustomerById
        public CustomerModel GetCustomerById(int customerId)
        {
            CustomerModel customer = null;
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Customers_Select_By_Id", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@CustomerId", customerId);

                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    customer = new CustomerModel
                    {
                        CustomerId = Convert.ToInt32(reader["customer_id"]),
                        CustomerName = reader["customer_name"].ToString(),
                        CustomerEmail = reader["customer_email"].ToString(),
                        CustomerPhone = reader["customer_phone"].ToString(),
                        CustomerAddress = reader["customer_address"].ToString(),
                        CreatedDate = Convert.ToDateTime(reader["created_date"]),
                        ModifiedDate = reader["modified_date"] as DateTime?
                    };
                }
            }

            return customer;
        }
        #endregion

        #region AddCustomer
        public string AddCustomer(CustomerModel customer)
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Customers_Insert", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@CustomerName", customer.CustomerName);
                cmd.Parameters.AddWithValue("@CustomerEmail", customer.CustomerEmail);
                cmd.Parameters.AddWithValue("@CustomerPhone", customer.CustomerPhone);
                cmd.Parameters.AddWithValue("@CustomerAddress", customer.CustomerAddress);

                connection.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0 ? "Customer added successfully" : "Failed to add customer";
            }
        }
        #endregion

        #region UpdateCustomer
        public string UpdateCustomer(CustomerModel customer)
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Customers_Update", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@CustomerId", customer.CustomerId);
                cmd.Parameters.AddWithValue("@CustomerName", customer.CustomerName);
                cmd.Parameters.AddWithValue("@CustomerEmail", customer.CustomerEmail);
                cmd.Parameters.AddWithValue("@CustomerPhone", customer.CustomerPhone);
                cmd.Parameters.AddWithValue("@CustomerAddress", customer.CustomerAddress);

                connection.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0 ? "Customer updated successfully" : "Failed to update customer";
            }
        }
        #endregion

        #region DeleteCustomer
        public string DeleteCustomer(int customerId)
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Customers_Delete", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@CustomerId", customerId);

                connection.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0 ? "Customer deleted successfully" : "Failed to delete customer";
            }
        }
        #endregion
    }
}
