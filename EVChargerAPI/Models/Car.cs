namespace EVChargerAPI.Models
{
    public class Car
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string VehicleNumber { get; set; } = string.Empty;
        public int Range { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public User User { get; set; } = null!;
    }
}