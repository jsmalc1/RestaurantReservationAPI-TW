using Microsoft.IdentityModel.Tokens;
using RestaurantAPI.DTOs;
using RestaurantAPI.Models;
using RestaurantAPI.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RestaurantAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<string> RegisterAsync(RegisterDto dto)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(dto.Email);
            if (existingUser != null) throw new Exception("Korisnik s ovim emailom vec postoji.");

            var user = new User
            {
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            var defaultRole = await _userRepository.GetRoleByNameAsync("Korisnik");
            if (defaultRole != null)
            {
                user.UserRoles.Add(new UserRole { RoleId = defaultRole.Id });
            }

            await _userRepository.AddUserAsync(user);

            await _userRepository.AddUserAsync(user);
            await _userRepository.SaveChangesAsync();

            return "Uspjesna registracija!";
        }

        public async Task<string> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.GetUserByEmailAsync(dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                throw new Exception("Pogresan email ili lozinka.");
            }

            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            foreach (var userRole in user.UserRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
            }

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}