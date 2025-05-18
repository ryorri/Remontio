using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Calculations
    {
        public Guid Id { get; set; }
        public float Value { get; set; }
        public CalculationsTypeEnum Type { get; set; }

        public Guid RoomId { get; set; } // FK
        public Room? Room { get; set; }  // Nav

        public Guid ProjectId { get; set; } // FK
        public Project? Project { get; set; }  // Nav

        public string UserId { get; set; } = string.Empty; // FK
        public User? User { get; set; }  // Nav
    }
}
