using Application.Objects.DTOs.UserDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.ServiceInterfaces
{
    public interface ITokenService
    {
        string GenerateToken(UserDataDTO user);
        string GenerateRefreshToken();

    }
}
