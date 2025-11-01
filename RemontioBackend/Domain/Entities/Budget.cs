using Domain.Entities.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Budget
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public float Total { get; set; }
        public float Spent { get; set; }
        public float EstimatedPrice { get; set; }
        public List<BudgetItem> Items { get; set; } = new List<BudgetItem>();

        public Guid RoomId { get; set; } // FK
        public Room? Room { get; set; }  // Nav

        public Guid ProjectId { get; set; } // FK
        public Project? Project { get; set; }  // Nav

        public string UserId { get; set; } = string.Empty; // FK
        public User? User { get; set; }  // Nav
    }
}
