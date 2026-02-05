using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using EVChargerAPI.Data;
using EVChargerAPI.Models;
using EVChargerAPI.DTOs;
using System.Security.Claims;
using System.Text.Json;

namespace EVChargerAPI.Controllers
{
    [ApiController]
    [Route("api/station-master")]
    [Authorize(Roles = "Admin,StationMaster")]
    public class StationMasterController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<StationMasterController> _logger;

        public StationMasterController(AppDbContext context, ILogger<StationMasterController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("stations")]
        public async Task<IActionResult> GetMyStations()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            var stations = await _context.Stations
                .Where(s => s.OwnerId == userId)
                .ToListAsync();

            var stationDtos = stations.Select(s => new StationDto
            {
                Id = s.Id,
                Name = s.Name,
                Address = s.Address,
                Latitude = s.Latitude,
                Longitude = s.Longitude,
                ConnectorTypes = string.IsNullOrEmpty(s.ConnectorTypes) ? Array.Empty<string>() : JsonSerializer.Deserialize<string[]>(s.ConnectorTypes) ?? Array.Empty<string>(),
                PowerOutput = s.PowerOutput,
                PricePerKwh = s.PricePerKwh,
                Amenities = string.IsNullOrEmpty(s.Amenities) ? Array.Empty<string>() : JsonSerializer.Deserialize<string[]>(s.Amenities) ?? Array.Empty<string>(),
                OperatingHours = s.OperatingHours,
                Status = s.Status,
                ApprovalStatus = s.ApprovalStatus ?? "Pending",
                TotalSlots = s.TotalSlots,
                AvailableSlots = s.AvailableSlots
            });

            return Ok(stationDtos);
        }

        [HttpPost("stations")]
        public async Task<IActionResult> CreateStation(CreateStationDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                _logger.LogInformation($"Creating station: {dto.Name}");

                if (string.IsNullOrEmpty(dto.Name) || string.IsNullOrEmpty(dto.Address))
                {
                    return BadRequest(new { message = "Name and Address are required" });
                }

                if (dto.Latitude == 0 || dto.Longitude == 0)
                {
                    return BadRequest(new { message = "Valid Latitude and Longitude are required" });
                }

                var station = new Station
                {
                    Name = dto.Name,
                    Address = dto.Address,
                    Latitude = dto.Latitude,
                    Longitude = dto.Longitude,
                    ConnectorTypes = JsonSerializer.Serialize(dto.ConnectorTypes ?? Array.Empty<string>()),
                    PowerOutput = dto.PowerOutput,
                    PricePerKwh = dto.PricePerKwh,
                    Amenities = JsonSerializer.Serialize(dto.Amenities ?? Array.Empty<string>()),
                    OperatingHours = dto.OperatingHours,
                    Status = dto.Status,
                    ApprovalStatus = "Pending",
                    TotalSlots = dto.TotalSlots,
                    AvailableSlots = dto.TotalSlots,
                    OwnerId = userId
                };

                _context.Stations.Add(station);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Station created successfully with ID: {station.Id}");
                return Ok(new { station = new { id = station.Id, name = station.Name } });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating station: {ex.Message}\n{ex.InnerException?.Message}");
                return StatusCode(500, new { message = "Error creating station", error = ex.Message, innerError = ex.InnerException?.Message });
            }
        }

        [HttpPut("stations/{id}")]
        public async Task<IActionResult> UpdateStation(int id, StationDto dto)
        {
            var station = await _context.Stations.FindAsync(id);
            if (station == null) return NotFound();

            station.Name = dto.Name;
            station.Address = dto.Address;
            station.Latitude = dto.Latitude;
            station.Longitude = dto.Longitude;
            station.ConnectorTypes = JsonSerializer.Serialize(dto.ConnectorTypes);
            station.PowerOutput = dto.PowerOutput;
            station.PricePerKwh = dto.PricePerKwh;
            station.Amenities = JsonSerializer.Serialize(dto.Amenities);
            station.OperatingHours = dto.OperatingHours;
            station.Status = dto.Status;
            station.TotalSlots = dto.TotalSlots;
            station.AvailableSlots = dto.AvailableSlots;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Station updated successfully" });
        }

        [HttpPut("stations/{id}/status")]
        public async Task<IActionResult> UpdateStationStatus(int id, [FromBody] Dictionary<string, string> statusUpdate)
        {
            var station = await _context.Stations.FindAsync(id);
            if (station == null) return NotFound();

            if (statusUpdate.TryGetValue("status", out var status))
            {
                station.Status = status;
                await _context.SaveChangesAsync();
                return Ok(new { message = "Station status updated successfully" });
            }

            return BadRequest("Status is required");
        }

        [HttpGet("stations/{id}/bookings")]
        public async Task<IActionResult> GetStationBookings(int id)
        {
            var station = await _context.Stations.FindAsync(id);
            if (station == null) return NotFound();

            var bookings = await _context.Bookings
                .Where(b => b.StationId == id)
                .Include(b => b.User)
                .Include(b => b.Station)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            var bookingDtos = bookings.Select(b => new BookingDto
            {
                Id = b.Id,
                UserId = b.UserId,
                StationId = b.StationId,
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                Status = b.Status,
                Amount = b.Amount,
                StationName = b.Station?.Name,
                UserName = b.User?.Name,
                Date = b.Date,
                TimeSlot = b.TimeSlot,
                Duration = b.Duration,
                PaymentMethod = b.PaymentMethod,
                VehicleType = b.VehicleType,
                VehicleBrand = b.VehicleBrand,
                VehicleModel = b.VehicleModel,
                VehicleNumber = b.VehicleNumber,
                PaymentId = b.PaymentId,
                CreatedAt = b.CreatedAt,
                CancellationMessage = b.CancellationMessage
            });

            return Ok(bookingDtos);
        }

        [HttpGet("stations/{id}/reviews")]
        public async Task<IActionResult> GetStationReviews(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            // Check if station belongs to this station master
            var station = await _context.Stations.FirstOrDefaultAsync(s => s.Id == id && s.OwnerId == userId);
            if (station == null)
                return NotFound(new { message = "Station not found or not owned by you" });
            
            var reviews = await _context.Reviews
                .Where(r => r.StationId == id)
                .Include(r => r.User)
                .OrderBy(r => r.Rating)
                .ToListAsync();

            return Ok(reviews);
        }

        [HttpGet("reviews")]
        public async Task<IActionResult> GetMyStationReviews()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                
                var reviews = await _context.Reviews
                    .Join(_context.Users, r => r.UserId, u => u.Id, (r, u) => new { r, u })
                    .Join(_context.Stations.Where(s => s.OwnerId == userId), ru => ru.r.StationId, s => s.Id, (ru, s) => new {
                        Id = ru.r.Id,
                        UserId = ru.r.UserId,
                        StationId = ru.r.StationId,
                        Rating = ru.r.Rating,
                        Comment = ru.r.Comment,
                        CreatedAt = ru.r.CreatedAt,
                        User = new { Name = ru.u.Name, Email = ru.u.Email },
                        Station = new { Name = s.Name }
                    })
                    .OrderBy(r => r.Rating)
                    .ToListAsync();
                
                return Ok(reviews);
            }
            catch
            {
                return Ok(new List<object>());
            }
        }

        [HttpPut("bookings/{id}/complete")]
        public async Task<IActionResult> CompleteBooking(int id)
        {
            try
            {
                var booking = await _context.Bookings
                    .Include(b => b.Station)
                    .FirstOrDefaultAsync(b => b.Id == id);
                
                if (booking == null)
                    return NotFound(new { message = "Booking not found" });
                
                booking.Status = "Completed";
                
                if (booking.Station != null)
                {
                    booking.Station.AvailableSlots++;
                }
                
                await _context.SaveChangesAsync();
                
                return Ok(new { message = "Booking completed successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error completing booking" });
            }
        }
    }
}