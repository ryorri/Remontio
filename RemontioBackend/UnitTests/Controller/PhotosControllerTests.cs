using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.ServiceInterfaces;
using Application.Objects.DTOs.PhotoDTO;
using Application.Objects.DTOs.Photos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Presentation.Controllers;

namespace UnitTests.Controller
{
    [TestFixture]
    public class PhotosControllerTests
    {
        private Mock<IPhotoService> _photoServiceMock = null!;
        private PhotosController _controller = null!;

        [SetUp]
        public void Setup()
        {
            _photoServiceMock = new Mock<IPhotoService>();
            _controller = new PhotosController(_photoServiceMock.Object);
        }

        private IFormFile CreateFakeFormFile(string fileName = "test.jpg", string contentType = "image/jpeg")
        {
            var bytes = Encoding.UTF8.GetBytes("fake image content");
            var stream = new MemoryStream(bytes);
            return new FormFile(stream, 0, bytes.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };
        }

        [Test]
        public async Task UploadPhoto_ReturnsOk_OnSuccess()
        {
            var photo = new PhotoDataDTO { Id = "1", FileName = "test.jpg", ContentType = "image/jpeg", Size = 10, Url = "url", StorageProvider = "local", CreatedAt = DateTime.UtcNow };
            _photoServiceMock.Setup(s => s.UploadAsync(It.IsAny<CreatePhotoDTO>())).ReturnsAsync(photo);

            var dto = new CreatePhotoDTO { File = CreateFakeFormFile(), UserId = "u" };
            var result = await _controller.UploadPhoto(dto);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.SameAs(photo));
        }

        [Test]
        public async Task UploadPhoto_ReturnsBadRequest_OnException()
        {
            _photoServiceMock.Setup(s => s.UploadAsync(It.IsAny<CreatePhotoDTO>())).ThrowsAsync(new Exception("fail"));

            var dto = new CreatePhotoDTO { File = CreateFakeFormFile(), UserId = "u" };
            var result = await _controller.UploadPhoto(dto);

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
            var bad = result.Result as BadRequestObjectResult;
            var message = bad?.Value?.GetType().GetProperty("message")!.GetValue(bad.Value) as string;
            Assert.That(message, Is.EqualTo("fail"));
        }

        [Test]
        public async Task GetFile_ReturnsFile_OnSuccess()
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("data"));
            var meta = new PhotoDataDTO { Id = "1", FileName = "file.txt", ContentType = "text/plain", Size = 4, Url = "url", StorageProvider = "local", CreatedAt = DateTime.UtcNow };
            _photoServiceMock.Setup(s => s.GetFileStreamAsync("1")).ReturnsAsync(stream);
            _photoServiceMock.Setup(s => s.GetMetadataAsync("1")).ReturnsAsync(meta);

            var result = await _controller.GetFile("1");

            Assert.That(result, Is.InstanceOf<FileStreamResult>());
            var fileResult = result as FileStreamResult;
            Assert.That(fileResult!.ContentType, Is.EqualTo("text/plain"));
            Assert.That(fileResult.FileDownloadName, Is.EqualTo("file.txt"));
        }

        [Test]
        public async Task GetFile_ReturnsNotFound_WhenStreamNull()
        {
            _photoServiceMock.Setup(s => s.GetFileStreamAsync("1")).ReturnsAsync((Stream?)null);

            var result = await _controller.GetFile("1");

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task GetFile_ReturnsBadRequest_OnException()
        {
            _photoServiceMock.Setup(s => s.GetFileStreamAsync(It.IsAny<string>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.GetFile("1");

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetMetadata_ReturnsOk_OnSuccess()
        {
            var meta = new PhotoDataDTO { Id = "1", FileName = "test.jpg", ContentType = "image/jpeg", Size = 10, Url = "url", StorageProvider = "local", CreatedAt = DateTime.UtcNow };
            _photoServiceMock.Setup(s => s.GetMetadataAsync("1")).ReturnsAsync(meta);

            var result = await _controller.GetMetadata("1");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.SameAs(meta));
        }

        [Test]
        public async Task GetMetadata_ReturnsBadRequest_OnException()
        {
            _photoServiceMock.Setup(s => s.GetMetadataAsync(It.IsAny<string>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.GetMetadata("1");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task ListByProject_ReturnsOk_OnSuccess()
        {
            var list = new List<PhotoDataDTO> { new PhotoDataDTO { Id = "1", FileName = "a", ContentType = "image/jpeg", Size = 1, Url = "url", StorageProvider = "local", CreatedAt = DateTime.UtcNow } };
            _photoServiceMock.Setup(s => s.ListByProjectAsync("p")).ReturnsAsync(list);

            var result = await _controller.ListByProject("p");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.SameAs(list));
        }

        [Test]
        public async Task ListByProject_ReturnsBadRequest_OnException()
        {
            _photoServiceMock.Setup(s => s.ListByProjectAsync(It.IsAny<string>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.ListByProject("p");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task DeletePhoto_ReturnsOk_OnSuccess()
        {
            _photoServiceMock.Setup(s => s.DeleteAsync("1")).ReturnsAsync(true);

            var result = await _controller.DeletePhoto("1");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task DeletePhoto_ReturnsBadRequest_OnException()
        {
            _photoServiceMock.Setup(s => s.DeleteAsync(It.IsAny<string>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.DeletePhoto("1");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }
    }
}
