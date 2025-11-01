using Application.Interfaces.SeederInterfaces;
using Application.Objects.DTOs.UserDTO;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Seeders
{
    public class UserSeeder : IUserSeeder
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly UserExtension _userExtension;
        public UserSeeder(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper, UserExtension userExtension)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _userExtension = userExtension;
        }

        public async Task SeedUsersAsync()
        {
            if (await _userManager.FindByEmailAsync("admin@admin") == null)
            {
                var admin = new CreateUserDTO
                {
                    Email = "admin@admin",
                    UserName = "admin",
                    Name = "Admin",
                    Surname = "Admin",
                    Password = "Admin1234!",
                    Role = "Admin",
                };

                var usr = _mapper.Map<User>(admin);
                var result = await _userManager.CreateAsync(usr, admin.Password);
                
                await _userExtension.AssignRefreshToken(usr);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(usr, admin.Role);
                }
                
            }
            
        }
        
    }
    
}
