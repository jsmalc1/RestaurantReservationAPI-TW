using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.DTOs;
using RestaurantAPI.Services;
using System.Security.Claims;

namespace RestaurantAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var reservations = await _reservationService.GetAllAsync();
            return Ok(reservations);
        }

        [HttpPost]
        [Authorize(Roles = "Korisnik,Admin")]
        public async Task<IActionResult> Create([FromBody] CreateReservationDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            await _reservationService.AddAsync(userId, dto);
            return Ok(new { message = "Rezervacija uspjesno kreirana!" });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Korisnik")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateReservationDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            await _reservationService.UpdateAsync(id, userId, dto);
            return Ok(new { message = "Rezervacija azurirana." });
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            await _reservationService.UpdateStatusAsync(id, status);
            return Ok(new { message = "Status azuriran." });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Korisnik")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var isAdmin = User.IsInRole("Admin");

            var all = await _reservationService.GetAllAsync();
            var res = all.FirstOrDefault(r => r.Id == id);

            if (res == null) return NotFound();
            if (!isAdmin && res.UserId != userId) return Unauthorized("Ne možete obrisati tuđu rezervaciju.");

            await _reservationService.DeleteAsync(id);
            return Ok(new { message = "Rezervacija obrisana." });
        }

        [HttpGet("my")]
        [Authorize(Roles = "Korisnik,Admin")]
        public async Task<IActionResult> GetMyReservations()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var all = await _reservationService.GetAllAsync();
            var my = all.Where(r => r.UserId == userId).ToList();
            return Ok(my);
        }
    }
}