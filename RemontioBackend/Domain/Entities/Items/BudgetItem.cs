using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Items
{
    public class BudgetItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public BudgetItemCategory Category { get; set; }
        public float Price { get; set; }
        public float Total { get; set; }
        public float EstimatedPrice { get; set; }
        public bool IsCompleted { get; set; }

        public Guid BudgetId { get; set; }
        public Budget? Budget { get; set; }
    }
}
