using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Infrastructure.Data
{
    public class PostgresDbContextFactory : IDesignTimeDbContextFactory<PostgresDbContext>
    {
        public PostgresDbContext CreateDbContext(string[] args)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../ECommerce.API");
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var connectionString = configuration.GetConnectionString("PostgreSQL");

            var optionsBuilder = new DbContextOptionsBuilder<PostgresDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new PostgresDbContext(optionsBuilder.Options);
        }
    }
}
