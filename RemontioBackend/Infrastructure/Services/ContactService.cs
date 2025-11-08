using Application.Interfaces.ServiceInterfaces;
using Application.Objects.DTOs.ContactsDTO;
using Application.Validators;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ContactService : IContactService
    {
        private readonly RemontioDbContext _dbContext;
        private readonly IMapper _mapper;

        public ContactService(RemontioDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<bool> CreateContactAsync(CreateContactDTO contactDTO)
        {
            if (contactDTO == null)
                throw new ArgumentNullException(nameof(contactDTO));

            try
            {
                var entity = _mapper.Map<Contacts>(contactDTO);
                await _dbContext.Contacts.AddAsync(entity);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<bool> UpdateContactAsync(ContactDataDTO contactDTO)
        {
            if (contactDTO == null)
                throw new ArgumentNullException(nameof(contactDTO));

            try
            {
                var guid = GuidValidator.ValidateGuid(contactDTO.Id);
                var contact = await _dbContext.Contacts.FindAsync(guid);

                if (contact != null)
                {
                    contact.Name = contactDTO.Name;
                    contact.Description = contactDTO.Description;
                    contact.ContactDetails = contactDTO.ContactDetails;
                }

                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<bool> DeleteContactAsync(string contactId)
        {
            if (contactId == null)
                throw new ArgumentNullException(nameof(contactId));

            try
            {
                var guid = GuidValidator.ValidateGuid(contactId);
                var contact = await _dbContext.Contacts.FindAsync(guid);
                if (contact != null)
                {
                    _dbContext.Contacts.Remove(contact);
                }
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<List<ContactDataDTO>> GetAllContactsAsync()
        {
            try
            {
                var contacts = await _dbContext.Contacts
                                               .Include(x => x.User)
                                               .ToListAsync();
                return _mapper.Map<List<ContactDataDTO>>(contacts);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<List<ContactDataDTO>> GetAllContactsByUserIdAsync(string userId)
        {
            try
            {
                var contacts = await _dbContext.Contacts
                                               .Include(x => x.User)
                                               .Where(x => x.UserId == userId)
                                               .ToListAsync();
                return _mapper.Map<List<ContactDataDTO>>(contacts);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<ContactDataDTO> GetContactAsync(string contactId)
        {
            try
            {
                var guid = GuidValidator.ValidateGuid(contactId);
                var contact = await _dbContext.Contacts
                                               .Include(x => x.User)
                                               .FirstOrDefaultAsync(x => x.Id == guid);
                return _mapper.Map<ContactDataDTO>(contact);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error: ${ex}");
            }
        }

        public async Task<bool> ChangePrivacyAsync(string contactId, bool isPrivate)
        {
            try
            {
                var guid = GuidValidator.ValidateGuid(contactId);
                var contact = await _dbContext.Contacts.FindAsync(guid);
                if (contact != null)
                {
                    contact.IsPrivate = isPrivate;
                }
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
