using System;

namespace Application.Objects.DTOs.ContactsDTO
{
    public class ContactDataDTO
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public required string ContactDetails { get; set; }
        public required DateTime CreatedDate { get; set; }
        public bool IsPrivate { get; set; }
        public required string UserId { get; set; }
    }
}
