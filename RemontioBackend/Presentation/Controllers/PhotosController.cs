using Application.Interfaces.ServiceInterfaces;
using Application.Objects.DTOs.PhotoDTO;
using Application.Objects.DTOs.Photos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhotosController : ControllerBase
    {
        private readonly IPhotoService _photoService;

        public PhotosController(IPhotoService photoService)
        {
            _photoService = photoService;
        }

        [Authorize]
        [HttpPost("upload-photo")]
        public async Task<ActionResult<PhotoDataDTO>> UploadPhoto( CreatePhotoDTO dto)
        {
            try
            {
                var result = await _photoService.UploadAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("file")]
        public async Task<IActionResult> GetFile(string photoId)
        {
            try
            {
                var stream = await _photoService.GetFileStreamAsync(photoId);
                if (stream == null) return NotFound();

                var meta = await _photoService.GetMetadataAsync(photoId);
                var contentType = meta?.ContentType ?? "application/octet-stream";
                var fileName = meta?.FileName ?? "file";

                return File(stream, contentType, fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("metadata")]
        public async Task<ActionResult<PhotoDataDTO?>> GetMetadata(string photoId)
        {
            try
            {
                var result = await _photoService.GetMetadataAsync(photoId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("list-by-project")]
        public async Task<ActionResult<List<PhotoDataDTO>>> ListByProject(string projectId)
        {
            try
            {
                var result = await _photoService.ListByProjectAsync(projectId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("delete-photo")]
        public async Task<ActionResult<bool>> DeletePhoto(string photoId)
        {
            try
            {
                var result = await _photoService.DeleteAsync(photoId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
