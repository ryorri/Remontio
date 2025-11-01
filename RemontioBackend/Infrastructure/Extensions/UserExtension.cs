using Application.Interfaces.ServiceInterfaces;
using Application.Objects.DTOs.UserDTO;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Extensions
{
    public class UserExtension
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        public UserExtension(UserManager<User> userManager, IMapper mapper, ITokenService tokenService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _tokenService = tokenService;
        }
        public async Task<UserDataDTO> AssignRolesAsync(User user)
        {
            var userDto = _mapper.Map<UserDataDTO>(user);

            var userId = user.Id;
            var exists = await _userManager.FindByIdAsync(userId);
            if (exists != null)
            {
                var roles = await _userManager.GetRolesAsync(exists);
                var rolesList = roles.ToList();
                userDto.Role = roles[0];
            }

            return userDto;
        }

        public async Task<UserDataDTO> AssignRefreshToken(User user)
        {
            user.RefreshToken = _tokenService.GenerateRefreshToken();
            await _userManager.UpdateAsync(user);
            return _mapper.Map<UserDataDTO>(user);
        }
    }
}
