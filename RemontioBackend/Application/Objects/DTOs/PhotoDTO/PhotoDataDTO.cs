using System;

namespace Application.Objects.DTOs.Photos
{
    public class PhotoDataDTO
    {
        public string Id { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public long Size { get; set; }
        public string Url { get; set; } = null!;
        public string StorageProvider { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public Guid? ProjectId { get; set; }
        public Guid? RoomId { get; set; }
        public string? UserId { get; set; }
    }
}
