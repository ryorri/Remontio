using Application.Interfaces.ServiceInterfaces;
using Application.Objects.DTOs.ListDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ListController : ControllerBase
    {
        private readonly IListService _listService;

        public ListController(IListService listService)
        {
            _listService = listService;
        }

        [Authorize]
        [HttpPost("create-list")]
        public async Task<ActionResult<bool>> CreateList(CreateListDTO listDTO)
        {
            try
            {
                var result = await _listService.CreateListAsync(listDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("get-list-by-id")]
        public async Task<ActionResult<ListDataDTO>> GetListById(string listId)
        {
            try
            {
                var result = await _listService.GetListAsync(listId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("get-list")]
        public async Task<ActionResult<List<ListDataDTO>>> GetList()
        {
            try
            {
                var result = await _listService.GetAllListsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("get-list-by-user-id")]
        public async Task<ActionResult<List<ListDataDTO>>> GetListByUserId(string userId)
        {
            try
            {
                var result = await _listService.GetAllListsByUserIdAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("get-list-by-project-id")]
        public async Task<ActionResult<List<ListDataDTO>>> GetListByProjectId(string projectId)
        {
            try
            {
                var result = await _listService.GetAllListsByProjectIdAsync(projectId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("get-list-by-room-id")]
        public async Task<ActionResult<List<ListDataDTO>>> GetListByRoomId(string roomId)
        {
            try
            {
                var result = await _listService.GetAllListsByRoomIdAsync(roomId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("delete-list")]
        public async Task<ActionResult<bool>> DeleteList(string listId)
        {
            try
            {
                var result = await _listService.DeleteListAsync(listId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("edit-list")]
        public async Task<ActionResult<bool>> EditList(ListDataDTO listDTO)
        {
            try
            {
                var result = await _listService.UpdateListAsync(listDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("add-item")]
        public async Task<ActionResult<bool>> AddItem(string listId, string name, int quantity, float price)
        {
            try
            {
                var result = await _listService.AddItemAsync(listId, name, quantity, price);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("remove-item")]
        public async Task<ActionResult<bool>> RemoveItem(string listId, string itemId)
        {
            try
            {
                var result = await _listService.RemoveItemAsync(listId, itemId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("mark-item-bought")]
        public async Task<ActionResult<bool>> MarkItemBought(string listId, string itemId, bool isBought)
        {
            try
            {
                var result = await _listService.MarkItemBoughtAsync(listId, itemId, isBought);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("clear-items")]
        public async Task<ActionResult<bool>> ClearItems(string listId)
        {
            try
            {
                var result = await _listService.ClearItemsAsync(listId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
