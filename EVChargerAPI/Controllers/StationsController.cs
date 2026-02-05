using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EVChargerAPI.Data;
using EVChargerAPI.Models;
using EVChargerAPI.DTOs;
using System.Text.Json;

namespace EVChargerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StationsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetStations()
        {
            var stations = await _context.Stations.ToListAsync();
            var stationDtos = new List<StationDto>();
            
            foreach (var s in stations)
            {
                // Count current active bookings (not cancelled, not completed, and not expired)
                var currentTime = DateTime.Now;
                var activeBookings = await _context.Bookings
                    .Where(b => b.StationId == s.Id && 
                               b.Status != "Cancelled" && 
                               b.Status != "Completed" &&
                               b.EndTime > currentTime)
                    .CountAsync();
                
                // Calculate available slots
                var availableSlots = Math.Max(0, s.TotalSlots - activeBookings);
                
                stationDtos.Add(new StationDto
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
                    AvailableSlots = availableSlots
                });
            }

            return Ok(new { stations = stationDtos });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStation(int id)
        {
            var station = await _context.Stations.FindAsync(id);
            if (station == null) return NotFound();

            var stationDto = new StationDto
            {
                Id = station.Id,
                Name = station.Name,
                Address = station.Address,
                Latitude = station.Latitude,
                Longitude = station.Longitude,
                ConnectorTypes = string.IsNullOrEmpty(station.ConnectorTypes) ? Array.Empty<string>() : JsonSerializer.Deserialize<string[]>(station.ConnectorTypes) ?? Array.Empty<string>(),
                PowerOutput = station.PowerOutput,
                PricePerKwh = station.PricePerKwh,
                Amenities = string.IsNullOrEmpty(station.Amenities) ? Array.Empty<string>() : JsonSerializer.Deserialize<string[]>(station.Amenities) ?? Array.Empty<string>(),
                OperatingHours = station.OperatingHours,
                Status = station.Status,
                TotalSlots = station.TotalSlots,
                AvailableSlots = station.AvailableSlots
            };

            return Ok(new { station = stationDto });
        }

        [HttpPost]
        public async Task<IActionResult> CreateStation(StationDto dto)
        {
            var station = new Station
            {
                Name = dto.Name,
                Address = dto.Address,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                ConnectorTypes = JsonSerializer.Serialize(dto.ConnectorTypes),
                PowerOutput = dto.PowerOutput,
                PricePerKwh = dto.PricePerKwh,
                Amenities = JsonSerializer.Serialize(dto.Amenities),
                OperatingHours = dto.OperatingHours,
                Status = dto.Status,
                TotalSlots = dto.TotalSlots,
                AvailableSlots = dto.AvailableSlots
            };

            _context.Stations.Add(station);
            await _context.SaveChangesAsync();

            return Ok(new { station = new StationDto { Id = station.Id, Name = station.Name } });
        }

        [HttpPut("{id}")]
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStation(int id)
        {
            var station = await _context.Stations.FindAsync(id);
            if (station == null) return NotFound();

            _context.Stations.Remove(station);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Station deleted successfully" });
        }

        [HttpGet("nearby")]
        public async Task<IActionResult> GetNearbyStations([FromQuery] double lat, [FromQuery] double lng, [FromQuery] double range = 50)
        {
            var stations = await _context.Stations
                .Where(s => s.Status == "Available") // Only show available stations to users
                .ToListAsync();
            
            var nearbyStations = stations.Where(s => 
            {
                var distance = CalculateDistance(lat, lng, s.Latitude, s.Longitude);
                return distance <= range;
            })
            .OrderBy(s => CalculateDistance(lat, lng, s.Latitude, s.Longitude))
            .ThenByDescending(s => s.AvailableSlots) // Prioritize stations with more available slots
            .Select(s => new StationDto
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
                TotalSlots = s.TotalSlots,
                AvailableSlots = s.AvailableSlots,
                Distance = CalculateDistance(lat, lng, s.Latitude, s.Longitude)
            });

            return Ok(new { stations = nearbyStations });
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchStations([FromQuery] string query)
        {
            var stations = await _context.Stations
                .Where(s => s.Name.Contains(query) || s.Address.Contains(query))
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
                TotalSlots = s.TotalSlots,
                AvailableSlots = s.AvailableSlots
            });

            return Ok(new { stations = stationDtos });
        }

        private static double CalculateDistance(double lat1, double lng1, double lat2, double lng2)
        {
            const double R = 6371; // Earth's radius in kilometers
            var dLat = ToRadians(lat2 - lat1);
            var dLng = ToRadians(lng2 - lng1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private static double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}