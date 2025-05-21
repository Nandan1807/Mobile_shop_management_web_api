using Microsoft.Data.SqlClient;
using mobile_shop_web_api.Models;
using System.Data;

namespace mobile_shop_web_api.Data
{
    public class DropdownRepository
    {
        private readonly IConfiguration _configuration;

        public DropdownRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region GetCategoriesForDropdown
        public List<DropdownItemModel> GetCategoriesForDropdown()
        {
            List<DropdownItemModel> categories = new List<DropdownItemModel>();
            string connectionString = _configuration.GetConnectionString("ConnectionString");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Categories_Dropdown", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    categories.Add(new DropdownItemModel
                    {
                        Id = Convert.ToInt32(reader["category_id"]),
                        Name = reader["category_name"].ToString()
                    });
                }
            }

            return categories;
        }
        #endregion

        #region GetBrandsForDropdown
        public List<DropdownItemModel> GetBrandsForDropdown()
        {
            List<DropdownItemModel> brands = new List<DropdownItemModel>();
            string connectionString = _configuration.GetConnectionString("ConnectionString");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Brands_Dropdown", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    brands.Add(new DropdownItemModel
                    {
                        Id = Convert.ToInt32(reader["brand_id"]),
                        Name = reader["brand_name"].ToString()
                    });
                }
            }

            return brands;
        }
        #endregion

        #region GetCustomersForDropdown
        public List<DropdownItemModel> GetCustomersForDropdown()
        {
            List<DropdownItemModel> customers = new List<DropdownItemModel>();
            string connectionString = _configuration.GetConnectionString("ConnectionString");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Customers_Dropdown", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    customers.Add(new DropdownItemModel
                    {
                        Id = Convert.ToInt32(reader["customer_id"]),
                        Name = reader["customer_name"].ToString()
                    });
                }
            }

            return customers;
        }
        #endregion

        #region GetUsersForDropdown
        public List<DropdownItemModel> GetUsersForDropdown()
        {
            List<DropdownItemModel> users = new List<DropdownItemModel>();
            string connectionString = _configuration.GetConnectionString("ConnectionString");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Users_Dropdown", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    users.Add(new DropdownItemModel
                    {
                        Id = Convert.ToInt32(reader["user_id"]),
                        Name = reader["user_name"].ToString()
                    });
                }
            }

            return users;
        }
        #endregion

        #region GetProductsForDropdown
        public List<DropdownItemModel> GetProductsForDropdown()
        {
            List<DropdownItemModel> products = new List<DropdownItemModel>();
            string connectionString = _configuration.GetConnectionString("ConnectionString");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Products_Dropdown", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    products.Add(new DropdownItemModel
                    {
                        Id = Convert.ToInt32(reader["product_id"]),
                        Name = reader["product_name"].ToString()
                    });
                }
            }

            return products;
        }
        #endregion
    }
}
