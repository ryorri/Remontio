using System;

namespace Application.Objects.DTOs.ListDTO
{
    public class CreateListDTO
    {
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public required DateTime CreateAt { get; set; } = DateTime.UtcNow;

        public required Guid RoomId { get; set; }
        public required Guid ProjectId { get; set; }
        public required string UserId { get; set; }
    }
}
