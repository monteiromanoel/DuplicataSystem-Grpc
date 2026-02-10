using Confluent.Kafka;
using System.Text.Json;
using Duplicata.Application.Interfaces;

public class KafkaEventPublisher : IEventPublisher
{
    private readonly IProducer<Null, string> _producer;

    public KafkaEventPublisher()
    {
        var config = new ProducerConfig
        {
            BootstrapServers = "localhost:29092"
        };

        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public async Task PublishAsync(string topic, object message)
    {
        var json = JsonSerializer.Serialize(message);
        await _producer.ProduceAsync(topic, new Message<Null, string> { Value = json });
    }
}
