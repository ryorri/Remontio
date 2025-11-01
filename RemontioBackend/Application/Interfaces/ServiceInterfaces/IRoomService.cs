using Application.Interfaces.AdditionalInterfaces;
using Application.Objects.DTOs.RoomDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.ServiceInterfaces
{
    public interface IRoomService
    {
        Task<bool> CreateRoomAsync(CreateRoomDTO roomDTO, string projectId);
        Task<bool> UpdateRoomAsync(RoomDataDTO roomDTO);
        Task<bool> DeleteRoomAsync(string roomId);
        Task<List<RoomDataDTO>> GetAllRoomsAsync();
        Task<List<RoomDataDTO>> GetAllRoomsByUserIdAsync(string userId);
        Task<List<RoomDataDTO>> GetAllRoomsByProjectIdAsync(string projectId);
        Task<RoomDataDTO> GetRoomAsync(string roomId);
        Task<bool> ChangeRoomStatusAsync(string roomId, string status);
        Task<bool> AddWallAsync(string roomId, List<IPoint> points, string wallName);
        Task<bool> RemoveWallFromRoomAsync(string roomId, string wallId);
        Task<bool> RemoveAllWallFromRoomAsync(string roomId);
        Task<bool> AddFloorAsync(string roomId, List<IPoint> points, string floorName);
        Task<bool> RemoveFloorFromRoomAsync(string roomId, string floorId);
    }
}
