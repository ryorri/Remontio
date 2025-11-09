using System;

namespace Application.Objects.DTOs.BudgetItemDTO
{
    public class BudgetItemDataDTO
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public float Price { get; set; }
        public float Total { get; set; }
        public float EstimatetPrice { get; set; }
        public bool IsCompleted { get; set; }
    }
}
