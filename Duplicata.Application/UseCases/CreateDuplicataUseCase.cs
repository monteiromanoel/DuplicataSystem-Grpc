using Duplicata.Application.DTOs;
using Duplicata.Application.Interfaces;
using Duplicata.Application.KafkaEvents;
using System;
using System.Collections.Generic;
using System.Text;

namespace Duplicata.Application.UseCases
{
    public class CreateDuplicataUseCase
    {
        private readonly IInMemoryDuplicataRepository _repo;
        private readonly IEventPublisher _publisher;

        public CreateDuplicataUseCase(IInMemoryDuplicataRepository repo, IEventPublisher publisher)
        {
            _repo = repo;
            _publisher = publisher;
        }

        public async Task<Guid> ExecuteAsync(CreateDuplicataDto dto)
        {
            var duplicata = new Domain.Entities.Duplicata(dto.Numero, dto.Valor, dto.Vencimento);
            await _repo.AddAsync(duplicata);

            await _publisher.PublishAsync(
                 "duplicata.created",
                 new DuplicataCreatedEvent(
                     duplicata.Id,
                     duplicata.Numero,
                     duplicata.Valor,
                     duplicata.Vencimento
                 )
             );

            return duplicata.Id;
        }
    }
}
