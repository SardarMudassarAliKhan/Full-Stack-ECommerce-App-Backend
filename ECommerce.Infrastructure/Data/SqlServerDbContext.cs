using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Data
{
    public class SqlServerDbContext : AppDbContext
    {
        public SqlServerDbContext(DbContextOptions<SqlServerDbContext> options)
            : base(options)
        {
        }
    }
}
