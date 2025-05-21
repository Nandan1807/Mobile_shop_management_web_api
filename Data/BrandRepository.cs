using System.Data;
using Microsoft.Data.SqlClient;
using mobile_shop_web_api.Models;

namespace mobile_shop_web_api.Data
{
    public class BrandRepository
    {
        private readonly IConfiguration _configuration;

        public BrandRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region GetAllBrands
        public List<BrandModel> GetAllBrands()
        {
            List<BrandModel> brandModels = new List<BrandModel>();
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Brands_Select_All", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    brandModels.Add(
                        new BrandModel
                        {
                            BrandId = Convert.ToInt32(reader["brand_id"]),
                            BrandName = reader["brand_name"].ToString(),
                            CreatedDate = Convert.ToDateTime(reader["created_date"]),
                            ModifiedDate = reader["modified_date"] as DateTime?
                        }
                    );
                }
            }

            return brandModels;
        }
        #endregion

        #region GetBrandById
        public BrandModel GetBrandById(int brandId)
        {
            BrandModel brand = null;
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Brands_Select_By_Id", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@BrandId", brandId);

                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    brand = new BrandModel
                    {
                        BrandId = Convert.ToInt32(reader["brand_id"]),
                        BrandName = reader["brand_name"].ToString(),
                        CreatedDate = Convert.ToDateTime(reader["created_date"]),
                        ModifiedDate = reader["modified_date"] as DateTime?
                    };
                }
            }

            return brand;
        }
        #endregion

        #region AddBrand
        public string AddBrand(BrandModel brand)
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Brands_Insert", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@BrandName", brand.BrandName);

                connection.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0 ? "Brand added successfully" : "Failed to add brand";
            }
        }
        #endregion

        #region UpdateBrand
        public string UpdateBrand(BrandModel brand)
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Brands_Update", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@BrandId", brand.BrandId);
                cmd.Parameters.AddWithValue("@BrandName", brand.BrandName);

                connection.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0 ? "Brand updated successfully" : "Failed to update brand";
            }
        }
        #endregion

        #region DeleteBrand
        public string DeleteBrand(int brandId)
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Brands_Delete", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@BrandId", brandId);

                connection.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0 ? "Brand deleted successfully" : "Failed to delete brand";
            }
        }
        #endregion
    }
}
