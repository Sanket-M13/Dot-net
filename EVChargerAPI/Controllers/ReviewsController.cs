using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using EVChargerAPI.Data;
using EVChargerAPI.Models;
using EVChargerAPI.DTOs;
using System.Security.Claims;

namespace EVChargerAPI.Controllers
{
    [ApiController]
    [Route("api/reviews")]
    public class ReviewsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReviewsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllReviews()
        {
            try
            {
                var reviews = await _context.Reviews
                    .Join(_context.Users, r => r.UserId, u => u.Id, (r, u) => new { r, u })
                    .Join(_context.Stations, ru => ru.r.StationId, s => s.Id, (ru, s) => new {
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

        [HttpGet("station/{stationId}")]
        public async Task<IActionResult> GetStationReviews(int stationId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.StationId == stationId)
                .Include(r => r.User)
                .ToListAsync();

            return Ok(reviews);
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAdminReviews()
        {
            return Ok(new List<object>());
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            var review = new Review
            {
                UserId = userId,
                StationId = dto.StationId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Review created successfully" });
        }
    }
}