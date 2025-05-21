using System.Data;
using Microsoft.Data.SqlClient;
using mobile_shop_web_api.Models;

namespace mobile_shop_web_api.Data
{
    public class ProductRepository
    {
        private readonly IConfiguration _configuration;

        public ProductRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region GetAllProducts

        public List<ProductModel> GetAllProducts(int? categoryId = null, int? brandId = null)
        {
            List<ProductModel> productModels = new List<ProductModel>();
            string connectionString = _configuration.GetConnectionString("ConnectionString");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("PR_Products_Select_All", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add parameters only if they have values
                    if (categoryId.HasValue)
                        cmd.Parameters.AddWithValue("@CategoryId", categoryId);
                    else
                        cmd.Parameters.AddWithValue("@CategoryId", DBNull.Value);

                    if (brandId.HasValue)
                        cmd.Parameters.AddWithValue("@BrandId", brandId);
                    else
                        cmd.Parameters.AddWithValue("@BrandId", DBNull.Value);

                    connection.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            productModels.Add(new ProductModel
                            {
                                ProductId = Convert.ToInt32(reader["product_id"]),
                                CategoryName = reader["category_name"].ToString(),
                                BrandName = reader["brand_name"].ToString(),
                                ProductName = reader["product_name"].ToString(),
                                ProductImage = reader["product_image"] != DBNull.Value
                                    ? reader["product_image"].ToString()
                                    : null,
                                ProductPrice = Convert.ToDecimal(reader["product_price"]),
                                StockQuantity = Convert.ToInt32(reader["stock_quantity"]),
                                ProductDescription = reader["product_description"] != DBNull.Value
                                    ? reader["product_description"].ToString()
                                    : null,
                                Status = reader["status"].ToString(),
                                CreatedDate = Convert.ToDateTime(reader["created_date"]),
                                ModifiedDate = reader["modified_date"] != DBNull.Value
                                    ? Convert.ToDateTime(reader["modified_date"])
                                    : (DateTime?)null
                            });
                        }
                    }
                }
            }

            return productModels;
        }

        #endregion

        #region GetProductById

        public ProductModel GetProductById(int productId)
        {
            ProductModel product = null;
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Products_Select_By_Id", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@ProductId", productId);

                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    product = new ProductModel
                    {
                        ProductId = Convert.ToInt32(reader["product_id"]),
                        CategoryName = reader["category_name"].ToString(),
                        CategoryId = Convert.ToInt32(reader["category_id"]),
                        ProductBrandId = Convert.ToInt32(reader["product_brand_id"]),
                        BrandName = reader["brand_name"].ToString(),
                        ProductName = reader["product_name"].ToString(),
                        ProductImage = reader["product_image"]?.ToString(),
                        ProductPrice = Convert.ToDecimal(reader["product_price"]),
                        StockQuantity = Convert.ToInt32(reader["stock_quantity"]),
                        ProductDescription = reader["product_description"]?.ToString(),
                        Status = reader["status"].ToString(),
                        CreatedDate = Convert.ToDateTime(reader["created_date"]),
                        ModifiedDate = reader["modified_date"] as DateTime?
                    };
                }
            }

            return product;
        }

        #endregion

        #region AddProduct

        public async Task<string> AddProduct(ProductModel product)
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");

            // Handle file upload
            if (product.ImageFile != null && product.ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(product.ImageFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await product.ImageFile.CopyToAsync(fileStream);
                }

                product.ProductImage = "/images/products/" + uniqueFileName; // Save path
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Products_Insert", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@CategoryId", product.CategoryId);
                cmd.Parameters.AddWithValue("@ProductName", product.ProductName);
                cmd.Parameters.AddWithValue("@ProductImage", product.ProductImage ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ProductBrandId", product.ProductBrandId);
                cmd.Parameters.AddWithValue("@ProductPrice", product.ProductPrice);
                cmd.Parameters.AddWithValue("@StockQuantity", product.StockQuantity);
                cmd.Parameters.AddWithValue("@ProductDescription", product.ProductDescription ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Status", product.Status);

                connection.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0 ? "Product added successfully" : "Failed to add product";
            }
        }

        #endregion

        #region UpdateProduct

        public async Task<string> UpdateProduct(ProductModel product)
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");

            // Handle file upload if a new file is provided
            if (product.ImageFile != null && product.ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(product.ImageFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await product.ImageFile.CopyToAsync(fileStream);
                }

                product.ProductImage = "/images/products/" + uniqueFileName; // Save new path
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Products_Update", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@ProductId", product.ProductId);
                cmd.Parameters.AddWithValue("@CategoryId", product.CategoryId);
                cmd.Parameters.AddWithValue("@ProductName", product.ProductName);
                cmd.Parameters.AddWithValue("@ProductImage", product.ProductImage ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ProductBrandId", product.ProductBrandId);
                cmd.Parameters.AddWithValue("@ProductPrice", product.ProductPrice);
                cmd.Parameters.AddWithValue("@StockQuantity", product.StockQuantity);
                cmd.Parameters.AddWithValue("@ProductDescription", product.ProductDescription ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Status", product.Status);

                connection.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0 ? "Product updated successfully" : "Failed to update product";
            }
        }

        #endregion

        #region DeleteProduct

        public string DeleteProduct(int productId)
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Products_Delete", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@ProductId", productId);

                connection.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0 ? "Product deleted successfully" : "Failed to delete product";
            }
        }

        #endregion
    }
}