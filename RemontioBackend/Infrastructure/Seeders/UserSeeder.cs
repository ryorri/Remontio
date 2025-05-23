using Application.Interfaces.SeederInterfaces;
using Application.Objects.DTOs.UserDTO;
using AutoMapper;
using Domain.Entities;
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
        public UserSeeder(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
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
                    Password = "Admin1!",
                    Role = "Admin",
                };

                var usr = _mapper.Map<User>(admin);
                var result = await _userManager.CreateAsync(usr, admin.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(usr, admin.Role);
                }
                
            }
            
        }
        
    }
    
}
