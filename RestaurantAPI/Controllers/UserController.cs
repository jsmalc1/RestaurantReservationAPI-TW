using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.DTOs;
using RestaurantAPI.Repositories;
using System.Security.Claims;

namespace RestaurantAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepo;

        public UserController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userRepo.GetAllUsersAsync();
            var dtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email
            });
            return Ok(dtos);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            if (id == currentUserId) return BadRequest(new { error = "Ne mozete obrisati vlastiti racun" });

            var user = await _userRepo.GetByIdAsync(id);
            if (user == null) return NotFound(new { error = "Korisnik nije pronaden" });

            await _userRepo.DeleteUserAsync(user);
            return Ok(new { message = "Korisnik uspjesno obrisan" });
        }
    }
}