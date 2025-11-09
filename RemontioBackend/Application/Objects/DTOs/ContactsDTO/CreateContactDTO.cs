using System;

namespace Application.Objects.DTOs.ContactsDTO
{
    public class CreateContactDTO
    {
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public required string ContactDetails { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public bool IsPrivate { get; set; } = false;
        public required string UserId { get; set; }
    }
}
