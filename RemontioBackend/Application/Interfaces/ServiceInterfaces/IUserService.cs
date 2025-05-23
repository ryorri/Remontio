using Application.Objects.DTOs.UserDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.ServiceInterfaces
{
    public interface IUserService
    {
        Task<bool> CreateUserAsync(CreateUserDTO user);
        Task<bool> UpdateUserAsync(string id, UserDataDTO user);
        Task<bool> DeleteUserAsync(string id);
        Task<UserDataDTO?> GetUserAsync(string id);
        Task<UserDataDTO?> LogInAsync(string username, string password);
        Task<List<UserDataDTO>> GetAllUsersAsync();
        Task<bool> ChangeRoleAsync(string id, string newRole);
        Task<bool> ChangePasswordAsync(string id, string oldPassword, string newPassword);
    }
}
