using Application.Interfaces.ServiceInterfaces;
using Application.Objects.DTOs.ContactsDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _contactService;

        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [Authorize]
        [HttpPost("create-contact")]
        public async Task<ActionResult<bool>> CreateContact(CreateContactDTO contactDTO)
        {
            try
            {
                var result = await _contactService.CreateContactAsync(contactDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("edit-contact")]
        public async Task<ActionResult<bool>> EditContact(ContactDataDTO contactDTO)
        {
            try
            {
                var result = await _contactService.UpdateContactAsync(contactDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("delete-contact")]
        public async Task<ActionResult<bool>> DeleteContact(string contactId)
        {
            try
            {
                var result = await _contactService.DeleteContactAsync(contactId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("get-contact-by-id")]
        public async Task<ActionResult<ContactDataDTO>> GetContactById(string contactId)
        {
            try
            {
                var result = await _contactService.GetContactAsync(contactId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("get-contact-list")]
        public async Task<ActionResult<List<ContactDataDTO>>> GetContactList()
        {
            try
            {
                var result = await _contactService.GetAllContactsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("get-contact-list-by-user-id")]
        public async Task<ActionResult<List<ContactDataDTO>>> GetContactListByUserId(string userId)
        {
            try
            {
                var result = await _contactService.GetAllContactsByUserIdAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("change-privacy")]
        public async Task<ActionResult<bool>> ChangePrivacy(string contactId, bool isPrivate)
        {
            try
            {
                var result = await _contactService.ChangePrivacyAsync(contactId, isPrivate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
