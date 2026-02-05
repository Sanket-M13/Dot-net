using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EVChargerAPI.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace EVChargerAPI.Controllers
{
    [ApiController]
    [Route("api/vehicles")]
    public class VehicleController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VehicleController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("brands")]
        public async Task<IActionResult> GetAllBrands()
        {
            var brands = await _context.VehicleBrands
                .Select(b => new { b.Id, b.Name, b.Type })
                .ToListAsync();

            return Ok(brands);
        }

        [HttpGet("brands/{type}")]
        public async Task<IActionResult> GetBrandsByType(string type)
        {
            var brands = await _context.VehicleBrands
                .Where(b => b.Type.ToLower() == type.ToLower())
                .Select(b => new { b.Id, b.Name })
                .ToListAsync();

            return Ok(brands);
        }

        [HttpGet("brands/{brandId}/models")]
        public async Task<IActionResult> GetModelsByBrand(int brandId)
        {
            var models = await _context.VehicleModels
                .Where(m => m.VehicleBrandId == brandId)
                .Select(m => new { m.Id, m.Name, Range = 300 }) // Default range, you can add this to your model
                .ToListAsync();

            return Ok(models);
        }

        [HttpPost("user-vehicle")]
        [Authorize]
        public async Task<IActionResult> SaveUserVehicle([FromBody] UserVehicleRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound("User not found");

            // Update user's vehicle information
            user.VehicleBrand = request.Brand;
            user.VehicleModel = request.Model;
            
            await _context.SaveChangesAsync();

            return Ok(new { message = "Vehicle data saved successfully" });
        }

        [HttpGet("user-vehicle")]
        [Authorize]
        public async Task<IActionResult> GetUserVehicle()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound("User not found");

            return Ok(new 
            {
                brand = user.VehicleBrand,
                model = user.VehicleModel
            });
        }
    }

    public class UserVehicleRequest
    {
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int BatteryPercent { get; set; }
        public int MaxRange { get; set; }
    }
}