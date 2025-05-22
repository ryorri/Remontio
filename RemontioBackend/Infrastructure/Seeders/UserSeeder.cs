using Application.Interfaces.DatabaseInterfaces.SeederInterfaces;
using Application.Objects.DTO.UserDTO;
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
            var usrDto = new CreateUserDTO
            {
                Email = "admin@example.com",
                UserName = "admin",
                Name = "Admin",
                Surname = "Admin",
                Password = "Admin1!",
                Role = "Admin",
            };


              var usr = _mapper.Map<User>(usrDto);
              var result = await _userManager.CreateAsync(usr, usrDto.Password);

              if (result.Succeeded)
              {
                  if(await _roleManager.RoleExistsAsync(usrDto.Role))
                {
                    await _userManager.AddToRoleAsync(usr, usrDto.Role);
                }
              }
        }
    }
}