using Application.Objects.DTOs.AlertsDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.ServiceInterfaces
{
    public interface IAlertService
    {
        Task<bool> CreateAlertAsync(CreateAlertDTO alert);
        Task<bool> DeleteAlertAsync(string alertId);
        Task<bool> UpdateAlertAsync(AlertDataDTO alert);
        Task <bool> ChangeStatusAsync (string alertId, string status);
        Task <bool> ChangePriorityAsync (string alertId, string priority);
        Task<AlertDataDTO?> GetAlertByIdAsync(string alertId);
        Task<List<AlertDataDTO>> GetAlertsByUserIdAsync(string userId);
        Task<List<AlertDataDTO>> GetAllAlertsAsync();

    }
}
