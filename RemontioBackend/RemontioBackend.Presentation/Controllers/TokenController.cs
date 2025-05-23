using Application.Interfaces.ServiceInterfaces;
using Application.Objects.DTOs.UserDTO;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public TokenController(ITokenService tokenService)
        {
            _tokenService = tokenService;
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

    }
}
