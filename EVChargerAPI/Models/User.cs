using System.ComponentModel.DataAnnotations;

namespace EVChargerAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        
        public string? Phone { get; set; }
        
        public string? VehicleNumber { get; set; }
        
        public string? VehicleType { get; set; } // "Car" or "Bike"
        
        public string? VehicleBrand { get; set; }
        
        public string? VehicleModel { get; set; }
        
        [Required]
        public string Role { get; set; } = "User";
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Car> Cars { get; set; } = new List<Car>();
    }
}