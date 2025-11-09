using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Interfaces.ServiceInterfaces;
using Application.Objects.DTOs.ListDTO;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Presentation.Controllers;

namespace UnitTests.Controller
{
    [TestFixture]
    public class ListControllerTests
    {
        private Mock<IListService> _listServiceMock = null!;
        private ListController _controller = null!;

        [SetUp]
        public void Setup()
        {
            _listServiceMock = new Mock<IListService>();
            _controller = new ListController(_listServiceMock.Object);
        }

        [Test]
        public async Task CreateList_ReturnsOk_OnSuccess()
        {
            _listServiceMock.Setup(s => s.CreateListAsync(It.IsAny<CreateListDTO>())).ReturnsAsync(true);

            var dto = new CreateListDTO { Name = "l", CreateAt = DateTime.UtcNow, RoomId = Guid.NewGuid(), ProjectId = Guid.NewGuid(), UserId = "u" };
            var result = await _controller.CreateList(dto);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task CreateList_ReturnsBadRequest_OnException()
        {
            _listServiceMock.Setup(s => s.CreateListAsync(It.IsAny<CreateListDTO>())).ThrowsAsync(new Exception("fail"));

            var dto = new CreateListDTO { Name = "l", CreateAt = DateTime.UtcNow, RoomId = Guid.NewGuid(), ProjectId = Guid.NewGuid(), UserId = "u" };
            var result = await _controller.CreateList(dto);

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
            var bad = result.Result as BadRequestObjectResult;
            var message = bad?.Value?.GetType().GetProperty("message")!.GetValue(bad.Value) as string;
            Assert.That(message, Is.EqualTo("fail"));
        }

        [Test]
        public async Task GetListById_ReturnsOk_OnSuccess()
        {
            var list = new ListDataDTO { Id = "1", Name = "l", CreateAt = DateTime.UtcNow, RoomId = Guid.NewGuid().ToString(), ProjectId = Guid.NewGuid().ToString(), UserId = "u" };
            _listServiceMock.Setup(s => s.GetListAsync("1")).ReturnsAsync(list);

            var result = await _controller.GetListById("1");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.SameAs(list));
        }

        [Test]
        public async Task GetListById_ReturnsBadRequest_OnException()
        {
            _listServiceMock.Setup(s => s.GetListAsync(It.IsAny<string>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.GetListById("1");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetList_ReturnsOk_OnSuccess()
        {
            var list = new List<ListDataDTO> { new ListDataDTO { Id = "1", Name = "l", CreateAt = DateTime.UtcNow, RoomId = Guid.NewGuid().ToString(), ProjectId = Guid.NewGuid().ToString(), UserId = "u" } };
            _listServiceMock.Setup(s => s.GetAllListsAsync()).ReturnsAsync(list);

            var result = await _controller.GetList();

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.SameAs(list));
        }

        [Test]
        public async Task GetList_ReturnsBadRequest_OnException()
        {
            _listServiceMock.Setup(s => s.GetAllListsAsync()).ThrowsAsync(new Exception("boom"));

            var result = await _controller.GetList();

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetListByUserId_ReturnsOk_OnSuccess()
        {
            var list = new List<ListDataDTO> { new ListDataDTO { Id = "1", Name = "l", CreateAt = DateTime.UtcNow, RoomId = Guid.NewGuid().ToString(), ProjectId = Guid.NewGuid().ToString(), UserId = "u" } };
            _listServiceMock.Setup(s => s.GetAllListsByUserIdAsync("u")).ReturnsAsync(list);

            var result = await _controller.GetListByUserId("u");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.SameAs(list));
        }

        [Test]
        public async Task GetListByUserId_ReturnsBadRequest_OnException()
        {
            _listServiceMock.Setup(s => s.GetAllListsByUserIdAsync(It.IsAny<string>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.GetListByUserId("u");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetListByProjectId_ReturnsOk_OnSuccess()
        {
            var list = new List<ListDataDTO> { new ListDataDTO { Id = "1", Name = "l", CreateAt = DateTime.UtcNow, RoomId = Guid.NewGuid().ToString(), ProjectId = Guid.NewGuid().ToString(), UserId = "u" } };
            _listServiceMock.Setup(s => s.GetAllListsByProjectIdAsync("p")).ReturnsAsync(list);

            var result = await _controller.GetListByProjectId("p");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.SameAs(list));
        }

        [Test]
        public async Task GetListByProjectId_ReturnsBadRequest_OnException()
        {
            _listServiceMock.Setup(s => s.GetAllListsByProjectIdAsync(It.IsAny<string>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.GetListByProjectId("p");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetListByRoomId_ReturnsOk_OnSuccess()
        {
            var list = new List<ListDataDTO> { new ListDataDTO { Id = "1", Name = "l", CreateAt = DateTime.UtcNow, RoomId = Guid.NewGuid().ToString(), ProjectId = Guid.NewGuid().ToString(), UserId = "u" } };
            _listServiceMock.Setup(s => s.GetAllListsByRoomIdAsync("r")).ReturnsAsync(list);

            var result = await _controller.GetListByRoomId("r");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.SameAs(list));
        }

        [Test]
        public async Task GetListByRoomId_ReturnsBadRequest_OnException()
        {
            _listServiceMock.Setup(s => s.GetAllListsByRoomIdAsync(It.IsAny<string>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.GetListByRoomId("r");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task DeleteList_ReturnsOk_OnSuccess()
        {
            _listServiceMock.Setup(s => s.DeleteListAsync("1")).ReturnsAsync(true);

            var result = await _controller.DeleteList("1");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task DeleteList_ReturnsBadRequest_OnException()
        {
            _listServiceMock.Setup(s => s.DeleteListAsync(It.IsAny<string>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.DeleteList("1");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task EditList_ReturnsOk_OnSuccess()
        {
            var dto = new ListDataDTO { Id = "1", Name = "l", CreateAt = DateTime.UtcNow, RoomId = Guid.NewGuid().ToString(), ProjectId = Guid.NewGuid().ToString(), UserId = "u" };
            _listServiceMock.Setup(s => s.UpdateListAsync(dto)).ReturnsAsync(true);

            var result = await _controller.EditList(dto);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task EditList_ReturnsBadRequest_OnException()
        {
            _listServiceMock.Setup(s => s.UpdateListAsync(It.IsAny<ListDataDTO>())).ThrowsAsync(new Exception("err"));

            var dto = new ListDataDTO { Id = "1", Name = "l", CreateAt = DateTime.UtcNow, RoomId = Guid.NewGuid().ToString(), ProjectId = Guid.NewGuid().ToString(), UserId = "u" };
            var result = await _controller.EditList(dto);

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task AddItem_ReturnsOk_OnSuccess()
        {
            _listServiceMock.Setup(s => s.AddItemAsync("1", "item", 2, 5f)).ReturnsAsync(true);

            var result = await _controller.AddItem("1", "item", 2, 5f);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task AddItem_ReturnsBadRequest_OnException()
        {
            _listServiceMock.Setup(s => s.AddItemAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<float>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.AddItem("1", "item", 2, 5f);

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task RemoveItem_ReturnsOk_OnSuccess()
        {
            _listServiceMock.Setup(s => s.RemoveItemAsync("1", "i1")).ReturnsAsync(true);

            var result = await _controller.RemoveItem("1", "i1");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task RemoveItem_ReturnsBadRequest_OnException()
        {
            _listServiceMock.Setup(s => s.RemoveItemAsync(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.RemoveItem("1", "i1");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task MarkItemBought_ReturnsOk_OnSuccess()
        {
            _listServiceMock.Setup(s => s.MarkItemBoughtAsync("1", "i1", true)).ReturnsAsync(true);

            var result = await _controller.MarkItemBought("1", "i1", true);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task MarkItemBought_ReturnsBadRequest_OnException()
        {
            _listServiceMock.Setup(s => s.MarkItemBoughtAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.MarkItemBought("1", "i1", true);

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task ClearItems_ReturnsOk_OnSuccess()
        {
            _listServiceMock.Setup(s => s.ClearItemsAsync("1")).ReturnsAsync(true);

            var result = await _controller.ClearItems("1");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task ClearItems_ReturnsBadRequest_OnException()
        {
            _listServiceMock.Setup(s => s.ClearItemsAsync(It.IsAny<string>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.ClearItems("1");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }
    }
}
