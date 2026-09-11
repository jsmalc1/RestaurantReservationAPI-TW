using RestaurantAPI.Models;

namespace RestaurantAPI.Repositories
{
    public interface IReservationRepository
    {
        Task<IEnumerable<Reservation>> GetAllReservationsAsync();
        Task AddReservationAsync(Reservation reservation);
        Task SaveChangesAsync();
        Task<Reservation?> GetByIdAsync(int id);
        void UpdateReservation(Reservation reservation);
        void DeleteReservation(Reservation reservation);
    }
}