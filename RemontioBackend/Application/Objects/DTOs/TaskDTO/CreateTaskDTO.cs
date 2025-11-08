using Domain.Enums;
namespace Application.Objects.DTOs.TaskDTO
{
    public class CreateTaskDTO
    {
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public StatusEnum Status { get; set; }
        public PriorityEnum Priority { get; set; }
        public required DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public DateTime StartAt { get; set; }
        public DateTime ClosedAt { get; set; }
        
        public required Guid RoomId { get; set; }
        public required Guid ProjectId { get; set; }
        public required string UserId { get; set; }
    }
}
