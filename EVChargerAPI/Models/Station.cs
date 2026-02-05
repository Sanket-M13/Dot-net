using System.ComponentModel.DataAnnotations;

namespace EVChargerAPI.Models
{
    public class Station
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public string Address { get; set; } = string.Empty;
        
        [Required]
        public double Latitude { get; set; }
        
        [Required]
        public double Longitude { get; set; }
        
        public string ConnectorTypes { get; set; } = string.Empty; // JSON array as string
        
        public string PowerOutput { get; set; } = string.Empty;
        
        public decimal PricePerKwh { get; set; }
        
        public string? Amenities { get; set; } // JSON array as string
        
        public string OperatingHours { get; set; } = string.Empty;
        
        [Required]
        public string Status { get; set; } = "Available";
        
        public string ApprovalStatus { get; set; } = "Pending";
        
        public int TotalSlots { get; set; }
        
        public int AvailableSlots { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public int? OwnerId { get; set; }
        
        // Navigation properties
        public User? Owner { get; set; }
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}