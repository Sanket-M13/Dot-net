using Microsoft.EntityFrameworkCore;
using EVChargerAPI.Models;

namespace EVChargerAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<VehicleBrand> VehicleBrands { get; set; }
        public DbSet<VehicleModel> VehicleModels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Station)
                .WithMany(s => s.Bookings)
                .HasForeignKey(b => b.StationId);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Station)
                .WithMany(s => s.Reviews)
                .HasForeignKey(r => r.StationId);

            modelBuilder.Entity<Car>()
                .HasOne(c => c.User)
                .WithMany(u => u.Cars)
                .HasForeignKey(c => c.UserId);
                
            modelBuilder.Entity<Station>()
                .HasOne(s => s.Owner)
                .WithMany()
                .HasForeignKey(s => s.OwnerId)
                .IsRequired(false);
                
            // Configure decimal precision
            modelBuilder.Entity<Booking>()
                .Property(b => b.Amount)
                .HasColumnType("decimal(18,2)");
                
            modelBuilder.Entity<Station>()
                .Property(s => s.PricePerKwh)
                .HasColumnType("decimal(18,2)");
        }
    }
}