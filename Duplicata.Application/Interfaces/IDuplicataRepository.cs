using Duplicata.Domain.Enums;

namespace Duplicata.Application.Interfaces
{
    public interface IDuplicataRepository
    {
        Task AddAsync(Domain.Entities.Duplicata duplicata);
        Task<Domain.Entities.Duplicata?> GetByIdAsync(Guid id);
        Task<List<Domain.Entities.Duplicata>> GetAllAsync();
        Task<Domain.Entities.Duplicata?> GetByNumberAsync(string numero);
        Task UpdateStatusAsync(Guid id, DuplicataStatus status);
    }
}
