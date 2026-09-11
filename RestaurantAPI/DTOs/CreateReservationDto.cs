namespace RestaurantAPI.DTOs
{
    public class CreateReservationDto
    {
        public int TableId { get; set; }
        public int TimeSlotId { get; set; }
        public DateTime ReservationDate { get; set; }
        public int GuestCount { get; set; }
        public List<int> SpecialServiceIds { get; set; } = new List<int>();
    }
}