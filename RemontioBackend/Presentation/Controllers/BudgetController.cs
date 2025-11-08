using Application.Interfaces.ServiceInterfaces;
using Application.Objects.DTOs;
using Application.Objects.DTOs.BudgetDTO;
using Application.Objects.DTOs.BudgetItemDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BudgetController : ControllerBase
    {
        private readonly IBudgetService _budgetService;

        public BudgetController(IBudgetService budgetService)
        {
            _budgetService = budgetService;
        }
        [Authorize]
        [HttpPost("create-budget")]
        public async Task<ActionResult<bool>> CreateBudget(CreateBudgetDTO budgetDTO)
        {
            try
            {
                var result = await _budgetService.CreateBudgetAsync(budgetDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("get-budget-by-id")]
        public async Task<ActionResult<BudgetDataDTO>> GetBudgetById(string budgetId)
        {
            try
            {
                var result = await _budgetService.GetBudgetAsync(budgetId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("get-budget-list")]
        public async Task<ActionResult<List<BudgetDataDTO>>> GetBudgetList()
        {
            try
            {
                var result = await _budgetService.GetAllBudgetsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("get-budget-list-by-user-id")]
        public async Task<ActionResult<List<BudgetDataDTO>>> GetBudgetListByUserId(string userId)
        {
            try
            {
                var result = await _budgetService.GetAllBudgetsByUserIdAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("get-budget-list-by-project-id")]
        public async Task<ActionResult<List<BudgetDataDTO>>> GetBudgetListByProjectId(string projectId)
        {
            try
            {
                var result = await _budgetService.GetAllBudgetsByProjectIdAsync(projectId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("get-budget-list-by-room-id")]
        public async Task<ActionResult<List<BudgetDataDTO>>> GetBudgetListByRoomId(string roomId)
        {
            try
            {
                var result = await _budgetService.GetAllBudgetsByRoomIdAsync(roomId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("delete-budget")]
        public async Task<ActionResult<bool>> DeleteBudget(string budgetId)
        {
            try
            {
                var result = await _budgetService.DeleteBudgetAsync(budgetId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("edit-budget")]
        public async Task<ActionResult<bool>> EditBudget(BudgetDataDTO budgetDTO)
        {
            try
            {
                var result = await _budgetService.UpdateBudgetAsync(budgetDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Items
        [Authorize]
        [HttpGet("get-budget-items")]
        public async Task<ActionResult<List<BudgetItemDataDTO>>> GetBudgetItems(string budgetId)
        {
            try
            {
                var result = await _budgetService.GetBudgetItemsAsync(budgetId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("add-item")]
        public async Task<ActionResult<bool>> AddItem(string budgetId, string name, float price, float total, float estimatedPrice)
        {
            try
            {
                var result = await _budgetService.AddItemAsync(budgetId, name, price, total, estimatedPrice);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("remove-item")]
        public async Task<ActionResult<bool>> RemoveItem(string budgetId, string itemId)
        {
            try
            {
                var result = await _budgetService.RemoveItemAsync(budgetId, itemId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("mark-item-completed")]
        public async Task<ActionResult<bool>> MarkItemCompleted(string budgetId, string itemId, bool isCompleted)
        {
            try
            {
                var result = await _budgetService.MarkItemCompletedAsync(budgetId, itemId, isCompleted);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("clear-items")]
        public async Task<ActionResult<bool>> ClearItems(string budgetId)
        {
            try
            {
                var result = await _budgetService.ClearItemsAsync(budgetId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("recalculate")]
        public async Task<ActionResult<bool>> Recalculate(string budgetId)
        {
            try
            {
                var result = await _budgetService.RecalculateBudgetAsync(budgetId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("add-shopping-list-as-item")]
        public async Task<ActionResult<bool>> AddShoppingListAsItem(string budgetId, string shoppingListId, bool snapshot)
        {
            try
            {
                var result = await _budgetService.AddShoppingListAsItemAsync(budgetId, shoppingListId, snapshot);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
