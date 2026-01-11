using Application.Interfaces.ServiceInterfaces;
using Application.Objects.DTOs.BudgetDTO;
using Application.Objects.DTOs;
using Application.Objects.DTOs.BudgetItemDTO;
using Application.Validators;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.Items;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;

namespace Infrastructure.Services
{
    public class BudgetService : IBudgetService
    {
        private readonly RemontioDbContext _dbContext;
        private readonly IMapper _mapper;

        public BudgetService(RemontioDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<bool> CreateBudgetAsync(CreateBudgetDTO budgetDTO)
        {
            if (budgetDTO == null)
                throw new ArgumentNullException(nameof(budgetDTO));

            try
            {
                var entity = _mapper.Map<Budget>(budgetDTO);
                entity.CreateAt = DateTime.UtcNow;
                await _dbContext.Budgets.AddAsync(entity);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: {ex.Message}", ex);
            }
        }

        public async Task<bool> UpdateBudgetAsync(BudgetDataDTO budgetDTO)
        {
            if (budgetDTO == null)
                throw new ArgumentNullException(nameof(budgetDTO));

            try
            {
                var guid = GuidValidator.ValidateGuid(budgetDTO.Id);
                var budget = await _dbContext.Budgets.Include(b => b.Items).FirstOrDefaultAsync(b => b.Id == guid);
                if (budget != null)
                {
                    budget.Name = budgetDTO.Name;
                    budget.Description = budgetDTO.Description;
                    budget.Spent = budgetDTO.Spent;

                }
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteBudgetAsync(string budgetId)
        {
            if (budgetId == null)
                throw new ArgumentNullException(nameof(budgetId));

            try
            {
                var guid = GuidValidator.ValidateGuid(budgetId);
                var budget = await _dbContext.Budgets.FindAsync(guid);
                if (budget != null)
                {
                    _dbContext.Budgets.Remove(budget);
                }
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: {ex.Message}", ex);
            }
        }

        public async Task<List<BudgetDataDTO>> GetAllBudgetsAsync()
        {
            try
            {
                var budgets = await _dbContext.Budgets.Include(b => b.Items).ToListAsync();
                return _mapper.Map<List<BudgetDataDTO>>(budgets);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: {ex.Message}", ex);
            }
        }

        public async Task<List<BudgetDataDTO>> GetAllBudgetsByUserIdAsync(string userId)
        {
            try
            {
                var budgets = await _dbContext.Budgets.Include(b => b.Items).Where(b => b.UserId == userId).ToListAsync();
                return _mapper.Map<List<BudgetDataDTO>>(budgets);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: {ex.Message}", ex);
            }
        }

        public async Task<List<BudgetDataDTO>> GetAllBudgetsByProjectIdAsync(string projectId)
        {
            try
            {
                var guid = GuidValidator.ValidateGuid(projectId);
                var budgets = await _dbContext.Budgets.Include(b => b.Items).Where(b => b.ProjectId == guid).ToListAsync();
                return _mapper.Map<List<BudgetDataDTO>>(budgets);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: {ex.Message}", ex);
            }
        }

        public async Task<List<BudgetDataDTO>> GetAllBudgetsByRoomIdAsync(string roomId)
        {
            try
            {
                var guid = GuidValidator.ValidateGuid(roomId);
                var budgets = await _dbContext.Budgets.Include(b => b.Items).Where(b => b.RoomId == guid).ToListAsync();
                return _mapper.Map<List<BudgetDataDTO>>(budgets);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: {ex.Message}", ex);
            }
        }

        public async Task<BudgetDataDTO> GetBudgetAsync(string budgetId)
        {
            try
            {
                var budget = await _dbContext.Budgets.Include(b => b.Items).FirstOrDefaultAsync(b => b.Id.ToString() == budgetId);
                return _mapper.Map<BudgetDataDTO>(budget);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: {ex.Message}", ex);
            }
        }

        public async Task<List<BudgetItemDataDTO>> GetBudgetItemsAsync(string budgetId)
        {
            try
            {
                var budgetGuid = GuidValidator.ValidateGuid(budgetId);
                var budget = await _dbContext.Budgets.Include(b => b.Items).FirstOrDefaultAsync(b => b.Id == budgetGuid);
                if (budget == null)
                    return new List<BudgetItemDataDTO>();

                return _mapper.Map<List<BudgetItemDataDTO>>(budget.Items);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: {ex.Message}", ex);
            }
        }

        public async Task<bool> AddItemAsync(string budgetId, CreateBudgetItemDTO itemDTO)
        {
            try
            {
                var budgetGuid = GuidValidator.ValidateGuid(budgetId);
                var budget = await _dbContext.Budgets
                    .FirstOrDefaultAsync(b => b.Id == budgetGuid);
                    
                if (budget == null)
                    return false;

                var newItem = new BudgetItem
                {
                    Id = Guid.NewGuid(),
                    Name = itemDTO.Name,
                    Description = itemDTO.Description,
                    Category = itemDTO.Category,
                    Price = itemDTO.Price,
                    Total = itemDTO.Total,
                    EstimatedPrice = itemDTO.EstimatedPrice,
                    IsCompleted = itemDTO.IsCompleted,
                    BudgetId = budgetGuid
                };

                await _dbContext.BudgetItems.AddAsync(newItem);
                await _dbContext.SaveChangesAsync();

                // Reload budget with items for recalculation
                budget = await _dbContext.Budgets
                    .Include(b => b.Items)
                    .FirstOrDefaultAsync(b => b.Id == budgetGuid);
                    
                if (budget != null)
                {
                    budget.Total = budget.Items.Sum(i => i.Total);
                    budget.EstimatedPrice = budget.Items.Sum(i => i.EstimatedPrice);
                    budget.Spent = budget.Items.Where(i => i.IsCompleted).Sum(i => i.Total);
                    await _dbContext.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: {ex.Message}", ex);
            }
        }

        public async Task<bool> RemoveItemAsync(string budgetId, string itemId)
        {
            try
            {
                var budgetGuid = GuidValidator.ValidateGuid(budgetId);
                var itemGuid = GuidValidator.ValidateGuid(itemId);

                var budget = await _dbContext.Budgets.Include(b => b.Items).FirstOrDefaultAsync(b => b.Id == budgetGuid);
                if (budget == null)
                    return false;

                var item = budget.Items.FirstOrDefault(i => i.Id == itemGuid);
                if (item != null)
                    budget.Items.Remove(item);

                await RecalculateTotals(budget);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: {ex.Message}", ex);
            }
        }

        public async Task<bool> UpdateItemAsync(string budgetId, BudgetItemDataDTO itemDTO)
        {
            try
            {
                var budgetGuid = GuidValidator.ValidateGuid(budgetId);
                var itemGuid = GuidValidator.ValidateGuid(itemDTO.Id);

                var item = await _dbContext.BudgetItems.FirstOrDefaultAsync(i => i.Id == itemGuid && i.BudgetId == budgetGuid);
                if (item == null)
                    return false;

                item.Name = itemDTO.Name;
                item.Description = itemDTO.Description;
                item.Category = itemDTO.Category;
                item.Price = itemDTO.Price;
                item.Total = itemDTO.Total;
                item.EstimatedPrice = itemDTO.EstimatedPrice;
                item.IsCompleted = itemDTO.IsCompleted;

                await _dbContext.SaveChangesAsync();

                // Reload budget with items for recalculation
                var budget = await _dbContext.Budgets
                    .Include(b => b.Items)
                    .FirstOrDefaultAsync(b => b.Id == budgetGuid);
                    
                if (budget != null)
                {
                    budget.Total = budget.Items.Sum(i => i.Total);
                    budget.EstimatedPrice = budget.Items.Sum(i => i.EstimatedPrice);
                    budget.Spent = budget.Items.Where(i => i.IsCompleted).Sum(i => i.Total);
                    await _dbContext.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: {ex.Message}", ex);
            }
        }

        public async Task<bool> MarkItemCompletedAsync(string budgetId, string itemId, bool isCompleted)
        {
            try
            {
                var budgetGuid = GuidValidator.ValidateGuid(budgetId);
                var itemGuid = GuidValidator.ValidateGuid(itemId);

                var budget = await _dbContext.Budgets.Include(b => b.Items).FirstOrDefaultAsync(b => b.Id == budgetGuid);
                if (budget == null)
                    return false;

                var item = budget.Items.FirstOrDefault(i => i.Id == itemGuid);
                if (item != null)
                    item.IsCompleted = isCompleted;

                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: {ex.Message}", ex);
            }
        }

        public async Task<bool> ClearItemsAsync(string budgetId)
        {
            try
            {
                var budgetGuid = GuidValidator.ValidateGuid(budgetId);
                var budget = await _dbContext.Budgets.Include(b => b.Items).FirstOrDefaultAsync(b => b.Id == budgetGuid);
                if (budget == null)
                    return false;

                budget.Items.Clear();
                await RecalculateTotals(budget);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: {ex.Message}", ex);
            }
        }

        public async Task<bool> RecalculateBudgetAsync(string budgetId)
        {
            try
            {
                var budgetGuid = GuidValidator.ValidateGuid(budgetId);
                var budget = await _dbContext.Budgets.Include(b => b.Items).FirstOrDefaultAsync(b => b.Id == budgetGuid);
                if (budget == null)
                    return false;

                await RecalculateTotals(budget);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: {ex.Message}", ex);
            }
        }

        public async Task<bool> AddShoppingListAsItemAsync(string budgetId, string shoppingListId, bool snapshot)
        {
            try
            {
                var budgetGuid = GuidValidator.ValidateGuid(budgetId);
                var listGuid = GuidValidator.ValidateGuid(shoppingListId);

                var budget = await _dbContext.Budgets.FirstOrDefaultAsync(b => b.Id == budgetGuid);
                if (budget == null)
                    return false;

                var shoppingList = await _dbContext.ShoppingLists.Include(l => l.Items).FirstOrDefaultAsync(l => l.Id == listGuid);
                if (shoppingList == null)
                    return false;

                float actualTotal = shoppingList.Items.Sum(i => i.Price * i.Quantity);
                float estimated = actualTotal;

                var newItem = new BudgetItem
                {
                    Id = Guid.NewGuid(),
                    Name = shoppingList.Name,
                    Description = string.Empty,
                    Category = BudgetItemCategory.Other,
                    Price = 0,
                    Total = snapshot ? actualTotal : 0,
                    EstimatedPrice = snapshot ? estimated : 0,
                    IsCompleted = false,
                    BudgetId = budgetGuid
                };

                await _dbContext.BudgetItems.AddAsync(newItem);
                await _dbContext.SaveChangesAsync();

                // Reload budget with items for recalculation
                budget = await _dbContext.Budgets
                    .Include(b => b.Items)
                    .FirstOrDefaultAsync(b => b.Id == budgetGuid);
                    
                if (budget != null)
                {
                    budget.Total = budget.Items.Sum(i => i.Total);
                    budget.EstimatedPrice = budget.Items.Sum(i => i.EstimatedPrice);
                    budget.Spent = budget.Items.Where(i => i.IsCompleted).Sum(i => i.Total);
                    await _dbContext.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: {ex.Message}", ex);
            }
        }

        private Task RecalculateTotals(Budget budget)
        {
            budget.Total = budget.Items.Sum(i => i.Total);
            budget.EstimatedPrice = budget.Items.Sum(i => i.EstimatedPrice);
            budget.Spent = budget.Items.Where(i => i.IsCompleted).Sum(i => i.Total);
            return Task.CompletedTask;
        }
    }
}
