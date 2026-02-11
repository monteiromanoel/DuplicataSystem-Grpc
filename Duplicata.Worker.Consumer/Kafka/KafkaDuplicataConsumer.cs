using Confluent.Kafka;
using System.Text.Json;

namespace Duplicata.Worker.Consumer.Kafka
{
    public class KafkaDuplicataConsumer
    {
        private readonly IConsumer<Ignore, string> _consumer;

        public KafkaDuplicataConsumer()
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:29092",
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
