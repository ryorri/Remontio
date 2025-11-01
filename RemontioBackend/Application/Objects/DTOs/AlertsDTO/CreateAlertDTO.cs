using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Objects.DTOs.AlertsDTO
{
    public class CreateAlertDTO
    {
        public required string Message { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public required string UserId { get; set; }

        public string Status { get; set; } = "To Do";
        public string Priority { get; set; } = "Medium";
    }
}
