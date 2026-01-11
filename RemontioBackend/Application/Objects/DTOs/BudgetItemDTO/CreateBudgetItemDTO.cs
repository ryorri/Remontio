using Domain.Enums;
using System;

namespace Application.Objects.DTOs.BudgetItemDTO
{
    public class CreateBudgetItemDTO
    {
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public BudgetItemCategory Category { get; set; }
        public float Price { get; set; }
        public float Total { get; set; }
        public float EstimatedPrice { get; set; }
        public bool IsCompleted { get; set; } = false;
    }
}
