using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Interfaces.ServiceInterfaces;
using Application.Objects.DTOs.BudgetDTO;
using Application.Objects.DTOs.BudgetItemDTO;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Presentation.Controllers;

namespace UnitTests.Controller
{
    [TestFixture]
    public class BudgetControllerTests
    {
        private Mock<IBudgetService> _budgetServiceMock = null!;
        private BudgetController _controller = null!;

        [SetUp]
        public void Setup()
        {
            _budgetServiceMock = new Mock<IBudgetService>();
            _controller = new BudgetController(_budgetServiceMock.Object);
        }

        [Test]
        public async Task CreateBudget_ReturnsOk_OnSuccess()
        {
            _budgetServiceMock.Setup(s => s.CreateBudgetAsync(It.IsAny<CreateBudgetDTO>())).ReturnsAsync(true);

            var dto = new CreateBudgetDTO
            {
                Name = "b",
                CreateAt = DateTime.UtcNow,
                RoomId = Guid.NewGuid(),
                ProjectId = Guid.NewGuid(),
                UserId = "u"
            };

            var result = await _controller.CreateBudget(dto);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task CreateBudget_ReturnsBadRequest_OnException()
        {
            _budgetServiceMock.Setup(s => s.CreateBudgetAsync(It.IsAny<CreateBudgetDTO>())).ThrowsAsync(new Exception("fail"));

            var dto = new CreateBudgetDTO
            {
                Name = "b",
                CreateAt = DateTime.UtcNow,
                RoomId = Guid.NewGuid(),
                ProjectId = Guid.NewGuid(),
                UserId = "u"
            };

            var result = await _controller.CreateBudget(dto);

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
            var bad = result.Result as BadRequestObjectResult;
            var message = bad?.Value?.GetType().GetProperty("message")!.GetValue(bad.Value) as string;
            Assert.That(message, Is.EqualTo("fail"));
        }

        [Test]
        public async Task GetBudgetById_ReturnsOk_OnSuccess()
        {
            var budget = new BudgetDataDTO
            {
                Id = "1",
                Name = "b",
                CreateAt = DateTime.UtcNow,
                RoomId = Guid.NewGuid().ToString(),
                ProjectId = Guid.NewGuid().ToString(),
                UserId = "u"
            };
            _budgetServiceMock.Setup(s => s.GetBudgetAsync("1")).ReturnsAsync(budget);

            var result = await _controller.GetBudgetById("1");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.SameAs(budget));
        }

        [Test]
        public async Task GetBudgetById_ReturnsBadRequest_OnException()
        {
            _budgetServiceMock.Setup(s => s.GetBudgetAsync(It.IsAny<string>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.GetBudgetById("1");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetBudgetList_ReturnsOk_OnSuccess()
        {
            var list = new List<BudgetDataDTO>
            {
                new BudgetDataDTO
                {
                    Id = "1", Name = "b", CreateAt = DateTime.UtcNow,
                    RoomId = Guid.NewGuid().ToString(), ProjectId = Guid.NewGuid().ToString(), UserId = "u"
                }
            };
            _budgetServiceMock.Setup(s => s.GetAllBudgetsAsync()).ReturnsAsync(list);

            var result = await _controller.GetBudgetList();

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.SameAs(list));
        }

        [Test]
        public async Task GetBudgetList_ReturnsBadRequest_OnException()
        {
            _budgetServiceMock.Setup(s => s.GetAllBudgetsAsync()).ThrowsAsync(new Exception("boom"));

            var result = await _controller.GetBudgetList();

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetBudgetListByUserId_ReturnsOk_OnSuccess()
        {
            var list = new List<BudgetDataDTO>
            {
                new BudgetDataDTO
                {
                    Id = "1", Name = "b", CreateAt = DateTime.UtcNow,
                    RoomId = Guid.NewGuid().ToString(), ProjectId = Guid.NewGuid().ToString(), UserId = "u"
                }
            };
            _budgetServiceMock.Setup(s => s.GetAllBudgetsByUserIdAsync("u")).ReturnsAsync(list);

            var result = await _controller.GetBudgetListByUserId("u");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.SameAs(list));
        }

        [Test]
        public async Task GetBudgetListByUserId_ReturnsBadRequest_OnException()
        {
            _budgetServiceMock.Setup(s => s.GetAllBudgetsByUserIdAsync(It.IsAny<string>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.GetBudgetListByUserId("u");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetBudgetListByProjectId_ReturnsOk_OnSuccess()
        {
            var list = new List<BudgetDataDTO>
            {
                new BudgetDataDTO
                {
                    Id = "1", Name = "b", CreateAt = DateTime.UtcNow,
                    RoomId = Guid.NewGuid().ToString(), ProjectId = Guid.NewGuid().ToString(), UserId = "u"
                }
            };
            _budgetServiceMock.Setup(s => s.GetAllBudgetsByProjectIdAsync("p")).ReturnsAsync(list);

            var result = await _controller.GetBudgetListByProjectId("p");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.SameAs(list));
        }

        [Test]
        public async Task GetBudgetListByProjectId_ReturnsBadRequest_OnException()
        {
            _budgetServiceMock.Setup(s => s.GetAllBudgetsByProjectIdAsync(It.IsAny<string>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.GetBudgetListByProjectId("p");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetBudgetListByRoomId_ReturnsOk_OnSuccess()
        {
            var list = new List<BudgetDataDTO>
            {
                new BudgetDataDTO
                {
                    Id = "1", Name = "b", CreateAt = DateTime.UtcNow,
                    RoomId = Guid.NewGuid().ToString(), ProjectId = Guid.NewGuid().ToString(), UserId = "u"
                }
            };
            _budgetServiceMock.Setup(s => s.GetAllBudgetsByRoomIdAsync("r")).ReturnsAsync(list);

            var result = await _controller.GetBudgetListByRoomId("r");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.SameAs(list));
        }

        [Test]
        public async Task GetBudgetListByRoomId_ReturnsBadRequest_OnException()
        {
            _budgetServiceMock.Setup(s => s.GetAllBudgetsByRoomIdAsync(It.IsAny<string>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.GetBudgetListByRoomId("r");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task DeleteBudget_ReturnsOk_OnSuccess()
        {
            _budgetServiceMock.Setup(s => s.DeleteBudgetAsync("1")).ReturnsAsync(true);

            var result = await _controller.DeleteBudget("1");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task DeleteBudget_ReturnsBadRequest_OnException()
        {
            _budgetServiceMock.Setup(s => s.DeleteBudgetAsync(It.IsAny<string>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.DeleteBudget("1");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task EditBudget_ReturnsOk_OnSuccess()
        {
            var dto = new BudgetDataDTO
            {
                Id = "1",
                Name = "b",
                CreateAt = DateTime.UtcNow,
                RoomId = Guid.NewGuid().ToString(),
                ProjectId = Guid.NewGuid().ToString(),
                UserId = "u"
            };
            _budgetServiceMock.Setup(s => s.UpdateBudgetAsync(dto)).ReturnsAsync(true);

            var result = await _controller.EditBudget(dto);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task EditBudget_ReturnsBadRequest_OnException()
        {
            _budgetServiceMock.Setup(s => s.UpdateBudgetAsync(It.IsAny<BudgetDataDTO>())).ThrowsAsync(new Exception("err"));

            var dto = new BudgetDataDTO
            {
                Id = "1",
                Name = "b",
                CreateAt = DateTime.UtcNow,
                RoomId = Guid.NewGuid().ToString(),
                ProjectId = Guid.NewGuid().ToString(),
                UserId = "u"
            };
            var result = await _controller.EditBudget(dto);

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetBudgetItems_ReturnsOk_OnSuccess()
        {
            var items = new List<BudgetItemDataDTO>
            {
                new BudgetItemDataDTO { Id = "i1", Name = "n1", Price = 10f, Total = 10f, EstimatetPrice = 12f, IsCompleted = false }
            };
            _budgetServiceMock.Setup(s => s.GetBudgetItemsAsync("1")).ReturnsAsync(items);

            var result = await _controller.GetBudgetItems("1");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.SameAs(items));
        }

        [Test]
        public async Task GetBudgetItems_ReturnsBadRequest_OnException()
        {
            _budgetServiceMock.Setup(s => s.GetBudgetItemsAsync(It.IsAny<string>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.GetBudgetItems("1");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task AddItem_ReturnsOk_OnSuccess()
        {
            _budgetServiceMock
                .Setup(s => s.AddItemAsync("1", "name", 10f, 10f, 12f))
                .ReturnsAsync(true);

            var result = await _controller.AddItem("1", "name", 10f, 10f, 12f);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task AddItem_ReturnsBadRequest_OnException()
        {
            _budgetServiceMock
                .Setup(s => s.AddItemAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<float>(), It.IsAny<float>(), It.IsAny<float>()))
                .ThrowsAsync(new Exception("err"));

            var result = await _controller.AddItem("1", "name", 10f, 10f, 12f);

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task RemoveItem_ReturnsOk_OnSuccess()
        {
            _budgetServiceMock.Setup(s => s.RemoveItemAsync("1", "i1")).ReturnsAsync(true);

            var result = await _controller.RemoveItem("1", "i1");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task RemoveItem_ReturnsBadRequest_OnException()
        {
            _budgetServiceMock.Setup(s => s.RemoveItemAsync(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.RemoveItem("1", "i1");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task MarkItemCompleted_ReturnsOk_OnSuccess()
        {
            _budgetServiceMock.Setup(s => s.MarkItemCompletedAsync("1", "i1", true)).ReturnsAsync(true);

            var result = await _controller.MarkItemCompleted("1", "i1", true);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task MarkItemCompleted_ReturnsBadRequest_OnException()
        {
            _budgetServiceMock.Setup(s => s.MarkItemCompletedAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.MarkItemCompleted("1", "i1", true);

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task ClearItems_ReturnsOk_OnSuccess()
        {
            _budgetServiceMock.Setup(s => s.ClearItemsAsync("1")).ReturnsAsync(true);

            var result = await _controller.ClearItems("1");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task ClearItems_ReturnsBadRequest_OnException()
        {
            _budgetServiceMock.Setup(s => s.ClearItemsAsync(It.IsAny<string>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.ClearItems("1");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task Recalculate_ReturnsOk_OnSuccess()
        {
            _budgetServiceMock.Setup(s => s.RecalculateBudgetAsync("1")).ReturnsAsync(true);

            var result = await _controller.Recalculate("1");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task Recalculate_ReturnsBadRequest_OnException()
        {
            _budgetServiceMock.Setup(s => s.RecalculateBudgetAsync(It.IsAny<string>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.Recalculate("1");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task AddShoppingListAsItem_ReturnsOk_OnSuccess()
        {
            _budgetServiceMock.Setup(s => s.AddShoppingListAsItemAsync("1", "s1", true)).ReturnsAsync(true);

            var result = await _controller.AddShoppingListAsItem("1", "s1", true);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task AddShoppingListAsItem_ReturnsBadRequest_OnException()
        {
            _budgetServiceMock.Setup(s => s.AddShoppingListAsItemAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.AddShoppingListAsItem("1", "s1", true);

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }
    }
}
