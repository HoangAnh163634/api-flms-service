using api_flms_service.Model;
using api_flms_service.ServiceInterface;
using Microsoft.AspNetCore.Mvc;

namespace api_flms_service.Controllers
{
    [ApiController]
    [Route("api/v0/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>List of users</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving users.", details = ex.Message });
            }
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User details</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Mobile = user.Mobile,
                    Address = user.Address,
                    GoogleId = user.GoogleId
                };

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the user.", details = ex.Message });
            }
        }

        /// <summary>
        /// Add a new user
        /// </summary>
        /// <param name="userDto">User DTO</param>
        /// <returns>Created user</returns>
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = new User
                {
                    Name = userDto.Name,
                    Email = userDto.Email,
                    Password = userDto.Password,
                    Mobile = userDto.Mobile,
                    Address = userDto.Address,
                    GoogleId = userDto.GoogleId
                };

                var createdUser = await _userService.AddUserAsync(user);

                var createdUserDto = new UserDto
                {
                    Id = createdUser.Id,
                    Name = createdUser.Name,
                    Email = createdUser.Email,
                    Mobile = createdUser.Mobile,
                    Address = createdUser.Address,
                    GoogleId = createdUser.GoogleId
                };

                return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUserDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the user.", details = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing user
        /// </summary>
        /// <param name="userDto">Updated user DTO</param>
        /// <returns>Updated user</returns>
        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingUser = await _userService.GetUserByIdAsync(userDto.Id);
                if (existingUser == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                existingUser.Name = userDto.Name;
                existingUser.Email = userDto.Email;
                existingUser.Password = userDto.Password;
                existingUser.Mobile = userDto.Mobile;
                existingUser.Address = userDto.Address;
                existingUser.GoogleId = userDto.GoogleId;

                var updatedUser = await _userService.UpdateUserAsync(existingUser);

                var updatedUserDto = new UserDto
                {
                    Id = updatedUser.Id,
                    Name = updatedUser.Name,
                    Email = updatedUser.Email,
                    Mobile = updatedUser.Mobile,
                    Address = updatedUser.Address,
                    GoogleId = updatedUser.GoogleId
                };

                return Ok(updatedUserDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the user.", details = ex.Message });
            }
        }

        /// <summary>
        /// Delete a user by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>No content on success</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                await _userService.DeleteUserAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the user.", details = ex.Message });
            }
        }
    }
}
