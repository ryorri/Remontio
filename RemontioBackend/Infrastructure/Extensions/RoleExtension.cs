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
    public class RoleExtension
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        public RoleExtension(UserManager<User> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }
        public async Task<UserDataDTO> AssignRoles(User user)
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
    }
}
