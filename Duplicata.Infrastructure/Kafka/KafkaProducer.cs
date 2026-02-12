using Confluent.Kafka;
using System.Text.Json;
using Duplicata.Application.Interfaces;
using Microsoft.Extensions.Options;

namespace Duplicata.Infrastructure.Kafka
{
    public class KafkaEventPublisher : IEventPublisher
    {
        private readonly IProducer<Null, string> _producer;

        public KafkaEventPublisher(IOptions<KafkaSettings> options)
        {
            var settings = options.Value;
            var config = new ProducerConfig
            {
                BootstrapServers = settings.BootstrapServers
            };
            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async Task PublishAsync(string topic, object message)
        {
            var json = JsonSerializer.Serialize(message);
            await _producer.ProduceAsync(topic, new Message<Null, string> { Value = json });
        }
    }
}
