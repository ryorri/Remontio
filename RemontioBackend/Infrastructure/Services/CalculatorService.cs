using Application.Interfaces.ServiceInterfaces;
using Application.Objects.DTOs.CalculationsDTO;
using Application.Validators;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class CalculatorService : ICalculatorService
    {
        private readonly RemontioDbContext _dbContext;
        private readonly IMapper _mapper;

        public CalculatorService(RemontioDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<bool> CreateCalculationAsync(CreateCalculationDTO calculationDTO, string roomId)
        {
            if (calculationDTO == null)
                throw new ArgumentNullException(nameof(calculationDTO));

            try
            {
                var roomGuid = GuidValidator.ValidateGuid(roomId);
                var room = await _dbContext.Rooms.FindAsync(roomGuid);

                if (room == null)
                    throw new ArgumentException($"Room with ID {roomId} not found");

                var calculation = new Calculations
                {
                    Id = Guid.NewGuid(),
                    Name = calculationDTO.Name,
                    Value = calculationDTO.Value,
                    Type = calculationDTO.Type,
                    RoomId = roomGuid,
                    ProjectId = room.ProjectId,
                    UserId = room.UserId
                };

                await _dbContext.Calculations.AddAsync(calculation);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: {ex.Message}");
            }
        }

        public async Task<bool> UpdateCalculationAsync(CalculationDataDTO calculationDTO)
        {
            try
            {
                var guid = GuidValidator.ValidateGuid(calculationDTO.Id);

                var calculation = await _dbContext.Calculations.FindAsync(guid);

                if (calculation != null)
                {
                    calculation.Name = calculationDTO.Name;
                    calculation.Value = calculationDTO.Value;
                    calculation.Type = calculationDTO.Type;
                }
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: {ex.Message}");
            }
        }

        public async Task<bool> DeleteCalculationAsync(string calculationId)
        {
            if (calculationId == null)
                throw new ArgumentNullException(nameof(calculationId));

            try
            {
                var guid = GuidValidator.ValidateGuid(calculationId);

                var calculation = await _dbContext.Calculations.FindAsync(guid);
                if (calculation != null)
                {
                    _dbContext.Calculations.Remove(calculation);
                }

                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: {ex.Message}");
            }
        }

        public async Task<List<CalculationDataDTO>> GetAllCalculationsAsync()
        {
            try
            {
                var calculationList = await _dbContext.Calculations
                                                        .Include(x => x.User)
                                                        .Include(x => x.Room)
                                                        .Include(x => x.Project)
                                                        .ToListAsync();
                return _mapper.Map<List<CalculationDataDTO>>(calculationList);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: {ex.Message}");
            }
        }

        public async Task<List<CalculationDataDTO>> GetAllCalculationsByUserIdAsync(string userId)
        {
            try
            {
                var calculationList = await _dbContext.Calculations
                                                  .Include(x => x.User)
                                                  .Include(x => x.Room)
                                                  .Include(x => x.Project)
                                                  .Where(x => x.UserId == userId)
                                                  .ToListAsync();
                return _mapper.Map<List<CalculationDataDTO>>(calculationList);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: {ex.Message}");
            }
        }

        public async Task<List<CalculationDataDTO>> GetAllCalculationsByProjectIdAsync(string projectId)
        {
            try
            {
                var guid = GuidValidator.ValidateGuid(projectId);

                var calculationList = await _dbContext.Calculations
                                                  .Include(x => x.User)
                                                  .Include(x => x.Room)
                                                  .Include(x => x.Project)
                                                  .Where(x => x.ProjectId == guid)
                                                  .ToListAsync();
                return _mapper.Map<List<CalculationDataDTO>>(calculationList);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: {ex.Message}");
            }
        }

        public async Task<List<CalculationDataDTO>> GetAllCalculationsByRoomIdAsync(string roomId)
        {
            try
            {
                var guid = GuidValidator.ValidateGuid(roomId);

                var calculationList = await _dbContext.Calculations
                                                  .Include(x => x.User)
                                                  .Include(x => x.Room)
                                                  .Include(x => x.Project)
                                                  .Where(x => x.RoomId == guid)
                                                  .ToListAsync();
                return _mapper.Map<List<CalculationDataDTO>>(calculationList);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: {ex.Message}");
            }
        }

        public async Task<CalculationDataDTO> GetCalculationAsync(string calculationId)
        {
            try
            {
                var guid = GuidValidator.ValidateGuid(calculationId);

                var calculation = await _dbContext.Calculations
                                                  .Include(x => x.User)
                                                  .Include(x => x.Room)
                                                  .Include(x => x.Project)
                                                  .FirstOrDefaultAsync(x => x.Id == guid);

                return _mapper.Map<CalculationDataDTO>(calculation);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: {ex.Message}");
            }
        }

        public async Task<bool> ChangeCalculationTypeAsync(string calculationId, string type)
        {
            try
            {
                var guid = GuidValidator.ValidateGuid(calculationId);

                var calculation = await _dbContext.Calculations.FindAsync(guid);

                if (calculation != null)
                {
                    if (Enum.TryParse<CalculationsTypeEnum>(type, true, out var parsedType))
                    {
                        calculation.Type = parsedType;
                    }
                    else
                    {
                        throw new ArgumentException($"Invalid calculation type: {type}");
                    }
                }
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: {ex.Message}");
            }
        }
    }
}
