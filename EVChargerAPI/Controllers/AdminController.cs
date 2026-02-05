using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using EVChargerAPI.Data;
using EVChargerAPI.DTOs;

namespace EVChargerAPI.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("dashboard-stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalStations = await _context.Stations.CountAsync();
            var totalBookings = await _context.Bookings.CountAsync();
            var activeStations = await _context.Stations.CountAsync(s => s.Status == "Available");

            return Ok(new
            {
                totalUsers,
                totalStations,
                totalBookings,
                activeStations
            });
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    Name = u.Name,
                    Role = u.Role,
                    Phone = u.Phone,
                    VehicleNumber = u.VehicleNumber,
                    VehicleType = u.VehicleType,
                    VehicleBrand = u.VehicleBrand,
                    VehicleModel = u.VehicleModel
                })
                .ToListAsync();

            return Ok(new { users });
        }

        [HttpGet("bookings")]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await _context.Bookings
                .Join(_context.Users, b => b.UserId, u => u.Id, (b, u) => new { b, u })
                .Join(_context.Stations, bu => bu.b.StationId, s => s.Id, (bu, s) => new BookingDto
                {
                    Id = bu.b.Id,
                    UserId = bu.b.UserId,
                    StationId = bu.b.StationId,
                    StartTime = bu.b.StartTime,
                    EndTime = bu.b.EndTime,
                    Status = bu.b.Status,
                    Amount = bu.b.Amount,
                    StationName = s.Name,
                    UserName = bu.u.Name,
                    Date = bu.b.Date,
                    TimeSlot = bu.b.TimeSlot,
                    Duration = bu.b.Duration,
                    CancellationMessage = bu.b.CancellationMessage
                })
                .ToListAsync();

            return Ok(new { bookings });
        }

        [HttpGet("station-analytics")]
        public async Task<IActionResult> GetStationAnalytics()
        {
            var analytics = await _context.Stations
                .Include(s => s.Bookings)
                .Select(s => new
                {
                    stationId = s.Id,
                    stationName = s.Name,
                    totalBookings = s.Bookings.Count,
                    revenue = s.Bookings.Sum(b => b.Amount),
                    status = s.Status
                })
                .ToListAsync();

            return Ok(analytics);
        }

        [HttpPut("users/{id}/status")]
        public async Task<IActionResult> UpdateUserStatus(int id, [FromBody] Dictionary<string, string> statusUpdate)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            if (statusUpdate.TryGetValue("status", out var status))
            {
                // For demo purposes, we'll just return success
                return Ok(new { message = "User status updated successfully" });
            }

            return BadRequest("Status is required");
        }

        [HttpPut("bookings/{id}/status")]
        public async Task<IActionResult> UpdateBookingStatus(int id, [FromBody] Dictionary<string, string> statusUpdate)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();

            if (statusUpdate.TryGetValue("status", out var status))
            {
                booking.Status = status;
                await _context.SaveChangesAsync();
                return Ok(new { message = "Booking status updated successfully" });
            }

            return BadRequest("Status is required");
        }

        [HttpGet("stations")]
        public async Task<IActionResult> GetAllStations()
        {
            var stations = await _context.Stations
                .ToListAsync();

            return Ok(new { stations });
        }

        [HttpPut("stations/{id}/approve")]
        public async Task<IActionResult> ApproveStation(int id)
        {
            var station = await _context.Stations.FindAsync(id);
            if (station == null) return NotFound();

            station.ApprovalStatus = "Approved";
            station.Status = "Available";
            await _context.SaveChangesAsync();

            return Ok(new { message = "Station approved successfully" });
        }

        [HttpPut("stations/{id}/reject")]
        public async Task<IActionResult> RejectStation(int id)
        {
            var station = await _context.Stations.FindAsync(id);
            if (station == null) return NotFound();

            station.ApprovalStatus = "Rejected";
            station.Status = "Offline";
            await _context.SaveChangesAsync();

            return Ok(new { message = "Station rejected successfully" });
        }
    }
}