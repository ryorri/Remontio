using Application.Interfaces.AdditionalInterfaces;
using Application.Interfaces.ServiceInterfaces;
using Application.Objects.DTOs.RoomDTO;
using Application.Validators;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class RoomService : IRoomService
    {
        private readonly RemontioDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly CalculatingAreaExtension _calc;
        public RoomService(RemontioDbContext dbContext, IMapper mapper, CalculatingAreaExtension calc)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _calc = calc;
        }
    
        public async Task<bool> AddFloorAsync(string roomId, List<IPoint> points, string floorName = "")
        {
            if (roomId == null)
                throw new ArgumentNullException(nameof(roomId));

            try
            {
                var calculatedArea = _calc.CalculatePolygonArea(points);

                var floorDTO = new FloorDTO
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = floorName,
                    CalculatedArea = calculatedArea,
                    RoomId = Guid.Parse(roomId)
                };

                var floor = _mapper.Map<Floor>(floorDTO);

                await _dbContext.Floors.AddAsync(floor);

                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async  Task<bool> AddWallAsync(string roomId, List<IPoint> points, string wallName ="")
        {

            if (roomId == null)
                throw new ArgumentNullException(nameof(roomId));

            try
            {
                var calculatedArea = _calc.CalculatePolygonArea(points);

                var wallDTO = new WallDTO
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = wallName,
                    CalculatedArea = calculatedArea,
                    RoomId = Guid.Parse(roomId)
                };

                var wall = _mapper.Map<Wall>(wallDTO);

                await _dbContext.Walls.AddAsync(wall);

                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<bool> ChangeRoomStatusAsync(string roomId, string status)
        {
            try
            {
                var guid = GuidValidator.ValidateGuid(roomId);

                var room = await _dbContext.Rooms.FindAsync(guid);

                if (room != null)
                {
                    room.Status = Enum.Parse<StatusEnum>(status, true);
                }
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<bool> CreateRoomAsync(CreateRoomDTO roomDTO, string projectId)
        {

            if (roomDTO == null)
                throw new ArgumentNullException(nameof(roomDTO));

            try
            {
                var room = _mapper.Map<Room>(roomDTO);
                room.ProjectId = Guid.Parse(projectId);
                await _dbContext.Rooms.AddAsync(room);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<bool> DeleteRoomAsync(string roomId)
        {
            if (roomId == null)
                throw new ArgumentNullException(nameof(roomId));

            try
            {
                var guid = GuidValidator.ValidateGuid(roomId);

                var room = await _dbContext.Rooms.FindAsync(guid);

                if (room != null)
                    _dbContext.Rooms.Remove(room);

                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<List<RoomDataDTO>> GetAllRoomsAsync()
        {
            try
            {
                var roomList = await _dbContext.Rooms.Include(x => x.User)
                                                        .ToListAsync();
                return _mapper.Map<List<RoomDataDTO>>(roomList);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<List<RoomDataDTO>> GetAllRoomsByProjectIdAsync(string projectId)
        {
            try
            {

                var guid = GuidValidator.ValidateGuid(projectId);

                var roomList = await _dbContext.Rooms.Include(x => x.User)
                                                  .Where(x => x.ProjectId == guid)
                                                  .ToListAsync();
                return _mapper.Map<List<RoomDataDTO>>(roomList);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<List<RoomDataDTO>> GetAllRoomsByUserIdAsync(string userId)
        {
            try
            {
                var roomList = await _dbContext.Rooms.Include(x => x.User)
                                                  .Where(x => x.UserId == userId)
                                                  .ToListAsync();
                return _mapper.Map<List<RoomDataDTO>>(roomList);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<RoomDataDTO> GetRoomAsync(string roomId)
        {
            try
            {
                var room = await _dbContext.Rooms.Include(x => x.User)
                                                  .FirstOrDefaultAsync(x => x.Id.ToString() == roomId);

                return _mapper.Map<RoomDataDTO>(room);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<bool> RemoveAllWallFromRoomAsync(string roomId)
        {
            if (roomId == null)
                throw new ArgumentNullException(nameof(roomId));

            try
            {
                var guid = GuidValidator.ValidateGuid(roomId);

                var walls = await _dbContext.Walls
                                            .Where(w => w.RoomId == guid)
                                            .ToListAsync();

                if (walls.Any())
                {
                    _dbContext.Walls.RemoveRange(walls);
                    await _dbContext.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<bool> RemoveFloorFromRoomAsync(string roomId, string floorId)
        {
            if (roomId == null)
                throw new ArgumentNullException(nameof(roomId));
            if (floorId == null)
                throw new ArgumentNullException(nameof(floorId));

            try
            {
                var roomGuid = GuidValidator.ValidateGuid(roomId);
                var floorGuid = GuidValidator.ValidateGuid(floorId);

                var floor = await _dbContext.Floors
                                            .FirstOrDefaultAsync(f => f.Id == floorGuid && f.RoomId == roomGuid);

                if (floor != null)
                {
                    _dbContext.Floors.Remove(floor);
                    await _dbContext.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<bool> RemoveWallFromRoomAsync(string roomId, string wallId)
        {
            if (roomId == null)
                throw new ArgumentNullException(nameof(roomId));
            if (wallId == null)
                throw new ArgumentNullException(nameof(wallId));

            try
            {
                var roomGuid = GuidValidator.ValidateGuid(roomId);
                var wallGuid = GuidValidator.ValidateGuid(wallId);

                var wall = await _dbContext.Walls
                                           .FirstOrDefaultAsync(w => w.Id == wallGuid && w.RoomId == roomGuid);

                if (wall != null)
                {
                    _dbContext.Walls.Remove(wall);
                    await _dbContext.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<bool> UpdateRoomAsync(RoomDataDTO roomDTO)
        {
            try
            {
                var guid = GuidValidator.ValidateGuid(roomDTO.Id);

                var room = await _dbContext.Rooms.FindAsync(guid);

                if (room != null)
                {
                    room.Name = roomDTO.Name;
                    room.Description = roomDTO.Description;

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
