using Application.Interfaces.ServiceInterfaces;
using Application.Objects.DTOs.ListDTO;
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
    public class ListService : IListService
    {
        private readonly RemontioDbContext _dbContext;
        private readonly IMapper _mapper;

        public ListService(RemontioDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<bool> CreateListAsync(CreateListDTO listDTO)
        {
            if (listDTO == null)
                throw new ArgumentNullException(nameof(listDTO));

            try
            {
                var entity = _mapper.Map<ShoppingList>(listDTO);
                entity.CreateAt = DateTime.UtcNow;
                await _dbContext.ShoppingLists.AddAsync(entity);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<bool> UpdateListAsync(ListDataDTO listDTO)
        {
            if (listDTO == null)
                throw new ArgumentNullException(nameof(listDTO));

            try
            {
                var guid = GuidValidator.ValidateGuid(listDTO.Id);
                var list = await _dbContext.ShoppingLists.FindAsync(guid);

                if (list != null)
                {
                    list.Name = listDTO.Name;
                    list.Description = listDTO.Description;
                }

                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<bool> DeleteListAsync(string listId)
        {
            if (listId == null)
                throw new ArgumentNullException(nameof(listId));

            try
            {
                var guid = GuidValidator.ValidateGuid(listId);
                var list = await _dbContext.ShoppingLists.FindAsync(guid);
                if (list != null)
                {
                    _dbContext.ShoppingLists.Remove(list);
                }
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<List<ListDataDTO>> GetAllListsAsync()
        {
            try
            {
                var lists = await _dbContext.ShoppingLists
                                             .Include(x => x.User)
                                             .ToListAsync();
                return _mapper.Map<List<ListDataDTO>>(lists);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<List<ListDataDTO>> GetAllListsByUserIdAsync(string userId)
        {
            try
            {
                var lists = await _dbContext.ShoppingLists
                                             .Include(x => x.User)
                                             .Where(x => x.UserId == userId)
                                             .ToListAsync();
                return _mapper.Map<List<ListDataDTO>>(lists);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<List<ListDataDTO>> GetAllListsByProjectIdAsync(string projectId)
        {
            try
            {
                var guid = GuidValidator.ValidateGuid(projectId);
                var lists = await _dbContext.ShoppingLists
                                             .Include(x => x.User)
                                             .Where(x => x.ProjectId == guid)
                                             .ToListAsync();
                return _mapper.Map<List<ListDataDTO>>(lists);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<List<ListDataDTO>> GetAllListsByRoomIdAsync(string roomId)
        {
            try
            {
                var guid = GuidValidator.ValidateGuid(roomId);
                var lists = await _dbContext.ShoppingLists
                                             .Include(x => x.User)
                                             .Where(x => x.RoomId == guid)
                                             .ToListAsync();
                return _mapper.Map<List<ListDataDTO>>(lists);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<ListDataDTO> GetListAsync(string listId)
        {
            try
            {
                var guid = GuidValidator.ValidateGuid(listId);
                var list = await _dbContext.ShoppingLists
                                            .Include(x => x.User)
                                            .FirstOrDefaultAsync(x => x.Id == guid);
                return _mapper.Map<ListDataDTO>(list);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<bool> AddItemAsync(string listId, string name, int quantity, float price)
        {
            try
            {
                var guid = GuidValidator.ValidateGuid(listId);
                var list = await _dbContext.ShoppingLists.FirstOrDefaultAsync(x => x.Id == guid);
                if (list == null)
                    return false;

                list.Items.Add(new ShoppingListItem
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    Quantity = quantity,
                    Price = price,
                    IsBought = false
                });

                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<bool> RemoveItemAsync(string listId, string itemId)
        {
            try
            {
                var listGuid = GuidValidator.ValidateGuid(listId);
                var itemGuid = GuidValidator.ValidateGuid(itemId);

                var list = await _dbContext.ShoppingLists.FirstOrDefaultAsync(x => x.Id == listGuid);
                if (list == null)
                    return false;

                var item = list.Items.FirstOrDefault(i => i.Id == itemGuid);
                if (item != null)
                {
                    list.Items.Remove(item);
                }

                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<bool> MarkItemBoughtAsync(string listId, string itemId, bool isBought)
        {
            try
            {
                var listGuid = GuidValidator.ValidateGuid(listId);
                var itemGuid = GuidValidator.ValidateGuid(itemId);

                var list = await _dbContext.ShoppingLists.FirstOrDefaultAsync(x => x.Id == listGuid);
                if (list == null)
                    return false;

                var item = list.Items.FirstOrDefault(i => i.Id == itemGuid);
                if (item != null)
                {
                    item.IsBought = isBought;
                }

                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<bool> ClearItemsAsync(string listId)
        {
            try
            {
                var listGuid = GuidValidator.ValidateGuid(listId);
                var list = await _dbContext.ShoppingLists.FirstOrDefaultAsync(x => x.Id == listGuid);
                if (list == null)
                    return false;

                list.Items.Clear();
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
