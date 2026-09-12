using Microsoft.AspNetCore.Mvc;
using Moq;
using RestaurantAPI.Controllers;
using RestaurantAPI.DTOs;
using RestaurantAPI.Services;

namespace RestaurantAPI.Tests
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _controller = new AuthController(_mockAuthService.Object);
        }

        [Fact]
        public async Task Register_ReturnsOk_WhenRegistrationIsSuccessful()
        {
            var registerDto = new RegisterDto
            {
                Email = "test@test.com",
                Password = "Password123",
                FirstName = "Ivan",
                LastName = "Horvat"
            };
            _mockAuthService.Setup(s => s.RegisterAsync(registerDto))
                            .ReturnsAsync("Uspjesna registracija!");

            var result = await _controller.Register(registerDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task Login_ReturnsOkWithToken_WhenCredentialsAreValid()
        {
            var loginDto = new LoginDto { Email = "test@test.com", Password = "Password123" };
            var fakeToken = "lazni-jwt-token";

            _mockAuthService.Setup(s => s.LoginAsync(loginDto))
                            .ReturnsAsync(fakeToken);

            var result = await _controller.Login(loginDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task Login_ThrowsException_WhenCredentialsAreInvalid()
        {
            var loginDto = new LoginDto { Email = "test@test.com", Password = "WrongPassword!" };

            _mockAuthService.Setup(s => s.LoginAsync(loginDto))
                            .ThrowsAsync(new Exception("Pogresan email ili lozinka."));

            var exception = await Assert.ThrowsAsync<Exception>(() => _controller.Login(loginDto));

            Assert.Equal("Pogresan email ili lozinka.", exception.Message);
        }
    }
}