using Domain.Enums;
using System;

namespace Application.Objects.DTOs.RoomDTO
{
    public class CreateRoomDTO
    {
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ClosedAt { get; set; }
        public StatusEnum Status { get; set; }
        public required Guid UserId { get; set; }
    }
}
