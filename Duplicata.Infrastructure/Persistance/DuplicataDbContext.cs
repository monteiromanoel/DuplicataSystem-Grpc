using Microsoft.EntityFrameworkCore;

namespace Duplicata.Infrastructure.Persistance
{
    public class DuplicataDbContext : DbContext
    {
        public DbSet<Domain.Entities.Duplicata> Duplicatas { get; set; }

        public DuplicataDbContext(DbContextOptions<DuplicataDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Domain.Entities.Duplicata>(e =>
            {
                e.HasKey(x => x.Id);

                e.Property(x => x.Numero)
                    .IsRequired()
                    .HasMaxLength(50);

                e.HasIndex(x => x.Numero).IsUnique();

                e.Property(x => x.Valor)
                    .HasPrecision(18, 2);

                e.Property(x => x.Vencimento)
                    .IsRequired();

                e.Property(x => x.CriadoEm)
                    .IsRequired();

                e.Property(x => x.Status)
                    .HasConversion<int>();
            });
        }
    }

}
