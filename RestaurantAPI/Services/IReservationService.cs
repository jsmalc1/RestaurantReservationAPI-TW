using RestaurantAPI.DTOs;
using RestaurantAPI.Models;

namespace RestaurantAPI.Services
{
    public interface IReservationService
    {
        Task<IEnumerable<Reservation>> GetAllAsync();
        Task AddAsync(int userId, CreateReservationDto dto);
        Task UpdateStatusAsync(int id, string status);
        Task DeleteAsync(int id);
        Task UpdateAsync(int id, int userId, CreateReservationDto dto);
    }
}