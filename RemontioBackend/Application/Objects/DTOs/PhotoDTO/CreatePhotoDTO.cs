using Microsoft.AspNetCore.Http;
using System;

namespace Application.Objects.DTOs.PhotoDTO
{
    public class CreatePhotoDTO
    {
        public IFormFile File { get; set; } = null!;
        public string? Description { get; set; }
        public string? StorageProvider { get; set; } = "local";
        public Guid? RoomId { get; set; }
        public Guid? ProjectId { get; set; }
        public string? UserId { get; set; }
    }
}
