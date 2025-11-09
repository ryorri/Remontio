using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Interfaces.ServiceInterfaces;
using Application.Objects.DTOs.ContactsDTO;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Presentation.Controllers;

namespace UnitTests.Controller
{
    [TestFixture]
    public class ContactControllerTests
    {
        private Mock<IContactService> _contactServiceMock = null!;
        private ContactController _controller = null!;

        [SetUp]
        public void Setup()
        {
            _contactServiceMock = new Mock<IContactService>();
            _controller = new ContactController(_contactServiceMock.Object);
        }

        [Test]
        public async Task CreateContact_ReturnsOk_OnSuccess()
        {
            _contactServiceMock.Setup(s => s.CreateContactAsync(It.IsAny<CreateContactDTO>())).ReturnsAsync(true);

            var dto = new CreateContactDTO { Name = "c", ContactDetails = "123", UserId = "u" };
            var result = await _controller.CreateContact(dto);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task CreateContact_ReturnsBadRequest_OnException()
        {
            _contactServiceMock.Setup(s => s.CreateContactAsync(It.IsAny<CreateContactDTO>())).ThrowsAsync(new Exception("fail"));

            var dto = new CreateContactDTO { Name = "c", ContactDetails = "123", UserId = "u" };
            var result = await _controller.CreateContact(dto);

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
            var bad = result.Result as BadRequestObjectResult;
            var message = bad?.Value?.GetType().GetProperty("message")!.GetValue(bad.Value) as string;
            Assert.That(message, Is.EqualTo("fail"));
        }

        [Test]
        public async Task EditContact_ReturnsOk_OnSuccess()
        {
            var dto = new ContactDataDTO { Id = "1", Name = "c", ContactDetails = "123", CreatedDate = DateTime.UtcNow, UserId = "u" };
            _contactServiceMock.Setup(s => s.UpdateContactAsync(dto)).ReturnsAsync(true);

            var result = await _controller.EditContact(dto);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task EditContact_ReturnsBadRequest_OnException()
        {
            _contactServiceMock.Setup(s => s.UpdateContactAsync(It.IsAny<ContactDataDTO>())).ThrowsAsync(new Exception("err"));

            var dto = new ContactDataDTO { Id = "1", Name = "c", ContactDetails = "123", CreatedDate = DateTime.UtcNow, UserId = "u" };
            var result = await _controller.EditContact(dto);

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task DeleteContact_ReturnsOk_OnSuccess()
        {
            _contactServiceMock.Setup(s => s.DeleteContactAsync("1")).ReturnsAsync(true);

            var result = await _controller.DeleteContact("1");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task DeleteContact_ReturnsBadRequest_OnException()
        {
            _contactServiceMock.Setup(s => s.DeleteContactAsync(It.IsAny<string>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.DeleteContact("1");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetContactById_ReturnsOk_OnSuccess()
        {
            var contact = new ContactDataDTO { Id = "1", Name = "c", ContactDetails = "123", CreatedDate = DateTime.UtcNow, UserId = "u" };
            _contactServiceMock.Setup(s => s.GetContactAsync("1")).ReturnsAsync(contact);

            var result = await _controller.GetContactById("1");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.SameAs(contact));
        }

        [Test]
        public async Task GetContactById_ReturnsBadRequest_OnException()
        {
            _contactServiceMock.Setup(s => s.GetContactAsync(It.IsAny<string>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.GetContactById("1");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetContactList_ReturnsOk_OnSuccess()
        {
            var list = new List<ContactDataDTO> { new ContactDataDTO { Id = "1", Name = "c", ContactDetails = "123", CreatedDate = DateTime.UtcNow, UserId = "u" } };
            _contactServiceMock.Setup(s => s.GetAllContactsAsync()).ReturnsAsync(list);

            var result = await _controller.GetContactList();

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.SameAs(list));
        }

        [Test]
        public async Task GetContactList_ReturnsBadRequest_OnException()
        {
            _contactServiceMock.Setup(s => s.GetAllContactsAsync()).ThrowsAsync(new Exception("boom"));

            var result = await _controller.GetContactList();

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetContactListByUserId_ReturnsOk_OnSuccess()
        {
            var list = new List<ContactDataDTO> { new ContactDataDTO { Id = "1", Name = "c", ContactDetails = "123", CreatedDate = DateTime.UtcNow, UserId = "u" } };
            _contactServiceMock.Setup(s => s.GetAllContactsByUserIdAsync("u")).ReturnsAsync(list);

            var result = await _controller.GetContactListByUserId("u");

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.SameAs(list));
        }

        [Test]
        public async Task GetContactListByUserId_ReturnsBadRequest_OnException()
        {
            _contactServiceMock.Setup(s => s.GetAllContactsByUserIdAsync(It.IsAny<string>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.GetContactListByUserId("u");

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task ChangePrivacy_ReturnsOk_OnSuccess()
        {
            _contactServiceMock.Setup(s => s.ChangePrivacyAsync("1", true)).ReturnsAsync(true);

            var result = await _controller.ChangePrivacy("1", true);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task ChangePrivacy_ReturnsBadRequest_OnException()
        {
            _contactServiceMock.Setup(s => s.ChangePrivacyAsync(It.IsAny<string>(), It.IsAny<bool>())).ThrowsAsync(new Exception("err"));

            var result = await _controller.ChangePrivacy("1", true);

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }
    }
}
