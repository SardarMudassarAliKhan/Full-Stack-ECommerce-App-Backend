using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Infrastructure.Data
{
    public class MySqlDbContextFactory : IDesignTimeDbContextFactory<MySqlDbContext>
    {
        public MySqlDbContext CreateDbContext(string[] args)
        {
            // Locate the API project's appsettings.json
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../ECommerce.API");
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var connectionString = configuration.GetConnectionString("MySQL")
                ?? throw new InvalidOperationException("MySQL connection string not found in appsettings.json");

            var optionsBuilder = new DbContextOptionsBuilder<MySqlDbContext>();
            // Use a fixed version instead of ServerVersion.AutoDetect to avoid connection attempts during design-time
            optionsBuilder.UseMySql(
                connectionString,
                new MySqlServerVersion(new Version(8, 0, 36))
            );

            return new MySqlDbContext(optionsBuilder.Options);
        }
    }

}
