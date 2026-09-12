using RestaurantAPI.Models;

namespace RestaurantAPI.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task AddUserAsync(User user);
        Task SaveChangesAsync();
        Task<Role?> GetRoleByNameAsync(string roleName);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetByIdAsync(int id);
        Task DeleteUserAsync(User user);
    }
}