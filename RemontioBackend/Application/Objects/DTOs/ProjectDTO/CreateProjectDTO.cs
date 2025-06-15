using Domain.Enums;

namespace Application.Objects.DTOs.ProjectDTO
{
    public class CreateProjectDTO
    {
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public required DateTime CreatedAt { get; set; }
        public required string UserId { get; set; }
        public StatusEnum Status { get; set; }
    }
}
