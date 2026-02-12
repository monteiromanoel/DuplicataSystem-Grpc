using Microsoft.EntityFrameworkCore;
using Duplicata.Application.Interfaces;
using Duplicata.Infrastructure.Persistance;
using Duplicata.Domain.Enums;

namespace Duplicata.Infrastructure.Repositories
{
    public class DuplicataRepository : IDuplicataRepository
    {
        private readonly DuplicataDbContext _ctx;

        public DuplicataRepository(DuplicataDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task AddAsync(Domain.Entities.Duplicata duplicata)
        {
            _ctx.Duplicatas.Add(duplicata);
            await _ctx.SaveChangesAsync();
        }

        public async Task<Domain.Entities.Duplicata?> GetByIdAsync(Guid id)
        {
            return await _ctx.Duplicatas
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Domain.Entities.Duplicata>> GetAllAsync()
        {
            return await _ctx.Duplicatas.ToListAsync();
        }

        public async Task<Domain.Entities.Duplicata?> GetByNumberAsync(string numero)
        {
            return await _ctx.Duplicatas.FirstOrDefaultAsync(x => x.Numero == numero);
        }

        public async Task UpdateStatusAsync(Guid id, DuplicataStatus status)
        {
            await _ctx.Duplicatas
                .Where(x => x.Id == id)
                .ExecuteUpdateAsync(s =>
                    s.SetProperty(x => x.Status, status)
                );
        }


    }
}
