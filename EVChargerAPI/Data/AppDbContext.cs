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
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Email)
                      .HasColumnType("varchar(255)")
                      .IsRequired();

                entity.Property(e => e.Name)
                      .HasColumnType("varchar(255)")
                      .IsRequired();

                entity.Property(e => e.PasswordHash)
                      .HasColumnType("varchar(255)")
                      .IsRequired();

                entity.Property(e => e.Phone)
                      .HasColumnType("varchar(20)");

                entity.Property(e => e.VehicleNumber)
                      .HasColumnType("varchar(50)");

                entity.Property(e => e.VehicleType)
                      .HasColumnType("varchar(50)");

                entity.Property(e => e.VehicleBrand)
                      .HasColumnType("varchar(50)");

                entity.Property(e => e.VehicleModel)
                      .HasColumnType("varchar(50)");

                entity.Property(e => e.Role)
                      .HasColumnType("varchar(50)")
                      .IsRequired();

                entity.Property(e => e.CreatedAt)
                      .HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt)
                      .HasColumnType("datetime");
            });
        }

    }
}