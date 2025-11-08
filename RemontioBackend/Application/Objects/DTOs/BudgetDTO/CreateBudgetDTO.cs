using System;

namespace Application.Objects.DTOs.BudgetDTO
{
    public class CreateBudgetDTO
    {
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public required DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public float Total { get; set; }
        public float Spent { get; set; }
        public float EstimatedPrice { get; set; }

        public required Guid RoomId { get; set; }
        public required Guid ProjectId { get; set; }
        public required string UserId { get; set; }
    }
}
