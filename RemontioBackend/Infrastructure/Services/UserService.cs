using Application.Interfaces.ServiceInterfaces;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Application.Objects.DTOs.UserDTO;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleExtension _roleExtension;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(UserManager<User> userManager, IMapper mapper, SignInManager<User> signInManager, RoleExtension roleExtension, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _mapper = mapper;
            _signInManager = signInManager;
            _roleExtension = roleExtension;
            _roleManager = roleManager;
        }

        public async Task<bool> ChangePasswordAsync(string id, string oldPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                throw new Exception("User not found");
            else
            {
                var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);

                if (result.Succeeded)
                {
                    return true;
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        throw new Exception($"{error.Code} : {error.Description}");
                    }
                    return false;
                }
            }
                
        }

        public async Task<bool> ChangeRoleAsync(string id, string newRole)
        {
            var user = await _userManager.FindByIdAsync(id);

            if(!await _roleManager.RoleExistsAsync(newRole))
            {
                throw new Exception("Role not found");
            }
            if (user == null)
                throw new Exception("User not found");
            else
            {
                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Count == 0)
                {

                    var result = await _userManager.AddToRoleAsync(user, newRole);
                    if (result.Succeeded)
                    {
                        return true;
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            throw new Exception($"{error.Code} : {error.Description}");
                        }
                        return false;
                    }

                }
                else 
                {
                    var result = await _userManager.RemoveFromRolesAsync(user, roles);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, newRole);
                        return true;
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            throw new Exception($"{error.Code} : {error.Description}");
                        }
                        return false;
                    }
                }
               
            }
        }

        public async Task<bool> CreateUserAsync(CreateUserDTO user)
        {
            var userDto = _mapper.Map<User>(user);

            if(await _roleManager.RoleExistsAsync(user.Role) == false)
            {
                throw new Exception($"Role not found");
            }

            var result = await _userManager.CreateAsync(userDto, user.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(userDto, user.Role);
                return true;
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    throw new Exception($"{error.Code} : {error.Description}");
                }
                return false;
            }
                
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                throw new Exception("User not found");
            else
            {
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        throw new Exception($"{error.Code} : {error.Description}");
                    }
                }
                return true;
            }
        }

        public async Task<List<UserDataDTO>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();

            var userDto = new List<UserDataDTO>();

            foreach (var user in users)
            {
                userDto.Add(await _roleExtension.AssignRoles(user));
            }
            return userDto;
        }

        public async Task<UserDataDTO?> GetUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var userDto = _mapper.Map<UserDataDTO>(user);

            if (user != null)
            {
                await _roleExtension.AssignRoles(user);
            }
            else
                throw new Exception("User not found");

            return userDto;
        }

        public async Task<UserDataDTO?> LogInAsync(string username, string password)
        {
            var login = await _signInManager.PasswordSignInAsync(username, password, false, false);

            if (!login.Succeeded)
                throw new Exception("Please, check username and password");
            else
            {
                var userExist = await _userManager.FindByNameAsync(username);
                var user = await _roleExtension.AssignRoles(userExist!);
                return user;
            }
        }

        public async Task<bool> UpdateUserAsync(string id, UserDataDTO user)
        {
            var oldUser = await _userManager.FindByIdAsync(id);

            if (oldUser == null)
                throw new Exception("User not found");

            _mapper.Map(user, oldUser);

            var result = await _userManager.UpdateAsync(oldUser);

            if (result.Succeeded)
            {
                return true;
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    throw new Exception($"{error.Code} : {error.Description}");
                }
            }

            return false;
        }
    }
}
