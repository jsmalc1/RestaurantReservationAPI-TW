namespace RestaurantAPI.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int TableId { get; set; }
        public RestaurantTable Table { get; set; } = null!;

        public int TimeSlotId { get; set; }
        public TimeSlot TimeSlot { get; set; } = null!;

        public DateTime ReservationDate { get; set; }
        public int GuestCount { get; set; }
        public string Status { get; set; } = "Pending"; // pending confirmed ili cancelled

        public ICollection<ReservationService> ReservationServices { get; set; } = new List<ReservationService>();
    }
}
