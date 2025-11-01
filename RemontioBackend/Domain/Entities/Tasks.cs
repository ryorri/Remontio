using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Tasks
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public StatusEnum Status { get; set; }
        public PriorityEnum Priority { get; set; }
        public required DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public DateTime StartAt { get; set; }
        public DateTime ClosedAt { get; set; }

        public Guid RoomId { get; set; } // FK
        public Room? Room { get; set; }  // Nav

        public Guid ProjectId { get; set; } // FK
        public Project? Project { get; set; }  // Nav
    }
}
