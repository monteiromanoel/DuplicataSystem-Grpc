using Confluent.Kafka;
using Duplicata.Application.UseCases;
using Duplicata.Domain.Enums;
using Duplicata.Infrastructure.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Duplicata.Worker.Status.Kafka
{
    public class DuplicataStatusConsumer
    {
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly IServiceScopeFactory _scopeFactory;

        public DuplicataStatusConsumer(IServiceScopeFactory scopeFactory, IOptions<KafkaSettings> options)
        {
            _scopeFactory = scopeFactory;
            var settings = options.Value;

            var config = new ConsumerConfig
            {
                BootstrapServers = settings.BootstrapServers,
                GroupId = "duplicata-status-worker",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        }

        public void Start(CancellationToken token)
        {
            _consumer.Subscribe(new[]
            {
                "duplicata.registered",
                "duplicata.rejected",
                "duplicata.paid"
            });

            while (!token.IsCancellationRequested)
            {
                var msg = _consumer.Consume(token);
                Console.WriteLine($"📩 EVENTO STATUS: {msg.Topic}");

                var evt = JsonSerializer.Deserialize<DuplicataEvent>(msg.Message.Value);

                var status = msg.Topic switch
                {
                    "duplicata.registered" => DuplicataStatus.Registrada,
                    "duplicata.paid" => DuplicataStatus.Baixada,
                    "duplicata.rejected" => DuplicataStatus.Cancelada,
                    _ => throw new Exception("Evento desconhecido")
                };

                using var scope = _scopeFactory.CreateScope();
                var useCase = scope.ServiceProvider.GetRequiredService<UpdateDuplicataStatusUseCase>();
                useCase.ExecuteAsync(evt.Id, status).Wait();
            }
        }
    }

    public record DuplicataEvent(Guid Id);
}
