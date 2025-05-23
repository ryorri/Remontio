using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Objects.DTOs.UserDTO
{
    public class CreateUserDTO
    {
        public required string UserName { get; set; }
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
