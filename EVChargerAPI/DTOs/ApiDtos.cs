namespace EVChargerAPI.DTOs
{
    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterDto
    {
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? VehicleNumber { get; set; }
        public string? VehicleType { get; set; }
        public string? VehicleBrand { get; set; }
        public string? VehicleModel { get; set; }
        public string Role { get; set; } = string.Empty;
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? VehicleNumber { get; set; }
        public string? VehicleType { get; set; }
        public string? VehicleBrand { get; set; }
        public string? VehicleModel { get; set; }
    }

    public class StationDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string[] ConnectorTypes { get; set; } = Array.Empty<string>();
        public string PowerOutput { get; set; } = string.Empty;
        public decimal PricePerKwh { get; set; }
        public string[] Amenities { get; set; } = Array.Empty<string>();
        public string OperatingHours { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string ApprovalStatus { get; set; } = string.Empty;
        public int TotalSlots { get; set; }
        public int AvailableSlots { get; set; }
        public double? Distance { get; set; }
        public string? OwnerName { get; set; }
    }

    public class BookingDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int StationId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? StationName { get; set; }
        public string? UserName { get; set; }
        
        // New fields
        public string Date { get; set; } = string.Empty;
        public string TimeSlot { get; set; } = string.Empty;
        public int Duration { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;
        public string VehicleBrand { get; set; } = string.Empty;
        public string VehicleModel { get; set; } = string.Empty;
        public string VehicleNumber { get; set; } = string.Empty;
        public string PaymentId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? CancellationMessage { get; set; }
    }

    public class CreateBookingDto
    {
        public int StationId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal Amount { get; set; }
        
        // New fields
        public string Date { get; set; } = string.Empty;
        public string TimeSlot { get; set; } = string.Empty;
        public int Duration { get; set; } = 1;
        public string PaymentMethod { get; set; } = "Card";
        public string VehicleType { get; set; } = string.Empty;
        public string VehicleBrand { get; set; } = string.Empty;
        public string VehicleModel { get; set; } = string.Empty;
        public string VehicleNumber { get; set; } = string.Empty;
        public string PaymentId { get; set; } = string.Empty;
        public string Status { get; set; } = "Confirmed";
    }

    public class AdminCancelBookingDto
    {
        public int BookingId { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class UpdateBookingDto
    {
        public string? Status { get; set; }
        public string? CancellationMessage { get; set; }
    }

    public class CancelBookingDto
    {
        public string Message { get; set; } = string.Empty;
    }

    public class UpdateProfileDto
    {
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? VehicleNumber { get; set; }
        public string? VehicleType { get; set; }
        public string? VehicleBrand { get; set; }
        public string? VehicleModel { get; set; }
    }

    public class CreateStationDto
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string[] ConnectorTypes { get; set; } = Array.Empty<string>();
        public string PowerOutput { get; set; } = string.Empty;
        public decimal PricePerKwh { get; set; }
        public string[] Amenities { get; set; } = Array.Empty<string>();
        public string OperatingHours { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int TotalSlots { get; set; }
        public int AvailableSlots { get; set; }
    }

    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class GoogleLoginDto
    {
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class CreateReviewDto
    {
        public int StationId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}