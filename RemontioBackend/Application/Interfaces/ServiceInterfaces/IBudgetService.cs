using Application.Objects.DTOs.BudgetDTO;
using Application.Objects.DTOs;
using Application.Objects.DTOs.BudgetItemDTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces.ServiceInterfaces
{
    public interface IBudgetService
    {
        Task<bool> CreateBudgetAsync(CreateBudgetDTO budgetDTO);
        Task<bool> UpdateBudgetAsync(BudgetDataDTO budgetDTO);
        Task<bool> DeleteBudgetAsync(string budgetId);
        Task<List<BudgetDataDTO>> GetAllBudgetsAsync();
        Task<List<BudgetDataDTO>> GetAllBudgetsByUserIdAsync(string userId);
        Task<List<BudgetDataDTO>> GetAllBudgetsByProjectIdAsync(string projectId);
        Task<List<BudgetDataDTO>> GetAllBudgetsByRoomIdAsync(string roomId);
        Task<BudgetDataDTO> GetBudgetAsync(string budgetId);
        Task<List<BudgetItemDataDTO>> GetBudgetItemsAsync(string budgetId);

        Task<bool> AddItemAsync(string budgetId, CreateBudgetItemDTO itemDTO);
        Task<bool> UpdateItemAsync(string budgetId, BudgetItemDataDTO itemDTO);
        Task<bool> RemoveItemAsync(string budgetId, string itemId);
        Task<bool> MarkItemCompletedAsync(string budgetId, string itemId, bool isCompleted);
        Task<bool> ClearItemsAsync(string budgetId);
        Task<bool> RecalculateBudgetAsync(string budgetId);
        Task<bool> AddShoppingListAsItemAsync(string budgetId, string shoppingListId, bool snapshot);
    }
}
