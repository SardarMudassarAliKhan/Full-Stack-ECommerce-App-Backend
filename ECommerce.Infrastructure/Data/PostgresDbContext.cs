using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Data
{
    public class PostgresDbContext : AppDbContext
    {
        public PostgresDbContext(DbContextOptions<PostgresDbContext> options)
      : base(options)
        {
        }
    }

}
