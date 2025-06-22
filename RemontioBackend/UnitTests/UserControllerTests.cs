using NUnit.Framework;
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers;
using Application.Interfaces.ServiceInterfaces;
using Application.Objects.DTOs.UserDTO;
using System;
using System.Collections.Generic;

namespace Presentation.Tests
{
    [TestFixture]
    public class UserControllerTests
    {
        private Mock<IUserService> _userServiceMock;
        private Mock<ITokenService> _tokenServiceMock;
        private UserController _controller;

        [SetUp]
        public void Setup()
        {
            _userServiceMock = new Mock<IUserService>();
            _tokenServiceMock = new Mock<ITokenService>();
            _controller = new UserController(_userServiceMock.Object, _tokenServiceMock.Object);
        }

        [Test]
        public async Task LogIn_ShouldReturnOkWithToken_WhenCredentialsValid()
        {
            // Arrange  
            var username = "admin";
            var password = "Admin1!";
            var userData = new UserDataDTO { Id = "123" };
            _userServiceMock.Setup(s => s.LogInAsync(username, password)).ReturnsAsync(userData);
            _tokenServiceMock.Setup(t => t.GenerateToken(userData)).Returns("token");
            _userServiceMock.Setup(s => s.GetRefreshTokenFromDBAsync(userData.Id)).ReturnsAsync("refreshToken"); // Fixed: ReturnsAsync instead of Returns  

            // Act  
            var result = await _controller.LogIn(username, password);

            // Assert  
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            var value = okResult!.Value;

            var resultProperty = value!.GetType().GetProperty("result")!.GetValue(value, null);
            var tokenProperty = value.GetType().GetProperty("token")!.GetValue(value, null);
            var refreshTokenProperty = value.GetType().GetProperty("refreshToken")!.GetValue(value, null);

            Assert.That(resultProperty, Is.EqualTo(userData));
            Assert.That(tokenProperty, Is.EqualTo("token"));
            Assert.That(refreshTokenProperty, Is.EqualTo("refreshToken"));
        }

        [Test]
        public async Task LogIn_ShouldReturnBadRequest_WhenInvalidCredentials()
        {
            // Arrange
            _userServiceMock.Setup(s => s.LogInAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((UserDataDTO)null!);

            // Act
            var result = await _controller.LogIn("bad", "bad");

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
            var badRequest = result.Result as BadRequestObjectResult;
            Assert.That(badRequest!.Value, Has.Property("message").EqualTo("Invalid username or password"));
        }

        [Test]
        public async Task Register_ShouldReturnOK()
        {
            // Arrange
            var dto = new CreateUserDTO
            {
                UserName = "testuser",
                Password = "Test1!",
                Role = "User",
                Email = "test@test",
                Name = "Test",
            };
            _userServiceMock.Setup(s => s.CreateUserAsync(dto)).ReturnsAsync(true);

            // Act
            var result = await _controller.Register(dto);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null, "Expected OkObjectResult but got null.");
            Assert.That(okResult!.Value, Has.Property("message").EqualTo("User created successfully"));
        }

        [Test]
        public async Task Register_ShouldReturnBadRequest_WhenPasswordIsInvalid()
        {
            // Arrange
            var dto = new CreateUserDTO
            {
                UserName = "testuser",
                Password = "", 
                Role = "User",
                Email = "test@test",
                Name = "Test",
            };

            _userServiceMock
                .Setup(s => s.CreateUserAsync(dto))
                .ThrowsAsync(new Exception("Password cannot be empty"));

            // Act
            var result = await _controller.Register(dto);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null, "Expected BadRequestObjectResult but got null.");
            Assert.That(badRequestResult!.Value, Has.Property("message").EqualTo("Password cannot be empty"));
        }

        [Test]
        public async Task GetUserList_ShouldReturnOkResultWithUserList()
        {
            var users = new List<UserDataDTO> { new UserDataDTO { Id = "1" }, new UserDataDTO { Id = "2" } };
            _userServiceMock.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(users);

            var result = await _controller.GetUserList();

            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult!.Value, Is.EqualTo(users));
        }

        [Test]
        public async Task GetUserList_ShouldReturnBadRequest_WhenExceptionThrown()
        {
            // Arrange
            _userServiceMock.Setup(s => s.GetAllUsersAsync())
                            .ThrowsAsync(new Exception("Something went wrong"));

            // Act
            var result = await _controller.GetUserList();

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
            var badRequest = result.Result as BadRequestObjectResult;
            Assert.That(badRequest!.Value, Has.Property("message").EqualTo("Something went wrong"));
        }

        [Test]
        public async Task GetUser_ShouldReturnOkWithUser()
        {
            var user = new UserDataDTO { Id = "123" };
            _userServiceMock.Setup(s => s.GetUserAsync("123")).ReturnsAsync(user);

            var result = await _controller.GetUser("123");

            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult!.Value, Is.EqualTo(user));
        }

        [Test]
        public async Task GetUser_ShouldReturnBadRequest_WhenExceptionThrown()
        {
            // Arrange
            _userServiceMock.Setup(s => s.GetUserAsync("123"))
                            .ThrowsAsync(new Exception("User not found"));

            // Act
            var result = await _controller.GetUser("123");

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
            var badRequest = result.Result as BadRequestObjectResult;
            Assert.That(badRequest!.Value, Has.Property("message").EqualTo("User not found"));
        }

        [Test]
        public async Task RemoveUser_ShouldReturnOk()
        {
            var user = new UserDataDTO { Id = "123" };
            _userServiceMock.Setup(s => s.DeleteUserAsync("123")).ReturnsAsync(true);

            var result = await _controller.RemoveUser("123");

            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task RemoveUser_ShouldReturnBadRequest_WhenExceptionThrown()
        {
            // Arrange
            _userServiceMock.Setup(s => s.DeleteUserAsync("123"))
                            .ThrowsAsync(new Exception("Deletion failed"));

            // Act
            var result = await _controller.RemoveUser("123");

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
            var badRequest = result.Result as BadRequestObjectResult;
            Assert.That(badRequest!.Value, Has.Property("message").EqualTo("Deletion failed"));
        }

        [Test]
        public async Task UpdateUser_ShouldReturnOk()
        {
            var user = new UserDataDTO { Id = "123", UserName = "updated" };
            _userServiceMock.Setup(s => s.UpdateUserAsync("123", user)).ReturnsAsync(true);

            var result = await _controller.UpdateUser("123", user);

            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task UpdateUser_ShouldReturnBadRequest_WhenExceptionThrown()
        {
            // Arrange
            var user = new UserDataDTO { Id = "123", UserName = "updated" };
            _userServiceMock.Setup(s => s.UpdateUserAsync("123", user))
                            .ThrowsAsync(new Exception("Update failed"));

            // Act
            var result = await _controller.UpdateUser("123", user);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
            var badRequest = result.Result as BadRequestObjectResult;
            Assert.That(badRequest!.Value, Has.Property("message").EqualTo("Update failed"));
        }

        [Test]
        public async Task ChangeUserPassword_ShouldReturnOk()
        {
            var user = new UserDataDTO { Id = "123" };
            _userServiceMock.Setup(s => s.ChangePasswordAsync("123", "old", "new"))
                            .ReturnsAsync(true);

            var result = await _controller.ChangeUserPassword("123", "old", "new");

            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task ChangeUserPassword_ShouldReturnBadRequest_WhenExceptionThrown()
        {
            // Arrange
            _userServiceMock.Setup(s => s.ChangePasswordAsync("123", "old", "new"))
                            .ThrowsAsync(new Exception("Password change failed"));

            // Act
            var result = await _controller.ChangeUserPassword("123", "old", "new");

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
            var badRequest = result.Result as BadRequestObjectResult;
            Assert.That(badRequest!.Value, Has.Property("message").EqualTo("Password change failed"));
        }


        [Test]
        public async Task ChangeUserRole_ShouldReturnOk()
        {
            var user = new UserDataDTO { Id = "123", Role = "Admin" };
            _userServiceMock.Setup(s => s.ChangeRoleAsync("123", "Admin"))
                            .ReturnsAsync(true);

            var result = await _controller.ChangeUserRole("123", "Admin");

            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult!.Value, Is.EqualTo(true));
        }

        [Test]
        public async Task ChangeUserRole_ShouldReturnBadRequest_WhenExceptionThrown()
        {
            // Arrange
            _userServiceMock.Setup(s => s.ChangeRoleAsync("123", "Admin"))
                            .ThrowsAsync(new Exception("Role change failed"));

            // Act
            var result = await _controller.ChangeUserRole("123", "Admin");

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
            var badRequest = result.Result as BadRequestObjectResult;
            Assert.That(badRequest!.Value, Has.Property("message").EqualTo("Role change failed"));
        }
    }
}
