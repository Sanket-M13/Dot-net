using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EVChargerAPI.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // LOCAL fallback connection string (only for EF tools)
            var connectionString =
                Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING")
                ?? "Server=localhost;Port=3306;Database=evcharger;User=root;Password=cdac;";

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            optionsBuilder.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString)
            );

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
