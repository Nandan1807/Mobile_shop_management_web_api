using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using mobile_shop_web_api.Models;

namespace mobile_shop_web_api.Data
{
    public class UserRepository
    {
        private readonly IConfiguration _configuration;

        public UserRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region GetAllUsers

        public List<UserModel> GetAllUsers()
        {
            List<UserModel> userModels = new List<UserModel>();
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Users_Select_All", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    userModels.Add(
                        new UserModel
                        {
                            UserId = Convert.ToInt32(reader["user_id"]),
                            UserName = reader["user_name"].ToString(),
                            UserEmail = reader["user_email"].ToString(),
                            Password = reader["password"].ToString(),
                            Role = reader["role"].ToString(),
                            Status = reader["status"].ToString(),
                            CreatedDate = Convert.ToDateTime(reader["created_date"]),
                            ModifiedDate = reader["modified_date"] as DateTime?
                        }
                    );
                }
            }

            return userModels;
        }

        #endregion

        #region GetUserById

        public UserModel GetUserById(int userId)
        {
            UserModel user = null;
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Users_Select_By_Id", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@UserId", userId);

                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    user = new UserModel
                    {
                        UserId = Convert.ToInt32(reader["user_id"]),
                        UserName = reader["user_name"].ToString(),
                        UserEmail = reader["user_email"].ToString(),
                        Role = reader["role"].ToString(),
                        Password = reader["password"].ToString(),
                        Status = reader["status"].ToString(),
                        CreatedDate = Convert.ToDateTime(reader["created_date"]),
                        ModifiedDate = reader["modified_date"] as DateTime?
                    };
                }
            }

            return user;
        }

        #endregion

        #region AddUser

        public string AddUser(UserModel user)
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Users_Insert", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@UserName", user.UserName);
                cmd.Parameters.AddWithValue("@UserEmail", user.UserEmail);
                cmd.Parameters.AddWithValue("@Password", user.Password);
                cmd.Parameters.AddWithValue("@Role", user.Role);
                cmd.Parameters.AddWithValue("@Status", user.Status);

                connection.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0 ? "User added successfully" : "Failed to add user";
            }
        }

        #endregion

        #region UpdateUser

        public string UpdateUser(UserModel user)
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Users_Update", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@UserId", user.UserId);
                cmd.Parameters.AddWithValue("@UserName", user.UserName);
                cmd.Parameters.AddWithValue("@UserEmail", user.UserEmail);
                cmd.Parameters.AddWithValue("@Password", user.Password);
                cmd.Parameters.AddWithValue("@Role", user.Role);
                cmd.Parameters.AddWithValue("@Status", user.Status);

                connection.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0 ? "User updated successfully" : "Failed to update user";
            }
        }

        #endregion

        #region DeleteUser

        public string DeleteUser(int userId)
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Users_Delete", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@UserId", userId);

                connection.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0 ? "User deleted successfully" : "Failed to delete user";
            }
        }

        #endregion

        #region SignInUser

        public Dictionary<string, object> SignInUser(UserAuthModel userAuthModel)
        {
            var dictionary = new Dictionary<string, object>();
            string connectionString = _configuration.GetConnectionString("ConnectionString");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand("PR_Users_Sign_In", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserEmail", userAuthModel.UserEmail);
                        cmd.Parameters.AddWithValue("@Password", userAuthModel.Password);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (reader.HasRows && reader["Message"].ToString() != "Invalid Email or Password")
                                {
                                    var user = new UserModel
                                    {
                                        UserId = Convert.ToInt32(reader["user_id"]),
                                        UserName = reader["user_name"].ToString(),
                                        UserEmail = reader["user_email"].ToString(),
                                        Role = reader["role"].ToString(),
                                        Password = reader["password"].ToString(),
                                        Status = reader["status"].ToString(),
                                        CreatedDate = Convert.ToDateTime(reader["created_date"]),
                                        ModifiedDate = reader["modified_date"] as DateTime?
                                    };
                                    var token = GenerateJwtToken(user);

                                    dictionary["UserDetails"] = user;
                                    dictionary["AuthToken"] = token;
                                }

                                // Make sure "Message" is always set
                                dictionary["Message"] = reader["Message"].ToString();
                            }
                            else
                            {
                                // If no rows found, set the "Message" key to indicate invalid credentials
                                dictionary["Message"] = "Invalid Email or Password";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                dictionary["Error"] = "An error occurred: " + ex.Message;
            }

            return dictionary;
        }

        #endregion

        #region SignOutUser

        public Dictionary<string, object> SignOutUser(UserAuthModel userAuthModel)
        {
            var dictionary = new Dictionary<string, object>();
            string connectionString = _configuration.GetConnectionString("ConnectionString");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand("PR_Users_Sign_Out", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserEmail", userAuthModel.UserEmail);
                        cmd.Parameters.AddWithValue("@Password", userAuthModel.Password);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (reader.HasRows && reader["Message"].ToString() != "Invalid Email or Password")
                                {
                                    dictionary["Message"] = reader["Message"].ToString();
                                }
                            }
                            else
                            {
                                dictionary["Message"] = "Invalid Email or Password";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                dictionary["Error"] = "An error occurred: " + ex.Message;
            }

            return dictionary;
        }

        #endregion

        #region GenerateJwtToken

        private string GenerateJwtToken(UserModel user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.UserEmail),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("UserId", user.UserId.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7), // Shorter expiry
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        #endregion
    }
}