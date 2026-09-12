using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Data;
using RestaurantAPI.Models;

namespace RestaurantAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly RestaurantDbContext _context;

        public UserRepository(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Role?> GetRoleByNameAsync(string roleName)
        {
            return await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task DeleteUserAsync(User user)
        {
            var userReservations = await _context.Reservations
                .Where(r => r.UserId == user.Id)
                .ToListAsync();

            if (userReservations.Any())
            {
                _context.Reservations.RemoveRange(userReservations);
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}