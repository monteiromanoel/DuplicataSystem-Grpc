using Confluent.Kafka;
using Duplicata.Infrastructure.Kafka;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Duplicata.Worker.Consumer.Kafka
{
    public class DuplicataCreationConsumer
    {
        private readonly IConsumer<Ignore, string> _consumer;

        public DuplicataCreationConsumer(IOptions<KafkaSettings> options)
        {
            var settings = options.Value;
            var config = new ConsumerConfig
            {
                BootstrapServers = settings.BootstrapServers,
                GroupId = "duplicata-worker-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        }

        public void StartConsuming(CancellationToken stoppingToken)
        {
            _consumer.Subscribe("duplicata.created");

            while (!stoppingToken.IsCancellationRequested)
            {
                var result = _consumer.Consume(stoppingToken);
                Console.WriteLine($"CONSUMER: EVENT RECEIVED: {result.Message.Value}");

                var duplicata = JsonSerializer.Deserialize<dynamic>(result.Message.Value);
                Console.WriteLine($"CONSUMER: Duplicata processada: {duplicata}");
            }
        }
    }
}
