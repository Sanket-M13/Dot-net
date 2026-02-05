using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EVChargerAPI.Data;
using EVChargerAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// --------------------
// Controllers & Swagger
// --------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --------------------
// Database (MySQL ONLY)
// --------------------
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new Exception("Database connection string is missing.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    )
);

// --------------------
// Authentication (JWT)
// --------------------
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["JWT_ISSUER"],
            ValidAudience = builder.Configuration["JWT_AUDIENCE"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["JWT_SECRET_KEY"]
                    ?? throw new Exception("JWT_SECRET_KEY not configured")
                )
            )
        };
    });

// --------------------
// CORS
// --------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        var allowedOrigins = builder.Configuration
            .GetSection("AllowedOrigins")
            .Get<string[]>() 
            ?? new[]
            {
                "http://localhost:5173",
                "http://localhost:5174",
                "http://localhost:3000"
            };

        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// --------------------
// Middleware
// --------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// --------------------
// Data Seeding (SAFE)
// --------------------
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // ❌ DO NOT use EnsureCreated() in production

    // Seed admin user
    if (!context.Users.Any(u => u.Email == "admin@evcharger.com"))
    {
        context.Users.Add(new User
        {
            Email = "admin@evcharger.com",
            Name = "Admin User",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            Role = "Admin"
        });

        context.SaveChanges();
    }

    // Seed vehicle brands
    if (!context.VehicleBrands.Any())
    {
        context.VehicleBrands.AddRange(
            new VehicleBrand { Name = "Tata", Type = "Car" },
            new VehicleBrand { Name = "Mahindra", Type = "Car" },
            new VehicleBrand { Name = "MG", Type = "Car" },
            new VehicleBrand { Name = "Hyundai", Type = "Car" },
            new VehicleBrand { Name = "Kia", Type = "Car" },
            new VehicleBrand { Name = "BYD", Type = "Car" },
            new VehicleBrand { Name = "Ather", Type = "Bike" },
            new VehicleBrand { Name = "Ola Electric", Type = "Bike" },
            new VehicleBrand { Name = "TVS", Type = "Bike" },
            new VehicleBrand { Name = "Bajaj", Type = "Bike" },
            new VehicleBrand { Name = "Hero Electric", Type = "Bike" }
        );

        context.SaveChanges();
    }
}

// --------------------
app.Run();
