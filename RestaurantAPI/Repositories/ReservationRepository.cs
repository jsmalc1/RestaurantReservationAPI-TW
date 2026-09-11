using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Data;
using RestaurantAPI.Models;

namespace RestaurantAPI.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly RestaurantDbContext _context;

        public ReservationRepository(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Reservation>> GetAllReservationsAsync()
        {
            return await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Table)
                .Include(r => r.ReservationServices)
                .ThenInclude(rs => rs.SpecialService)
                .ToListAsync();
        }

        public async Task AddReservationAsync(Reservation reservation)
        {
            await _context.Reservations.AddAsync(reservation);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Reservation?> GetByIdAsync(int id)
        {
            return await _context.Reservations
                .Include(r => r.ReservationServices)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public void UpdateReservation(Reservation reservation)
        {
            _context.Reservations.Update(reservation);
        }

        public void DeleteReservation(Reservation reservation)
        {
            _context.Reservations.Remove(reservation);
        }
    }
}