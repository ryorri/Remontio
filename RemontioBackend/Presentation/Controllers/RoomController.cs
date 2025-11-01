using Application.Interfaces.AdditionalInterfaces;
using Application.Interfaces.ServiceInterfaces;
using Application.Objects.DTOs.RoomDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [Authorize]
        [HttpPost("create-room")]
        public async Task<ActionResult<bool>> CreateRoom(CreateRoomDTO roomDTO, string projectId)
        {
            try
            {
                var result = await _roomService.CreateRoomAsync(roomDTO, projectId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("get-room-by-id")]
        public async Task<ActionResult<RoomDataDTO>> GetRoomById(string roomId)
        {
            try
            {
                var result = await _roomService.GetRoomAsync(roomId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("get-room-list")]
        public async Task<ActionResult<List<RoomDataDTO>>> GetRoomList()
        {
            try
            {
                var result = await _roomService.GetAllRoomsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("get-room-list-by-user-id")]
        public async Task<ActionResult<List<RoomDataDTO>>> GetRoomListByUserId(string userId)
        {
            try
            {
                var result = await _roomService.GetAllRoomsByUserIdAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("get-room-list-by-project-id")]
        public async Task<ActionResult<List<RoomDataDTO>>> GetRoomListByProjectId(string projectId)
        {
            try
            {
                var result = await _roomService.GetAllRoomsByProjectIdAsync(projectId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("delete-room")]
        public async Task<ActionResult<bool>> DeleteRoom(string roomId)
        {
            try
            {
                var result = await _roomService.DeleteRoomAsync(roomId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("edit-room")]
        public async Task<ActionResult<bool>> EditRoom(RoomDataDTO roomDTO)
        {
            try
            {
                var result = await _roomService.UpdateRoomAsync(roomDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("edit-room-status")]
        public async Task<ActionResult<bool>> EditRoomStatus(string roomId, string status)
        {
            try
            {
                var result = await _roomService.ChangeRoomStatusAsync(roomId, status);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("add-wall")]
        public async Task<ActionResult<bool>> AddWall(string roomId, [FromBody] List<IPoint> points, string wallName = "")
        {
            try
            {
                var result = await _roomService.AddWallAsync(roomId, points, wallName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("remove-wall")]
        public async Task<ActionResult<bool>> RemoveWall(string roomId, string wallId)
        {
            try
            {
                var result = await _roomService.RemoveWallFromRoomAsync(roomId, wallId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("remove-all-walls")]
        public async Task<ActionResult<bool>> RemoveAllWalls(string roomId)
        {
            try
            {
                var result = await _roomService.RemoveAllWallFromRoomAsync(roomId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("add-floor")]
        public async Task<ActionResult<bool>> AddFloor(string roomId, [FromBody] List<IPoint> points, string floorName = "")
        {
            try
            {
                var result = await _roomService.AddFloorAsync(roomId, points, floorName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("remove-floor")]
        public async Task<ActionResult<bool>> RemoveFloor(string roomId, string floorId)
        {
            try
            {
                var result = await _roomService.RemoveFloorFromRoomAsync(roomId, floorId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
