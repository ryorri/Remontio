using Application.Interfaces.ServiceInterfaces;
using Application.Objects.DTOs.UserDTO;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;

        public TokenController(ITokenService tokenService, IUserService userService)
        {
            _tokenService = tokenService;
            _userService = userService;

        }

        [HttpGet("generate-token")]
        public IActionResult GenerateToken(UserDataDTO user)
        {
            var token = _tokenService.GenerateToken(user);
            return Ok(new { Token = token });
        }

        [HttpGet("generate-refresh-token")]
        public IActionResult GenerateRefreshToken()
        {
            var refreshToken = _tokenService.GenerateRefreshToken();
            return Ok(new { RefreshToken = refreshToken });
        }

        [HttpGet("get-refresh-token")]
        public async Task<ActionResult<string>> GetByRefreshToken(string id, string refreshToken)
        {

            try
            {
                var user = await _userService.GetUserAsync(id);


                if (user != null && await _userService.CheckRefreshTokenAsync(id, refreshToken))
                {
                    var newRefreshToken = _tokenService.GenerateRefreshToken();
                    await _userService.AssignNewRefreshTokenAsync(id, newRefreshToken);
                    var token = _tokenService.GenerateToken(user!);
                    return Ok(new { newRefreshToken, token });
                }
                else
                {
                    return BadRequest(new { message = "Invalid refresh token" });
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }
    }
}
