using Application.Objects.DTOs.ContactsDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.ServiceInterfaces
{
    public interface IContactService
    {
        Task<bool> CreateContactAsync(CreateContactDTO contactDTO);
        Task<bool> UpdateContactAsync(ContactDataDTO contactDTO);
        Task<bool> DeleteContactAsync(string contactId);
        Task<List<ContactDataDTO>> GetAllContactsAsync();
        Task<List<ContactDataDTO>> GetAllContactsByUserIdAsync(string userId);
        Task<ContactDataDTO> GetContactAsync(string contactId);
        Task<bool> ChangePrivacyAsync(string contactId, bool isPrivate);
    }
}
