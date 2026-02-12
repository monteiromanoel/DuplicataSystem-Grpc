using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Duplicata.Infrastructure.Persistance
{
    public class DuplicataDbContextFactory : IDesignTimeDbContextFactory<DuplicataDbContext>
    {
        public DuplicataDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DuplicataDbContext>();

            optionsBuilder.UseNpgsql(
                "Host=localhost;Port=5432;Database=duplicata_db;Username=duplicata;Password=duplicata123"
            );

            return new DuplicataDbContext(optionsBuilder.Options);
        }
    }
}
