using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Objects.DTOs.RoomDTO
{
    public class WallDTO
    {
        public required string Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public required double CalculatedArea { get; set; } 
        public required Guid RoomId { get; set; }
    }
}
