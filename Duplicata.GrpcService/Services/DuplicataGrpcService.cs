using Duplicata.Application.DTOs;
using Duplicata.Application.Interfaces;
using Duplicata.Application.UseCases;
using Grpc.Core;

namespace Duplicata.GrpcService.Services
{
    public class DuplicataGrpcService : DuplicataService.DuplicataServiceBase
    {
        private readonly CreateDuplicataUseCase _useCase;
        private readonly IDuplicataRepository _repo;

        public DuplicataGrpcService(CreateDuplicataUseCase useCase, IDuplicataRepository repo)
        {
            _useCase = useCase;
            _repo = repo;
        }

        public override async Task<DuplicataResponse> CreateDuplicata(CreateDuplicataRequest request, ServerCallContext context)
        {
            var id = await _useCase.ExecuteAsync(new CreateDuplicataDto
            {
                Numero = request.Numero,
                Valor = (decimal)request.Valor,
                Vencimento = DateTime.Parse(request.Vencimento)
            });

            return new DuplicataResponse { Id = id.ToString() };
        }

        public override async Task<DuplicataListResponse> GetDuplicatas(Empty request, ServerCallContext context)
        {
            var duplicatas = await _repo.GetAllAsync();
            var response = new DuplicataListResponse();

            response.Duplicatas.AddRange(duplicatas.Select(d => new DuplicataDto
            {
                Id = d.Id.ToString(),
                Numero = d.Numero,
                Valor = (double)d.Valor,
                Vencimento = d.Vencimento.ToString("O")
            }));

            return response;
        }
    }


}
