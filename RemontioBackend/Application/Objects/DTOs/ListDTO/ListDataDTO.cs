using System;
using System.Collections.Generic;

namespace Application.Objects.DTOs.ListDTO
{
    public class ListDataDTO
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public required DateTime CreateAt { get; set; }
        public List<string> ItemIds { get; set; } = new List<string>();

        public required string RoomId { get; set; }
        public required string ProjectId { get; set; }
        public required string UserId { get; set; }
    }
}
