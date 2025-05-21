using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mobile_shop_web_api.Data;
using mobile_shop_web_api.Models;

namespace mobile_shop_web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserRepository _userRepository;

        public UsersController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // GET: api/Users
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            try
            {
                var users = _userRepository.GetAllUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Users/{id}
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {
            try
            {
                var user = _userRepository.GetUserById(id);
                if (user == null)
                {
                    return NotFound($"User with ID {id} not found.");
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/Users
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AddUser([FromBody] UserModel user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid user data.");
            }

            try
            {
                var result = _userRepository.AddUser(user);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/Users/{id}
        [Authorize]
        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] UserModel user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid user data.");
            }

            if (id != user.UserId)
            {
                return BadRequest("User ID mismatch.");
            }

            try
            {
                var result = _userRepository.UpdateUser(user);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/Users/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                var result = _userRepository.DeleteUser(id);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        // POST: api/Users/signin
        [HttpPost]
        [Route("signin")]
        public IActionResult SignIn([FromBody] UserAuthModel user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid user data.");
            }

            try
            {
                var result = _userRepository.SignInUser(user);

                // Check if the 'Message' key is present in the result dictionary
                if (result.ContainsKey("Message") && result["Message"].ToString() == "Invalid Email or Password")
                {
                    return BadRequest(result);
                }

                // Return the response with the result, including the token and user details
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        
        // POST: api/Users/signout
        [HttpPost]
        [Route("signout")]
        public IActionResult SignOut([FromBody] UserAuthModel user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid user data.");
            }

            try
            {
                var result = _userRepository.SignOutUser(user);
                if (result["Message"].ToString() == "Invalid Email or Password")
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
