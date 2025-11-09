using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Interfaces.AdditionalInterfaces;
using Application.Interfaces.ServiceInterfaces;
using Application.Objects.DTOs.RoomDTO;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Presentation.Controllers;

namespace UnitTests.Controller
{
    [TestFixture]
    public class RoomControllerTests
    {
        private Mock<IRoomService> _roomServiceMock = null!;
        private RoomController _controller = null!;

        [SetUp]
        public void Setup()
        {
            _roomServiceMock = new Mock<IRoomService>();
            _controller = new RoomController(_roomServiceMock.Object);
        }

        private CreateRoomDTO MakeCreateRoomDto() => new CreateRoomDTO
        {
            Name = "room",
            CreatedAt = DateTime.UtcNow,
            Status = StatusEnum.ToDo,
            UserId = Guid.NewGuid()
        };

        private RoomDataDTO MakeRoomDataDto() => new RoomDataDTO
        {
            Id = "1",
            Name = "room",
            CreateAt = DateTime.UtcNow,
            Status = StatusEnum.ToDo,
            ProjectId = Guid.NewGuid().ToString(),
            UserId = Guid.NewGuid().ToString()
        };

        [Test]
        public async Task CreateRoom_ReturnsOk_OnSuccess()
        {
            _roomServiceMock.Setup(s => s.CreateRoomAsync(It.IsAny<CreateRoomDTO>(), "proj1")).ReturnsAsync(true);

            var result = await _controller.CreateRoom(MakeCreateRoomDto(), "proj1");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task CreateRoom_ReturnsBadRequest_OnException()
        {
            _roomServiceMock.Setup(s => s.CreateRoomAsync(It.IsAny<CreateRoomDTO>(), It.IsAny<string>())).ThrowsAsync(new Exception("fail"));

            var result = await _controller.CreateRoom(MakeCreateRoomDto(), "proj1");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
            var bad = result.Result as BadRequestObjectResult;
            var message = bad?.Value?.GetType().GetProperty("message")!.GetValue(bad.Value) as string;
            Assert.That(message, Is.EqualTo("fail"));
        }

        [Test]
        public async Task GetRoomById_ReturnsOk_OnSuccess()
        {
            var room = MakeRoomDataDto();
            _roomServiceMock.Setup(s => s.GetRoomAsync("1")).ReturnsAsync(room);

            var result = await _controller.GetRoomById("1");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.SameAs(room));
        }

        [Test]
        public async Task GetRoomById_ReturnsBadRequest_OnException()
        {
            _roomServiceMock.Setup(s => s.GetRoomAsync(It.IsAny<string>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.GetRoomById("1");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetRoomList_ReturnsOk_OnSuccess()
        {
            var list = new List<RoomDataDTO> { MakeRoomDataDto() };
            _roomServiceMock.Setup(s => s.GetAllRoomsAsync()).ReturnsAsync(list);

            var result = await _controller.GetRoomList();

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.SameAs(list));
        }

        [Test]
        public async Task GetRoomList_ReturnsBadRequest_OnException()
        {
            _roomServiceMock.Setup(s => s.GetAllRoomsAsync()).ThrowsAsync(new Exception("boom"));

            var result = await _controller.GetRoomList();

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetRoomListByUserId_ReturnsOk_OnSuccess()
        {
            var list = new List<RoomDataDTO> { MakeRoomDataDto() };
            _roomServiceMock.Setup(s => s.GetAllRoomsByUserIdAsync("u")).ReturnsAsync(list);

            var result = await _controller.GetRoomListByUserId("u");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.SameAs(list));
        }

        [Test]
        public async Task GetRoomListByUserId_ReturnsBadRequest_OnException()
        {
            _roomServiceMock.Setup(s => s.GetAllRoomsByUserIdAsync(It.IsAny<string>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.GetRoomListByUserId("u");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetRoomListByProjectId_ReturnsOk_OnSuccess()
        {
            var list = new List<RoomDataDTO> { MakeRoomDataDto() };
            _roomServiceMock.Setup(s => s.GetAllRoomsByProjectIdAsync("p")).ReturnsAsync(list);

            var result = await _controller.GetRoomListByProjectId("p");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.SameAs(list));
        }

        [Test]
        public async Task GetRoomListByProjectId_ReturnsBadRequest_OnException()
        {
            _roomServiceMock.Setup(s => s.GetAllRoomsByProjectIdAsync(It.IsAny<string>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.GetRoomListByProjectId("p");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task DeleteRoom_ReturnsOk_OnSuccess()
        {
            _roomServiceMock.Setup(s => s.DeleteRoomAsync("1")).ReturnsAsync(true);

            var result = await _controller.DeleteRoom("1");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task DeleteRoom_ReturnsBadRequest_OnException()
        {
            _roomServiceMock.Setup(s => s.DeleteRoomAsync(It.IsAny<string>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.DeleteRoom("1");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task EditRoom_ReturnsOk_OnSuccess()
        {
            var dto = MakeRoomDataDto();
            _roomServiceMock.Setup(s => s.UpdateRoomAsync(dto)).ReturnsAsync(true);

            var result = await _controller.EditRoom(dto);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task EditRoom_ReturnsBadRequest_OnException()
        {
            _roomServiceMock.Setup(s => s.UpdateRoomAsync(It.IsAny<RoomDataDTO>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.EditRoom(MakeRoomDataDto());

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task EditRoomStatus_ReturnsOk_OnSuccess()
        {
            _roomServiceMock.Setup(s => s.ChangeRoomStatusAsync("1", "Open")).ReturnsAsync(true);

            var result = await _controller.EditRoomStatus("1", "Open");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task EditRoomStatus_ReturnsBadRequest_OnException()
        {
            _roomServiceMock.Setup(s => s.ChangeRoomStatusAsync(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.EditRoomStatus("1", "Open");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        private class FakePoint : IPoint
        {
            public float X { get; set; }
            public float Y { get; set; }
        }

        [Test]
        public async Task AddWall_ReturnsOk_OnSuccess()
        {
            _roomServiceMock.Setup(s => s.AddWallAsync("1", It.IsAny<List<IPoint>>(), "wall")).ReturnsAsync(true);

            var result = await _controller.AddWall("1", new List<IPoint> { new FakePoint { X = 1, Y = 2 } }, "wall");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task AddWall_ReturnsBadRequest_OnException()
        {
            _roomServiceMock.Setup(s => s.AddWallAsync(It.IsAny<string>(), It.IsAny<List<IPoint>>(), It.IsAny<string>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.AddWall("1", new List<IPoint> { new FakePoint { X = 1, Y = 2 } }, "wall");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task RemoveWall_ReturnsOk_OnSuccess()
        {
            _roomServiceMock.Setup(s => s.RemoveWallFromRoomAsync("1", "w1")).ReturnsAsync(true);

            var result = await _controller.RemoveWall("1", "w1");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task RemoveWall_ReturnsBadRequest_OnException()
        {
            _roomServiceMock.Setup(s => s.RemoveWallFromRoomAsync(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.RemoveWall("1", "w1");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task RemoveAllWalls_ReturnsOk_OnSuccess()
        {
            _roomServiceMock.Setup(s => s.RemoveAllWallFromRoomAsync("1")).ReturnsAsync(true);

            var result = await _controller.RemoveAllWalls("1");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task RemoveAllWalls_ReturnsBadRequest_OnException()
        {
            _roomServiceMock.Setup(s => s.RemoveAllWallFromRoomAsync(It.IsAny<string>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.RemoveAllWalls("1");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task AddFloor_ReturnsOk_OnSuccess()
        {
            _roomServiceMock.Setup(s => s.AddFloorAsync("1", It.IsAny<List<IPoint>>(), "floor")).ReturnsAsync(true);

            var result = await _controller.AddFloor("1", new List<IPoint> { new FakePoint { X = 3, Y = 4 } }, "floor");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task AddFloor_ReturnsBadRequest_OnException()
        {
            _roomServiceMock.Setup(s => s.AddFloorAsync(It.IsAny<string>(), It.IsAny<List<IPoint>>(), It.IsAny<string>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.AddFloor("1", new List<IPoint> { new FakePoint { X = 3, Y = 4 } }, "floor");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task RemoveFloor_ReturnsOk_OnSuccess()
        {
            _roomServiceMock.Setup(s => s.RemoveFloorFromRoomAsync("1", "f1")).ReturnsAsync(true);

            var result = await _controller.RemoveFloor("1", "f1");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task RemoveFloor_ReturnsBadRequest_OnException()
        {
            _roomServiceMock.Setup(s => s.RemoveFloorFromRoomAsync(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.RemoveFloor("1", "f1");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }
    }
}
