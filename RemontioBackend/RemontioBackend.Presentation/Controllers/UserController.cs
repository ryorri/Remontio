using Application.Interfaces.ServiceInterfaces;
using Application.Objects.DTOs.UserDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        public UserController(IUserService userService, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<CreateUserDTO>> Register(CreateUserDTO dto)
        {
            try
            {
                var result = await _userService.CreateUserAsync(dto);
                return Ok(new { message = "User created successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("log-in")]
        public async Task<ActionResult<UserDataDTO>> LogIn(string username, string password)
        {

            try
            {
                var result = await _userService.LogInAsync(username, password);
                if (result != null)
                {
                    var token = _tokenService.GenerateToken(result);
                    var refreshToken = _tokenService.GenerateRefreshToken();
                    return Ok(new { result, token, refreshToken });
                }
                else
                {
                    return BadRequest(new { message = "Invalid username or password" });
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

        [Authorize]
        [HttpGet("get-user-list")]
        public async Task<ActionResult<List<UserDataDTO>>> GetUserList()
        {

            try
            {
                var result = await _userService.GetAllUsersAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

        [Authorize]
        [HttpGet("get-user")]
        public async Task<ActionResult<UserDataDTO>> GetUser(string id)
        {

            try
            {
                var result = await _userService.GetUserAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

        [Authorize]
        [HttpDelete("delete-user")]
        public async Task<ActionResult<UserDataDTO>> RemoveUser(string id)
        {

            try
            {
                var result = await _userService.DeleteUserAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

        [Authorize]
        [HttpPut("update-user")]
        public async Task<ActionResult<UserDataDTO>> UpdateUser(string id, UserDataDTO user)
        {

            try
            {
                var result = await _userService.UpdateUserAsync(id, user);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

        [Authorize]
        [HttpPut("change-user-password")]
        public async Task<ActionResult<UserDataDTO>> ChangeUserPassword(string id, string oldPassword, string newPassword)
        {

            try
            {
                var result = await _userService.ChangePasswordAsync(id, oldPassword, newPassword);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

        [Authorize]
        [HttpPut("change-user-role")]
        public async Task<ActionResult<UserDataDTO>> ChangeUserRole(string id, string newRole)
        {

            try
            {
                var result = await _userService.ChangeRoleAsync(id, newRole);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

    }
}
