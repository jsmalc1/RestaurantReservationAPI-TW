using RestaurantAPI.Models;

public class ReservationService
{
    public int ReservationId { get; set; }
    public Reservation Reservation { get; set; } = null!;

    public int SpecialServiceId { get; set; }
    public SpecialService SpecialService { get; set; } = null!;
}