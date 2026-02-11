namespace Duplicata.Application.Interfaces
{
    public interface IInMemoryDuplicataRepository
    {
        Task AddAsync(Domain.Entities.Duplicata duplicata);
        Task<Domain.Entities.Duplicata?> GetByIdAsync(Guid id);
        Task<List<Domain.Entities.Duplicata>> GetAllAsync();
        Task<Domain.Entities.Duplicata?> GetByNumberAsync(string numero);
    }
}
