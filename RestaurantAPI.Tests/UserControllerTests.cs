using Microsoft.AspNetCore.Mvc;
using Moq;
using RestaurantAPI.Controllers;
using RestaurantAPI.DTOs;
using RestaurantAPI.Models;
using RestaurantAPI.Repositories;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace RestaurantAPI.Tests
{
    public class UserControllerTests
    {
        private readonly Mock<IUserRepository> _mockRepo;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _mockRepo = new Mock<IUserRepository>();
            _controller = new UserController(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult_WithListOfUserDtos()
        {
            var mockUsers = new List<User>
            {
                new User { Id = 1, FirstName = "Ana", LastName = "Anic", Email = "ana@test.com" },
                new User { Id = 2, FirstName = "Ivan", LastName = "Ivic", Email = "ivan@test.com" }
            };
            _mockRepo.Setup(repo => repo.GetAllUsersAsync()).ReturnsAsync(mockUsers);

            var result = await _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<UserDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count());
        }

        [Fact]
        public async Task Delete_ReturnsBadRequest_WhenAdminTriesToDeleteHimself()
        {
            int adminId = 1;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, adminId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            var result = await _controller.Delete(adminId);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}