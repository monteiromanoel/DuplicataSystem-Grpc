using Microsoft.EntityFrameworkCore;

namespace Duplicata.Infrastructure
{
    public class DuplicataDbContext : DbContext
    {
        public DbSet<Domain.Entities.Duplicata> Duplicatas { get; set; }

        public DuplicataDbContext(DbContextOptions options) : base(options) { }
    }

}
