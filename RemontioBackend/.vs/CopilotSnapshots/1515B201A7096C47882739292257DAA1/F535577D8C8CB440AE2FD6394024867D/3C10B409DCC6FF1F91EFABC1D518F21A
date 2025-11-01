using Application.Interfaces.ServiceInterfaces;
using Application.Objects.DTOs.AlertsDTO;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class AlertService : IAlertService
    {
        private readonly RemontioDbContext _dbContext;
        private readonly IMapper _mapper;

        public AlertService(RemontioDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<bool> ChangePriorityAsync(string alertId, string priority)
        {
            try
            {
                if (!Guid.TryParse(alertId, out Guid guid))
                    throw new ArgumentException("Invalid alert ID format");

                var alert = await _dbContext.Alerts.FindAsync(guid);

                if (alert != null)
                {
                    alert.Priority = Enum.Parse<PriorityEnum>(priority, true);
                }

                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<bool> ChangeStatusAsync(string alertId, string status)
        {
            try
            {
                if (!Guid.TryParse(alertId, out Guid guid))
                    throw new ArgumentException("Invalid alert ID format");

                var alert = await _dbContext.Alerts.FindAsync(guid);

                if (alert != null)
                {
                    alert.Status = Enum.Parse<StatusEnum>(status, true);
                }

                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<bool> CreateAlertAsync(CreateAlertDTO alert)
        {
            if (alert == null)
                throw new ArgumentNullException(nameof(alert));

            try
            {
                var entity = _mapper.Map<Alerts>(alert);
                await _dbContext.Alerts.AddAsync(entity);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<bool> DeleteAlertAsync(string alertId)
        {
            if (alertId == null)
                throw new ArgumentNullException(nameof(alertId));

            try
            {
                if (!Guid.TryParse(alertId, out Guid guid))
                    throw new ArgumentException("Invalid alert ID format");

                var alert = await _dbContext.Alerts.FindAsync(guid);

                if (alert != null)
                    _dbContext.Alerts.Remove(alert);

                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<AlertDataDTO?> GetAlertByIdAsync(string alertId)
        {
            try
            {
                var alert = await _dbContext.Alerts.Include(x => x.User)
                                                    .FirstOrDefaultAsync(x => x.Id.ToString() == alertId);

                return _mapper.Map<AlertDataDTO>(alert);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<List<AlertDataDTO>> GetAlertsByUserIdAsync(string userId)
        {
            try
            {
                var alerts = await _dbContext.Alerts.Include(x => x.User)
                                                    .Where(x => x.UserId == userId)
                                                    .ToListAsync();

                return _mapper.Map<List<AlertDataDTO>>(alerts);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<List<AlertDataDTO>> GetAllAlertsAsync()
        {
            try
            {
                var alerts = await _dbContext.Alerts.Include(x => x.User)
                                                     .ToListAsync();

                return _mapper.Map<List<AlertDataDTO>>(alerts);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<bool> UpdateAlertAsync(AlertDataDTO alert)
        {
            try
            {
                if (!Guid.TryParse(alert.Id, out Guid guid))
                    throw new ArgumentException("Invalid alert ID format");

                var entity = await _dbContext.Alerts.FindAsync(guid);

                if (entity != null)
                {
                    entity.Message = alert.Message;

                    if (!string.IsNullOrEmpty(alert.Status))
                        entity.Status = Enum.Parse<StatusEnum>(alert.Status, true);

                    if (!string.IsNullOrEmpty(alert.Priority))
                        entity.Priority = Enum.Parse<PriorityEnum>(alert.Priority, true);
                }

                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }
    }
}
