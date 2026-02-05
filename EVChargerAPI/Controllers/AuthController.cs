using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using EVChargerAPI.Data;
using EVChargerAPI.Models;
using EVChargerAPI.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace EVChargerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized(new { message = "Invalid credentials" });
            
            var token = GenerateJwtToken(user);
            return Ok(new { token, user = new UserDto { Id = user.Id, Email = user.Email, Name = user.Name, Role = user.Role, Phone = user.Phone } });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest(new { message = "User already exists" });

            var user = new User
            {
                Email = dto.Email,
                Name = dto.Name,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Phone = dto.Phone,
                VehicleNumber = dto.VehicleNumber,
                VehicleType = dto.VehicleType,
                VehicleBrand = dto.VehicleBrand,
                VehicleModel = dto.VehicleModel,
                Role = string.IsNullOrEmpty(dto.Role) ? "User" : dto.Role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user);
            return Ok(new { token, user = new UserDto { Id = user.Id, Email = user.Email, Name = user.Name, Role = user.Role, Phone = user.Phone } });
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? "your-secret-key-here"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
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

        [HttpPost("google")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            
            if (user == null)
            {
                // First time Google user - create account automatically
                user = new User
                {
                    Email = dto.Email,
                    Name = dto.Name,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()),
                    Role = "User"
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            
            var token = GenerateJwtToken(user);
            return Ok(new { token, user = new UserDto { Id = user.Id, Email = user.Email, Name = user.Name, Role = user.Role, Phone = user.Phone } });
        }

        [HttpPost("google-register")]
        public async Task<IActionResult> GoogleRegister([FromBody] GoogleLoginDto dto)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (existingUser != null)
                return BadRequest(new { message = "User already exists" });
            
            var user = new User
            {
                Email = dto.Email,
                Name = dto.Name,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()),
                Role = "User"
            };
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user);
            return Ok(new { token, user = new UserDto { Id = user.Id, Email = user.Email, Name = user.Name, Role = user.Role, Phone = user.Phone } });
        }

        [HttpPost("refresh")]
        public IActionResult RefreshToken()
        {
            return Ok(new { message = "Token refreshed" });
        }
        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            return Ok(new { message = "Logged out successfully" });
        }
    }
}