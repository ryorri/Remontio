using Application.Interfaces.ServiceInterfaces;
using Application.Objects.DTOs.PhotoDTO;
using Application.Objects.DTOs.Photos;
using Application.Validators;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly RemontioDbContext _db;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<PhotoService> _logger;
        private readonly string _storageFolder;

        private static readonly HashSet<string> AllowedContentTypes = new()
        {
            "image/png", "image/jpeg", "image/jpg", "image/gif", "image/webp"
        };

        private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB

        public PhotoService(RemontioDbContext db, IMapper mapper, IWebHostEnvironment env, ILogger<PhotoService> logger)
        {
            _db = db;
            _mapper = mapper;
            _env = env;
            _logger = logger;

            _storageFolder = Path.Combine(
                _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"),
                "uploads",
                "photos");

            Directory.CreateDirectory(_storageFolder);
        }

        public async Task<PhotoDataDTO> UploadAsync(CreatePhotoDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (dto.File == null) throw new ArgumentNullException(nameof(dto.File));

            if (!AllowedContentTypes.Contains(dto.File.ContentType ?? string.Empty))
                throw new ArgumentException("Unsupported file type.");

            if (dto.File.Length <= 0 || dto.File.Length > MaxFileSizeBytes)
                throw new ArgumentException("File is empty or exceeds maximum allowed size.");

            try
            {
                var id = Guid.NewGuid();
                var ext = Path.GetExtension(dto.File.FileName);
                var storedFileName = $"{id}{ext}";
                var relativeUrl = $"/uploads/photos/{storedFileName}";
                var fullPath = Path.Combine(_storageFolder, storedFileName);

                await using (var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await dto.File.CopyToAsync(fs);
                }

                var entity = new Photos
                {
                    Id = id,
                    FileName = dto.File.FileName,
                    ContentType = dto.File.ContentType ?? "application/octet-stream",
                    Size = dto.File.Length,
                    Url = relativeUrl,
                    StorageProvider = string.IsNullOrWhiteSpace(dto.StorageProvider) ? "local" : dto.StorageProvider!,
                    ProjectId = dto.ProjectId ?? Guid.Empty,
                    RoomId = dto.RoomId ?? Guid.Empty,
                    UserId = dto.UserId ?? string.Empty,
                    CreateAt = DateTime.UtcNow
                };

                await _db.Photos.AddAsync(entity);
                await _db.SaveChangesAsync();

                return new PhotoDataDTO
                {
                    Id = entity.Id.ToString(),
                    FileName = entity.FileName,
                    ContentType = entity.ContentType,
                    Size = entity.Size,
                    Url = entity.Url,
                    StorageProvider = entity.StorageProvider,
                    CreatedAt = entity.CreateAt,
                    ProjectId = entity.ProjectId == Guid.Empty ? null : entity.ProjectId,
                    RoomId = entity.RoomId == Guid.Empty ? null : entity.RoomId,
                    UserId = string.IsNullOrWhiteSpace(entity.UserId) ? null : entity.UserId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Upload failed");
                throw new ArgumentException($"Error: {ex.Message}", ex);
            }
        }

        public async Task<Stream?> GetFileStreamAsync(string photoId)
        {
            var guid = GuidValidator.ValidateGuid(photoId);

            var meta = await _db.Photos.FindAsync(guid);
            if (meta == null) return null;

            if (string.Equals(meta.StorageProvider, "local", StringComparison.OrdinalIgnoreCase))
            {
                var fileName = Path.GetFileName(meta.Url);
                var fullPath = Path.Combine(_storageFolder, fileName);
                if (!File.Exists(fullPath)) return null;

                return new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            return null;
        }

        public async Task<PhotoDataDTO?> GetMetadataAsync(string photoId)
        {
            var guid = GuidValidator.ValidateGuid(photoId);

            var p = await _db.Photos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == guid);
            if (p == null) return null;

            return new PhotoDataDTO
            {
                Id = p.Id.ToString(),
                FileName = p.FileName,
                ContentType = p.ContentType,
                Size = p.Size,
                Url = p.Url,
                StorageProvider = p.StorageProvider,
                CreatedAt = p.CreateAt,
                ProjectId = p.ProjectId == Guid.Empty ? null : p.ProjectId,
                RoomId = p.RoomId == Guid.Empty ? null : p.RoomId,
                UserId = string.IsNullOrWhiteSpace(p.UserId) ? null : p.UserId
            };
        }

        public async Task<List<PhotoDataDTO>> ListByProjectAsync(string projectId)
        {
            var guid = GuidValidator.ValidateGuid(projectId);

            var list = await _db.Photos.AsNoTracking().Where(x => x.ProjectId == guid).ToListAsync();

            return list.Select(p => new PhotoDataDTO
            {
                Id = p.Id.ToString(),
                FileName = p.FileName,
                ContentType = p.ContentType,
                Size = p.Size,
                Url = p.Url,
                StorageProvider = p.StorageProvider,
                CreatedAt = p.CreateAt,
                ProjectId = p.ProjectId == Guid.Empty ? null : p.ProjectId,
                RoomId = p.RoomId == Guid.Empty ? null : p.RoomId,
                UserId = string.IsNullOrWhiteSpace(p.UserId) ? null : p.UserId
            }).ToList();
        }

        public async Task<bool> DeleteAsync(string photoId)
        {
            var guid = GuidValidator.ValidateGuid(photoId);

            var p = await _db.Photos.FindAsync(guid);
            if (p == null) return false;

            if (string.Equals(p.StorageProvider, "local", StringComparison.OrdinalIgnoreCase))
            {
                var fileName = Path.GetFileName(p.Url);
                var fullPath = Path.Combine(_storageFolder, fileName);
                try
                {
                    if (File.Exists(fullPath)) File.Delete(fullPath);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed deleting file from disk");
                }
            }

            _db.Photos.Remove(p);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
