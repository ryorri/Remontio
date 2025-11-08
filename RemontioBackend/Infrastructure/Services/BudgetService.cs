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
                await _dbContext.Budgets.AddAsync(entity);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
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
                }
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
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
                throw new ArgumentException($"Error: ${ex}");
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
                throw new ArgumentException($"Error: ${ex}");
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
                throw new ArgumentException($"Error: ${ex}");
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
                throw new ArgumentException($"Error: ${ex}");
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
                throw new ArgumentException($"Error: ${ex}");
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
                throw new ArgumentException($"Error: ${ex}");
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
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<bool> AddItemAsync(string budgetId, string name, float price, float total, float estimatedPrice)
        {
            try
            {
                var budgetGuid = GuidValidator.ValidateGuid(budgetId);
                var budget = await _dbContext.Budgets.Include(b => b.Items).FirstOrDefaultAsync(b => b.Id == budgetGuid);
                if (budget == null)
                    return false;

                var createDto = new CreateBudgetItemDTO
                {
                    Name = name,
                    Price = price,
                    Total = total,
                    EstimatetPrice = estimatedPrice,
                    IsCompleted = false
                };

                var newItem = _mapper.Map<BudgetItem>(createDto);
                newItem.Id = Guid.NewGuid();
                budget.Items.Add(newItem);

                await RecalculateTotals(budget);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
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
                throw new ArgumentException($"Error: ${ex}");
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
                throw new ArgumentException($"Error: ${ex}");
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
                throw new ArgumentException($"Error: ${ex}");
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
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<bool> AddShoppingListAsItemAsync(string budgetId, string shoppingListId, bool snapshot)
        {
            try
            {
                var budgetGuid = GuidValidator.ValidateGuid(budgetId);
                var listGuid = GuidValidator.ValidateGuid(shoppingListId);

                var budget = await _dbContext.Budgets.Include(b => b.Items).FirstOrDefaultAsync(b => b.Id == budgetGuid);
                if (budget == null)
                    return false;

                var shoppingList = await _dbContext.ShoppingLists.Include(l => l.Items).FirstOrDefaultAsync(l => l.Id == listGuid);
                if (shoppingList == null)
                    return false;

                float actualTotal = shoppingList.Items.Sum(i => i.Price * i.Quantity);
                float estimated = actualTotal;

                var createDto = new CreateBudgetItemDTO
                {
                    Name = $"ShoppingList: {shoppingList.Name}",
                    Price = 0,
                    Total = snapshot ? actualTotal : 0,
                    EstimatetPrice = snapshot ? estimated : 0,
                    IsCompleted = false
                };

                var newItem = _mapper.Map<BudgetItem>(createDto);
                newItem.Id = Guid.NewGuid();
                budget.Items.Add(newItem);

                await RecalculateTotals(budget);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        private Task RecalculateTotals(Budget budget)
        {
            budget.Total = budget.Items.Sum(i => i.Total);
            budget.EstimatedPrice = budget.Items.Sum(i => i.EstimatetPrice);
            budget.Spent = budget.Items.Where(i => i.IsCompleted).Sum(i => i.Total);
            return Task.CompletedTask;
        }
    }
}
