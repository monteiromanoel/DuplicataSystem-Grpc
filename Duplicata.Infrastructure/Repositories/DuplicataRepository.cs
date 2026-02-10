using Microsoft.EntityFrameworkCore;
using Duplicata.Application.Interfaces;

namespace Duplicata.Infrastructure.Repositories
{
    public class DuplicataRepository
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
            return await _ctx.Duplicatas.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
