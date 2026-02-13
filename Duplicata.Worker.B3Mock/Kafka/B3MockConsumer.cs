using Confluent.Kafka;
using Duplicata.Application.KafkaEvents;
using Duplicata.Infrastructure.Kafka;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Duplicata.Worker.B3Mock.Kafka
{
    /// <summary>
    /// Simula o processo da B3: consome duplicata.created, simula processamento
    /// e publica eventos de status (registered, paid, rejected) para o Worker.Status consumir.
    /// </summary>
    public class B3MockConsumer
    {
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly IProducer<Null, string> _producer;
        private readonly ILogger<B3MockConsumer> _logger;
        private readonly B3MockOptions _options;

        public B3MockConsumer(
            IOptions<KafkaSettings> kafkaOptions,
            IOptions<B3MockOptions> b3Options,
            ILogger<B3MockConsumer> logger)
        {
            var settings = kafkaOptions.Value;
            _options = b3Options.Value;
            _logger = logger;

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = settings.BootstrapServers,
                GroupId = "duplicata-b3-mock-worker",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                MaxPollIntervalMs = 600000
            };
            _consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();

            var producerConfig = new ProducerConfig
            {
                BootstrapServers = settings.BootstrapServers
            };
            _producer = new ProducerBuilder<Null, string>(producerConfig).Build();
        }

        public void Start(CancellationToken token)
        {
            _consumer.Subscribe("duplicata.created");
            _logger.LogInformation("B3Mock consumindo duplicata.created (simulando B3)");

            while (!token.IsCancellationRequested)
            {
                var msg = _consumer.Consume(token);
                _logger.LogInformation("📥 Recebido duplicata.created: {Value}", msg.Message.Value);

                try
                {
                    var evt = JsonSerializer.Deserialize<DuplicataCreatedEvent>(msg.Message.Value);
                    if (evt != null)
                    {
                        ProcessAndPublishAsync(evt, token).GetAwaiter().GetResult();
                    }
                    else
                    {
                        _logger.LogWarning("Evento inválido ou nulo, ignorando");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar duplicata.created");
                }
            }
        }

        private async Task ProcessAndPublishAsync(DuplicataCreatedEvent evt, CancellationToken token)
        {
            // Simula tempo de processamento da B3
            var delayMs = Random.Shared.Next(_options.MinDelayMs, _options.MaxDelayMs + 1);
            _logger.LogInformation("⏳ Simulando B3: processando duplicata {Id} por {Delay}ms", evt.Id, delayMs);
            await Task.Delay(delayMs, token);

            // Escolhe resultado simulado
            var topic = ChooseResultTopic();
            var statusEvent = new { evt.Id };

            await _producer.ProduceAsync(
                topic,
                new Message<Null, string> { Value = JsonSerializer.Serialize(statusEvent) },
                token);

            _logger.LogInformation("✅ B3Mock publicou {Topic} para duplicata {Id}", topic, evt.Id);
        }

        private string ChooseResultTopic()
        {
            var rnd = Random.Shared.NextDouble();
            if (rnd < _options.RegisteredProbability)
                return "duplicata.registered";
            if (rnd < _options.RegisteredProbability + _options.PaidProbability)
                return "duplicata.paid";
            return "duplicata.rejected";
        }
    }

    public class B3MockOptions
    {
        public const string SectionName = "B3Mock";
        public int MinDelayMs { get; set; } = 2000;
        public int MaxDelayMs { get; set; } = 5000;
        public double RegisteredProbability { get; set; } = 0.7;
        public double PaidProbability { get; set; } = 0.1;
    }
}
