using Application.Interfaces.ServiceInterfaces;
using Application.Objects.DTOs.UserDTO;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        public string GenerateToken(UserDataDTO user)
        {
            

            var claims = new List<Claim>
            {

                new Claim(ClaimTypes.NameIdentifier, user.Id ?? throw new ArgumentNullException(nameof(user.Id))),
                new Claim(ClaimTypes.Name, user.UserName ?? throw new ArgumentNullException(nameof(user.UserName))),
                new Claim(ClaimTypes.Role, user.Role ?? throw new ArgumentNullException(nameof(user.Role)))
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
