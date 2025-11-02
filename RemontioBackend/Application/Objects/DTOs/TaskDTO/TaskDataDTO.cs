using Application.Objects.DTOs.ProjectDTO;
using Application.Objects.DTOs.RoomDTO;
using Domain.Enums;
using System;

namespace Application.Objects.DTOs.TaskDTO
{
    public class TaskDataDTO
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public StatusEnum Status { get; set; }
        public PriorityEnum Priority { get; set; }
        public required DateTime CreateAt { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime ClosedAt { get; set; }

        public required string RoomId { get; set; }

        public required string ProjectId { get; set; }

        public required string UserId { get; set; }
    }
}
