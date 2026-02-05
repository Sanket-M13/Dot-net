using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using EVChargerAPI.Data;
using EVChargerAPI.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace EVChargerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users.Select(u => new UserDto
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
            }).ToListAsync();

            return Ok(users);
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _context.Users.FindAsync(userId);
            
            if (user == null)
                return NotFound();

            return Ok(new UserDto 
            { 
                Id = user.Id, 
                Email = user.Email, 
                Name = user.Name, 
                Role = user.Role, 
                Phone = user.Phone,
                VehicleNumber = user.VehicleNumber,
                VehicleType = user.VehicleType,
                VehicleBrand = user.VehicleBrand,
                VehicleModel = user.VehicleModel
            });
        }

        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _context.Users.FindAsync(userId);
            
            if (user == null)
                return NotFound();

            user.Name = dto.Name ?? user.Name;
            user.Phone = dto.Phone ?? user.Phone;
            user.VehicleNumber = dto.VehicleNumber ?? user.VehicleNumber;
            user.VehicleType = dto.VehicleType ?? user.VehicleType;
            user.VehicleBrand = dto.VehicleBrand ?? user.VehicleBrand;
            user.VehicleModel = dto.VehicleModel ?? user.VehicleModel;

            await _context.SaveChangesAsync();

            return Ok(new UserDto 
            { 
                Id = user.Id, 
                Email = user.Email, 
                Name = user.Name, 
                Role = user.Role, 
                Phone = user.Phone,
                VehicleNumber = user.VehicleNumber,
                VehicleType = user.VehicleType,
                VehicleBrand = user.VehicleBrand,
                VehicleModel = user.VehicleModel
            });
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _context.Users.FindAsync(userId);
            
            if (user == null)
                return NotFound();

            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
                return BadRequest(new { message = "Current password is incorrect" });

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Password changed successfully" });
        }

        [HttpPut("{id}/password")]
        [Authorize]
        public async Task<IActionResult> ChangeUserPassword(int id, ChangePasswordDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId != id) return Forbid();
            
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            
            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
                return BadRequest(new { message = "Current password is incorrect" });
            
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _context.SaveChangesAsync();
            
            return Ok(new { message = "Password changed successfully" });
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            return Ok(new UserDto 
            { 
                Id = user.Id, 
                Email = user.Email, 
                Name = user.Name, 
                Role = user.Role, 
                Phone = user.Phone,
                VehicleNumber = user.VehicleNumber,
                VehicleType = user.VehicleType,
                VehicleBrand = user.VehicleBrand,
                VehicleModel = user.VehicleModel
            });
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(int id, UpdateProfileDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.Name = dto.Name ?? user.Name;
            user.Phone = dto.Phone ?? user.Phone;
            user.VehicleNumber = dto.VehicleNumber ?? user.VehicleNumber;
            user.VehicleType = dto.VehicleType ?? user.VehicleType;
            user.VehicleBrand = dto.VehicleBrand ?? user.VehicleBrand;
            user.VehicleModel = dto.VehicleModel ?? user.VehicleModel;

            await _context.SaveChangesAsync();

            return Ok(new UserDto 
            { 
                Id = user.Id, 
                Email = user.Email, 
                Name = user.Name, 
                Role = user.Role, 
                Phone = user.Phone,
                VehicleNumber = user.VehicleNumber,
                VehicleType = user.VehicleType,
                VehicleBrand = user.VehicleBrand,
                VehicleModel = user.VehicleModel
            });
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User deleted successfully" });
        }
    }
}