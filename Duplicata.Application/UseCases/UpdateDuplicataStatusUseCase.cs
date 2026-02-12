using Duplicata.Application.Interfaces;
using Duplicata.Domain.Enums;

namespace Duplicata.Application.UseCases
{
    public class UpdateDuplicataStatusUseCase
    {
        private readonly IDuplicataRepository _repo;

        public UpdateDuplicataStatusUseCase(IDuplicataRepository repo)
        {
            _repo = repo;
        }

        public async Task ExecuteAsync(Guid id, DuplicataStatus status)
        {
            var duplicata = await _repo.GetByIdAsync(id);

            if (duplicata == null)
                throw new Exception("Duplicata não encontrada.");

            switch (status)
            {
                case DuplicataStatus.Registrada:
                    duplicata.Registrar();
                    break;

                case DuplicataStatus.Baixada:
                    duplicata.Baixar();
                    break;

                case DuplicataStatus.Cancelada:
                    duplicata.Cancelar();
                    break;
            }

            await _repo.UpdateStatusAsync(id, duplicata.Status);
        }

    }
}
