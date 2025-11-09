using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Interfaces.ServiceInterfaces;
using Application.Objects.DTOs.AlertsDTO;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Presentation.Controllers;

namespace UnitTests.Controller
{
    [TestFixture]
    public class AlertControllerTests
    {
        private Mock<IAlertService> _alertServiceMock = null!;
        private AlertController _controller = null!;

        [SetUp]
        public void Setup()
        {
            _alertServiceMock = new Mock<IAlertService>();
            _controller = new AlertController(_alertServiceMock.Object);
        }

        [Test]
        public async Task CreateAlert_ReturnsOk_OnSuccess()
        {
            _alertServiceMock.Setup(s => s.CreateAlertAsync(It.IsAny<CreateAlertDTO>())).ReturnsAsync(true);

            var dto = new CreateAlertDTO { Message = "m", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(1), UserId = "u" };
            var result = await _controller.CreateAlert(dto);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task CreateAlert_ReturnsBadRequest_OnException()
        {
            _alertServiceMock.Setup(s => s.CreateAlertAsync(It.IsAny<CreateAlertDTO>())).ThrowsAsync(new Exception("fail"));

            var dto = new CreateAlertDTO { Message = "m", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(1), UserId = "u" };
            var result = await _controller.CreateAlert(dto);

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
            var bad = result.Result as BadRequestObjectResult;
            var message = bad?.Value?.GetType().GetProperty("message")!.GetValue(bad.Value) as string;
            Assert.That(message, Is.EqualTo("fail"));
        }

        [Test]
        public async Task GetAlertById_ReturnsOk_OnSuccess()
        {
            var alert = new AlertDataDTO { Id = "1" };
            _alertServiceMock.Setup(s => s.GetAlertByIdAsync("1")).ReturnsAsync(alert);

            var result = await _controller.GetAlertById("1");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.SameAs(alert));
        }

        [Test]
        public async Task GetAlertById_ReturnsBadRequest_OnException()
        {
            _alertServiceMock.Setup(s => s.GetAlertByIdAsync(It.IsAny<string>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.GetAlertById("1");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetAlertList_ReturnsOk_OnSuccess()
        {
            var list = new List<AlertDataDTO> { new AlertDataDTO { Id = "1" } };
            _alertServiceMock.Setup(s => s.GetAllAlertsAsync()).ReturnsAsync(list);

            var result = await _controller.GetAlertList();

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.SameAs(list));
        }

        [Test]
        public async Task GetAlertList_ReturnsBadRequest_OnException()
        {
            _alertServiceMock.Setup(s => s.GetAllAlertsAsync()).ThrowsAsync(new Exception("boom"));

            var result = await _controller.GetAlertList();

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetAlertListByUserId_ReturnsOk_OnSuccess()
        {
            var list = new List<AlertDataDTO> { new AlertDataDTO { Id = "1" } };
            _alertServiceMock.Setup(s => s.GetAlertsByUserIdAsync("u")).ReturnsAsync(list);

            var result = await _controller.GetAlertListByUserId("u");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.SameAs(list));
        }

        [Test]
        public async Task GetAlertListByUserId_ReturnsBadRequest_OnException()
        {
            _alertServiceMock.Setup(s => s.GetAlertsByUserIdAsync(It.IsAny<string>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.GetAlertListByUserId("u");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task DeleteAlert_ReturnsOk_OnSuccess()
        {
            _alertServiceMock.Setup(s => s.DeleteAlertAsync("1")).ReturnsAsync(true);

            var result = await _controller.DeleteAlert("1");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task DeleteAlert_ReturnsBadRequest_OnException()
        {
            _alertServiceMock.Setup(s => s.DeleteAlertAsync(It.IsAny<string>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.DeleteAlert("1");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task EditAlert_ReturnsOk_OnSuccess()
        {
            var dto = new AlertDataDTO { Id = "1" };
            _alertServiceMock.Setup(s => s.UpdateAlertAsync(dto)).ReturnsAsync(true);

            var result = await _controller.EditAlert(dto);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task EditAlert_ReturnsBadRequest_OnException()
        {
            _alertServiceMock.Setup(s => s.UpdateAlertAsync(It.IsAny<AlertDataDTO>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.EditAlert(new AlertDataDTO { Id = "1" });

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task EditAlertStatus_ReturnsOk_OnSuccess()
        {
            _alertServiceMock.Setup(s => s.ChangeStatusAsync("1", "Open")).ReturnsAsync(true);

            var result = await _controller.EditAlertStatus("1", "Open");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task EditAlertStatus_ReturnsBadRequest_OnException()
        {
            _alertServiceMock.Setup(s => s.ChangeStatusAsync(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.EditAlertStatus("1", "Open");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task EditAlertPriority_ReturnsOk_OnSuccess()
        {
            _alertServiceMock.Setup(s => s.ChangePriorityAsync("1", "High")).ReturnsAsync(true);

            var result = await _controller.EditAlertPriority("1", "High");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task EditAlertPriority_ReturnsBadRequest_OnException()
        {
            _alertServiceMock.Setup(s => s.ChangePriorityAsync(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.EditAlertPriority("1", "High");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }
    }
}
