namespace RestaurantAPI.Models
{
    public class SpecialService
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;

        public ICollection<ReservationService> ReservationServices { get; set; } = new List<ReservationService>();
    }
}
