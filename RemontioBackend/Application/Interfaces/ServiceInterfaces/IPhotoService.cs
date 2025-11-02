using Application.Objects.DTOs.PhotoDTO;
using Application.Objects.DTOs.Photos;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Application.Interfaces.ServiceInterfaces
{
    public interface IPhotoService
    {
        Task<PhotoDataDTO> UploadAsync(CreatePhotoDTO dto);
        Task<Stream?> GetFileStreamAsync(string photoId);
        Task<PhotoDataDTO?> GetMetadataAsync(string photoId);
        Task<List<PhotoDataDTO>> ListByProjectAsync(string projectId);
        Task<bool> DeleteAsync(string photoId);
    }
}
