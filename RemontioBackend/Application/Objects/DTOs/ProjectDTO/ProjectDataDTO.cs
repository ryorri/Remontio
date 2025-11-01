using Application.Objects.DTOs.UserDTO;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Objects.DTOs.ProjectDTO
{
    public class ProjectDataDTO
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public required DateTime CreateAt { get; set; }
        public DateTime ClosedAt { get; set; }
        public StatusEnum Status { get; set; }
        public required UserDataDTO User {  get; set; }
    }
}
