using Application.Objects.DTOs.ListDTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces.ServiceInterfaces
{
    public interface IListService
    {
        Task<bool> CreateListAsync(CreateListDTO listDTO);
        Task<bool> UpdateListAsync(ListDataDTO listDTO);
        Task<bool> DeleteListAsync(string listId);
        Task<List<ListDataDTO>> GetAllListsAsync();
        Task<List<ListDataDTO>> GetAllListsByUserIdAsync(string userId);
        Task<List<ListDataDTO>> GetAllListsByProjectIdAsync(string projectId);
        Task<List<ListDataDTO>> GetAllListsByRoomIdAsync(string roomId);
        Task<ListDataDTO> GetListAsync(string listId);

        Task<bool> AddItemAsync(string listId, string name, int quantity, float price);
        Task<bool> RemoveItemAsync(string listId, string itemId);
        Task<bool> MarkItemBoughtAsync(string listId, string itemId, bool isBought);
        Task<bool> ClearItemsAsync(string listId);
    }
}
