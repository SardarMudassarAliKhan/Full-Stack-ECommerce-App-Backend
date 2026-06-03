using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Infrastructure.Data
{
    public class SqlServerDbContextFactory : IDesignTimeDbContextFactory<SqlServerDbContext>
    {
        public SqlServerDbContext CreateDbContext(string[] args)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../ECommerce.API");
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var connectionString = configuration.GetConnectionString("SqlServer")
                ?? throw new InvalidOperationException("SqlServer connection string not found in appsettings.json");

            var optionsBuilder = new DbContextOptionsBuilder<SqlServerDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new SqlServerDbContext(optionsBuilder.Options);
        }
    }
}
