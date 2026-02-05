using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using EVChargerAPI.Data;
using EVChargerAPI.Models;
using System.Security.Claims;

namespace EVChargerAPI.Controllers
{
    [ApiController]
    [Route("api/cars")]
    [Authorize]
    public class CarsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CarsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCars()
        {
            var cars = await _context.Cars.ToListAsync();
            return Ok(cars);
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetUserCars()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var cars = await _context.Cars.Where(c => c.UserId == userId).ToListAsync();
            return Ok(cars);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCar(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null) return NotFound();
            return Ok(car);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCar(Car car)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            car.UserId = userId;
            
            _context.Cars.Add(car);
            await _context.SaveChangesAsync();
            
            return Ok(car);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCar(int id, Car car)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var existingCar = await _context.Cars.FindAsync(id);
            
            if (existingCar == null) return NotFound();
            if (existingCar.UserId != userId) return Forbid();
            
            existingCar.Brand = car.Brand;
            existingCar.Model = car.Model;
            existingCar.VehicleNumber = car.VehicleNumber;
            existingCar.Range = car.Range;
            
            await _context.SaveChangesAsync();
            return Ok(existingCar);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var car = await _context.Cars.FindAsync(id);
            
            if (car == null) return NotFound();
            if (car.UserId != userId) return Forbid();
            
            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();
            
            return Ok(new { message = "Car deleted successfully" });
        }
    }
}