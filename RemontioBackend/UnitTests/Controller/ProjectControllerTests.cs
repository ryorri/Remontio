using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Interfaces.ServiceInterfaces;
using Application.Objects.DTOs.ProjectDTO;
using Application.Objects.DTOs.UserDTO;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Presentation.Controllers;

namespace UnitTests.Controller
{
    [TestFixture]
    public class ProjectControllerTests
    {
        private Mock<IProjectService> _projectServiceMock = null!;
        private ProjectController _controller = null!;

        [SetUp]
        public void Setup()
        {
            _projectServiceMock = new Mock<IProjectService>();
            _controller = new ProjectController(_projectServiceMock.Object);
        }

        [Test]
        public async Task CreateProject_ReturnsOk_OnSuccess()
        {
            _projectServiceMock.Setup(s => s.CreateProjectAsync(It.IsAny<CreateProjectDTO>())).ReturnsAsync(true);

            var dto = new CreateProjectDTO { Name = "p", CreatedAt = DateTime.UtcNow, UserId = "u" };
            var result = await _controller.CreateProject(dto);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task CreateProject_ReturnsBadRequest_OnException()
        {
            _projectServiceMock.Setup(s => s.CreateProjectAsync(It.IsAny<CreateProjectDTO>())).ThrowsAsync(new Exception("fail"));

            var dto = new CreateProjectDTO { Name = "p", CreatedAt = DateTime.UtcNow, UserId = "u" };
            var result = await _controller.CreateProject(dto);

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
            var bad = result.Result as BadRequestObjectResult;
            var message = bad?.Value?.GetType().GetProperty("message")!.GetValue(bad.Value) as string;
            Assert.That(message, Is.EqualTo("fail"));
        }

        [Test]
        public async Task GetProjectById_ReturnsOk_OnSuccess()
        {
            var project = new ProjectDataDTO { Id = "1", Name = "proj", CreateAt = DateTime.UtcNow, User = new UserDataDTO { Id = "u" } };
            _projectServiceMock.Setup(s => s.GetProjectAsync("1")).ReturnsAsync(project);

            var result = await _controller.GetProjectById("1");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.SameAs(project));
        }

        [Test]
        public async Task GetProjectById_ReturnsBadRequest_OnException()
        {
            _projectServiceMock.Setup(s => s.GetProjectAsync(It.IsAny<string>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.GetProjectById("1");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetProjectList_ReturnsOk_OnSuccess()
        {
            var list = new List<ProjectDataDTO> { new ProjectDataDTO { Id = "1", Name = "p", CreateAt = DateTime.UtcNow, User = new UserDataDTO { Id = "u" } } };
            _projectServiceMock.Setup(s => s.GetAllProjectsAsync()).ReturnsAsync(list);

            var result = await _controller.GetProjectList();

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.SameAs(list));
        }

        [Test]
        public async Task GetProjectList_ReturnsBadRequest_OnException()
        {
            _projectServiceMock.Setup(s => s.GetAllProjectsAsync()).ThrowsAsync(new Exception("boom"));

            var result = await _controller.GetProjectList();

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetProjectListByUserId_ReturnsOk_OnSuccess()
        {
            var list = new List<ProjectDataDTO> { new ProjectDataDTO { Id = "1", Name = "p", CreateAt = DateTime.UtcNow, User = new UserDataDTO { Id = "u" } } };
            _projectServiceMock.Setup(s => s.GetAllProjectsByUserIdAsync("u")).ReturnsAsync(list);

            var result = await _controller.GetProjectListByUserId("u");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.SameAs(list));
        }

        [Test]
        public async Task GetProjectListByUserId_ReturnsBadRequest_OnException()
        {
            _projectServiceMock.Setup(s => s.GetAllProjectsByUserIdAsync(It.IsAny<string>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.GetProjectListByUserId("u");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task DeleteProject_ReturnsOk_OnSuccess()
        {
            _projectServiceMock.Setup(s => s.DeleteProjectAsync("1")).ReturnsAsync(true);

            var result = await _controller.DeleteProject("1");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task DeleteProject_ReturnsBadRequest_OnException()
        {
            _projectServiceMock.Setup(s => s.DeleteProjectAsync(It.IsAny<string>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.DeleteProject("1");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task EditProject_ReturnsOk_OnSuccess()
        {
            var project = new ProjectDataDTO { Id = "1", Name = "p", CreateAt = DateTime.UtcNow, User = new UserDataDTO { Id = "u" } };
            _projectServiceMock.Setup(s => s.UpdateProjectAsync(project)).ReturnsAsync(true);

            var result = await _controller.EditProject(project);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task EditProject_ReturnsBadRequest_OnException()
        {
            _projectServiceMock.Setup(s => s.UpdateProjectAsync(It.IsAny<ProjectDataDTO>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.EditProject(new ProjectDataDTO { Id = "1", Name = "p", CreateAt = DateTime.UtcNow, User = new UserDataDTO { Id = "u" } });

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task EditProjectStatus_ReturnsOk_OnSuccess()
        {
            _projectServiceMock.Setup(s => s.ChangeStatusAsync("1", "Open")).ReturnsAsync(true);

            var result = await _controller.EditProjectStatus("1", "Open");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task EditProjectStatus_ReturnsBadRequest_OnException()
        {
            _projectServiceMock.Setup(s => s.ChangeStatusAsync(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.EditProjectStatus("1", "Open");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }
    }
}
