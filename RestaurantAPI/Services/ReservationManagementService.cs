using RestaurantAPI.DTOs;
using RestaurantAPI.Models;
using RestaurantAPI.Repositories;

namespace RestaurantAPI.Services
{
    public class ReservationManagementService : IReservationService
    {
        private readonly IReservationRepository _repo;

        public ReservationManagementService(IReservationRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Reservation>> GetAllAsync()
        {
            return await _repo.GetAllReservationsAsync();
        }

        public async Task AddAsync(int userId, CreateReservationDto dto)
        {
            var existingReservations = await _repo.GetAllReservationsAsync();
            bool isBooked = existingReservations.Any(r =>
                r.TableId == dto.TableId &&
                r.TimeSlotId == dto.TimeSlotId &&
                r.ReservationDate.Date == dto.ReservationDate.Date &&
                r.Status != "Otkazano");

            if (isBooked) throw new Exception("Ovaj stol je vec rezerviran u odabranom terminu.");

            var reservation = new Reservation
            {
                UserId = userId,
                TableId = dto.TableId,
                TimeSlotId = dto.TimeSlotId,
                ReservationDate = dto.ReservationDate,
                GuestCount = dto.GuestCount,
                Status = "Pending"
            };

            foreach (var serviceId in dto.SpecialServiceIds)
            {
                reservation.ReservationServices.Add(new ReservationService
                {
                    SpecialServiceId = serviceId
                }); 
            }

            await _repo.AddReservationAsync(reservation);
            await _repo.SaveChangesAsync();
        }

        public async Task UpdateAsync(int id, int userId, CreateReservationDto dto)
        {
            var res = await _repo.GetByIdAsync(id);
            if (res == null) throw new Exception("Rezervacija ne postoji.");
            if (res.UserId != userId) throw new Exception("Nemate prava mijenjati tudu rezervaciju.");

            var all = await _repo.GetAllReservationsAsync();
            bool isBooked = all.Any(r => r.Id != id && r.TableId == dto.TableId &&
                                         r.TimeSlotId == dto.TimeSlotId &&
                                         r.ReservationDate.Date == dto.ReservationDate.Date &&
                                         r.Status != "Otkazano");

            if (isBooked) throw new Exception("Novi termin koji ste odabrali je vec zauzet.");

            res.TableId = dto.TableId;
            res.TimeSlotId = dto.TimeSlotId;
            res.ReservationDate = dto.ReservationDate;
            res.GuestCount = dto.GuestCount;
            res.Status = "Pending";

            res.ReservationServices.Clear();
            if (dto.SpecialServiceIds != null)
            {
                foreach (var serviceId in dto.SpecialServiceIds)
                {
                    res.ReservationServices.Add(new ReservationService
                    {
                        SpecialServiceId = serviceId
                    });
                }
            }

            _repo.UpdateReservation(res);
            await _repo.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(int id, string status)
        {
            var reservation = await _repo.GetByIdAsync(id);
            if (reservation != null)
            {
                reservation.Status = status;
                _repo.UpdateReservation(reservation);
                await _repo.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var reservation = await _repo.GetByIdAsync(id);
            if (reservation != null)
            {
                _repo.DeleteReservation(reservation);
                await _repo.SaveChangesAsync();
            }
        }
    }
}