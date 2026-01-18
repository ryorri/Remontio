using Application.Objects.DTOs.CalculationsDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.ServiceInterfaces
{
    public interface ICalculatorService
    {
        Task<bool> CreateCalculationAsync(CreateCalculationDTO calculationDTO, string roomId);
        Task<bool> UpdateCalculationAsync(CalculationDataDTO calculationDTO);
        Task<bool> DeleteCalculationAsync(string calculationId);
        Task<List<CalculationDataDTO>> GetAllCalculationsAsync();
        Task<List<CalculationDataDTO>> GetAllCalculationsByUserIdAsync(string userId);
        Task<List<CalculationDataDTO>> GetAllCalculationsByProjectIdAsync(string projectId);
        Task<List<CalculationDataDTO>> GetAllCalculationsByRoomIdAsync(string roomId);
        Task<CalculationDataDTO> GetCalculationAsync(string calculationId);
        Task<bool> ChangeCalculationTypeAsync(string calculationId, string type);
    }
}
