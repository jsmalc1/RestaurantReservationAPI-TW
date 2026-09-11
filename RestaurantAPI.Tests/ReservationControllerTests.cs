using Microsoft.AspNetCore.Mvc;
using Moq;
using RestaurantAPI.Controllers;
using RestaurantAPI.DTOs;
using RestaurantAPI.Models;
using RestaurantAPI.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace RestaurantAPI.Tests
{
    public class ReservationControllerTests
    {
        private readonly Mock<IReservationService> _mockService;
        private readonly ReservationController _controller;

        public ReservationControllerTests()
        {
            _mockService = new Mock<IReservationService>();
            _controller = new ReservationController(_mockService.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult_WithListOfReservations()
        {
            var mockReservations = new List<Reservation>
            {
                new Reservation { Id = 1, GuestCount = 2, Status = "Pending" },
                new Reservation { Id = 2, GuestCount = 4, Status = "Confirmed" }
            };
            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(mockReservations);

            var result = await _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<Reservation>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task UpdateStatus_ReturnsOkResult_WhenAdminUpdatesStatus()
        {
            int reservationId = 1;
            string newStatus = "Potvrđeno";
            _mockService.Setup(s => s.UpdateStatusAsync(reservationId, newStatus)).Returns(Task.CompletedTask);

            var result = await _controller.UpdateStatus(reservationId, newStatus);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task Delete_ReturnsOkResult_WhenValidIdIsProvided()
        {
            int reservationId = 1;
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, "Admin")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<Reservation>
            {
                new Reservation { Id = reservationId, UserId = 1 }
            });
            _mockService.Setup(s => s.DeleteAsync(reservationId)).Returns(Task.CompletedTask);

            var result = await _controller.Delete(reservationId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }
    }
}