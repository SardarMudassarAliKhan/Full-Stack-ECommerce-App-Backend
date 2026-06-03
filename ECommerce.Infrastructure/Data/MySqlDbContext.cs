using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Data
{
    public class MySqlDbContext : AppDbContext
    {
        public MySqlDbContext(DbContextOptions<MySqlDbContext> options)
            : base(options)
        {
        }
    }

}
