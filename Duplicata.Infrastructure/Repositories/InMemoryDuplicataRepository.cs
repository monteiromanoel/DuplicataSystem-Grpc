using Duplicata.Application.Interfaces;

namespace Duplicata.Infrastructure.Repositories
{
    public class InMemoryDuplicataRepository : IDuplicataRepository
    {
        private static readonly List<Domain.Entities.Duplicata> _db = new();

        public Task AddAsync(Domain.Entities.Duplicata duplicata)
        {
            _db.Add(duplicata);
            return Task.CompletedTask;
        }

        public Task<Domain.Entities.Duplicata?> GetByIdAsync(Guid id)
        {
            return Task.FromResult(_db.FirstOrDefault(x => x.Id == id));
        }

        public Task<List<Domain.Entities.Duplicata>> GetAllAsync()
        {
            return Task.FromResult(_db.ToList());
        }
    }
}
