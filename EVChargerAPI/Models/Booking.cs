using System.ComponentModel.DataAnnotations;

namespace EVChargerAPI.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int StationId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; } = "Confirmed";
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // New fields for complete booking info
        public string Date { get; set; } = string.Empty;
        public string TimeSlot { get; set; } = string.Empty;
        public int Duration { get; set; } = 1;
        public string PaymentMethod { get; set; } = "Card";
        public string VehicleType { get; set; } = string.Empty;
        public string VehicleBrand { get; set; } = string.Empty;
        public string VehicleModel { get; set; } = string.Empty;
        public string VehicleNumber { get; set; } = string.Empty;
        public string PaymentId { get; set; } = string.Empty;
        public string? CancellationMessage { get; set; }
        
        public User User { get; set; } = null!;
        public Station Station { get; set; } = null!;
    }
}