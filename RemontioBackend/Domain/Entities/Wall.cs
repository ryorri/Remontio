using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Wall
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double CalculatedArea { get; set; }

        public Guid RoomId { get; set; } // FK

        public Room? Room { get; set; }  // Nav  
    }
}
