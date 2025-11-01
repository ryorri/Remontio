using Application.Objects.DTOs.ProjectDTO;
using Domain.Enums;
using System;
using System.Collections.Generic;

namespace Application.Objects.DTOs.RoomDTO
{
    public class RoomDataDTO
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public required DateTime CreateAt { get; set; }
        public DateTime ClosedAt { get; set; }
        public StatusEnum Status { get; set; }

        public required string ProjectId { get; set; }
        public required string UserId { get; set; }
        public ProjectDataDTO? Project { get; set; }
        public List<string> TaskIds { get; set; } = new List<string>();
        public List<string> CalculationIds { get; set; } = new List<string>();
        public List<string> ShoppingListIds { get; set; } = new List<string>();
        public List<string> BudgetIds { get; set; } = new List<string>();
        public List<string> PhotoIds { get; set; } = new List<string>();
        public List<string> WallIds { get; set; } = new List<string>();
    }
}
