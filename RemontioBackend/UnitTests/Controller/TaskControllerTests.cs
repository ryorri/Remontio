using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Interfaces.ServiceInterfaces;
using Application.Objects.DTOs.TaskDTO;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Presentation.Controllers;

namespace UnitTests.Controller
{
    [TestFixture]
    public class TaskControllerTests
    {
        private Mock<ITaskService> _taskServiceMock = null!;
        private TaskController _controller = null!;

        [SetUp]
        public void Setup()
        {
            _taskServiceMock = new Mock<ITaskService>();
            _controller = new TaskController(_taskServiceMock.Object);
        }

        private CreateTaskDTO MakeCreateTaskDto() => new CreateTaskDTO
        {
            Name = "t",
            Status = StatusEnum.ToDo,
            Priority = PriorityEnum.Medium,
            CreateAt = DateTime.UtcNow,
            RoomId = Guid.NewGuid(),
            ProjectId = Guid.NewGuid(),
            UserId = "u"
        };

        private TaskDataDTO MakeTaskDataDto() => new TaskDataDTO
        {
            Id = "1",
            Name = "t",
            Status = StatusEnum.ToDo,
            Priority = PriorityEnum.Medium,
            CreateAt = DateTime.UtcNow,
            RoomId = Guid.NewGuid().ToString(),
            ProjectId = Guid.NewGuid().ToString(),
            UserId = "u"
        };

        [Test]
        public async Task CreateTask_ReturnsOk_OnSuccess()
        {
            _taskServiceMock.Setup(s => s.CreateTaskAsync(It.IsAny<CreateTaskDTO>())).ReturnsAsync(true);

            var result = await _controller.CreateTask(MakeCreateTaskDto());

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task CreateTask_ReturnsBadRequest_OnException()
        {
            _taskServiceMock.Setup(s => s.CreateTaskAsync(It.IsAny<CreateTaskDTO>())).ThrowsAsync(new Exception("fail"));

            var result = await _controller.CreateTask(MakeCreateTaskDto());

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
            var bad = result.Result as BadRequestObjectResult;
            var message = bad?.Value?.GetType().GetProperty("message")!.GetValue(bad.Value) as string;
            Assert.That(message, Is.EqualTo("fail"));
        }

        [Test]
        public async Task GetTaskById_ReturnsOk_OnSuccess()
        {
            var dto = MakeTaskDataDto();
            _taskServiceMock.Setup(s => s.GetTaskAsync("1")).ReturnsAsync(dto);

            var result = await _controller.GetTaskById("1");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.SameAs(dto));
        }

        [Test]
        public async Task GetTaskById_ReturnsBadRequest_OnException()
        {
            _taskServiceMock.Setup(s => s.GetTaskAsync(It.IsAny<string>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.GetTaskById("1");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetTaskList_ReturnsOk_OnSuccess()
        {
            var list = new List<TaskDataDTO> { MakeTaskDataDto() };
            _taskServiceMock.Setup(s => s.GetAllTasksAsync()).ReturnsAsync(list);

            var result = await _controller.GetTaskList();

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.SameAs(list));
        }

        [Test]
        public async Task GetTaskList_ReturnsBadRequest_OnException()
        {
            _taskServiceMock.Setup(s => s.GetAllTasksAsync()).ThrowsAsync(new Exception("boom"));

            var result = await _controller.GetTaskList();

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetTaskListByProjectId_ReturnsOk_OnSuccess()
        {
            var list = new List<TaskDataDTO> { MakeTaskDataDto() };
            _taskServiceMock.Setup(s => s.GetAllTasksByProjectIdAsync(It.IsAny<Guid>())).ReturnsAsync(list);

            var result = await _controller.GetTaskListByProjectId(Guid.NewGuid().ToString());

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.SameAs(list));
        }

        [Test]
        public async Task GetTaskListByProjectId_ReturnsBadRequest_OnInvalidGuid()
        {
            var result = await _controller.GetTaskListByProjectId("not-a-guid");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
            var bad = result.Result as BadRequestObjectResult;
            var message = bad?.Value?.GetType().GetProperty("message")!.GetValue(bad.Value) as string;
            Assert.That(message, Is.EqualTo("Invalid projectId format"));
        }

        [Test]
        public async Task GetTaskListByProjectId_ReturnsBadRequest_OnException()
        {
            _taskServiceMock.Setup(s => s.GetAllTasksByProjectIdAsync(It.IsAny<Guid>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.GetTaskListByProjectId(Guid.NewGuid().ToString());

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetTaskListByUserId_ReturnsOk_OnSuccess()
        {
            var list = new List<TaskDataDTO> { MakeTaskDataDto() };
            _taskServiceMock.Setup(s => s.GetAllTasksByUserIdAsync("u")).ReturnsAsync(list);

            var result = await _controller.GetTaskListByUserId("u");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.SameAs(list));
        }

        [Test]
        public async Task GetTaskListByUserId_ReturnsBadRequest_OnException()
        {
            _taskServiceMock.Setup(s => s.GetAllTasksByUserIdAsync(It.IsAny<string>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.GetTaskListByUserId("u");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task DeleteTask_ReturnsOk_OnSuccess()
        {
            _taskServiceMock.Setup(s => s.DeleteTaskAsync("1")).ReturnsAsync(true);

            var result = await _controller.DeleteTask("1");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task DeleteTask_ReturnsBadRequest_OnException()
        {
            _taskServiceMock.Setup(s => s.DeleteTaskAsync(It.IsAny<string>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.DeleteTask("1");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task EditTask_ReturnsOk_OnSuccess()
        {
            var dto = MakeTaskDataDto();
            _taskServiceMock.Setup(s => s.UpdateTaskAsync(dto)).ReturnsAsync(true);

            var result = await _controller.EditTask(dto);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task EditTask_ReturnsBadRequest_OnException()
        {
            _taskServiceMock.Setup(s => s.UpdateTaskAsync(It.IsAny<TaskDataDTO>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.EditTask(MakeTaskDataDto());

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task EditTaskStatus_ReturnsOk_OnSuccess()
        {
            _taskServiceMock.Setup(s => s.ChangeStatusAsync("1", "Open")).ReturnsAsync(true);

            var result = await _controller.EditTaskStatus("1", "Open");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task EditTaskStatus_ReturnsBadRequest_OnException()
        {
            _taskServiceMock.Setup(s => s.ChangeStatusAsync(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.EditTaskStatus("1", "Open");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task EditTaskPriority_ReturnsOk_OnSuccess()
        {
            _taskServiceMock.Setup(s => s.ChangePriorityAsync("1", "High")).ReturnsAsync(true);

            var result = await _controller.EditTaskPriority("1", "High");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task EditTaskPriority_ReturnsBadRequest_OnException()
        {
            _taskServiceMock.Setup(s => s.ChangePriorityAsync(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.EditTaskPriority("1", "High");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }
    }
}
