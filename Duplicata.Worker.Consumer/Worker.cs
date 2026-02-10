using Duplicata.Worker.Consumer.Kafka;

namespace Duplicata.Worker.Consumer;

public class Worker : BackgroundService
{
    private readonly KafkaDuplicataConsumer _consumer;
    private readonly ILogger<Worker> _logger;

    public Worker(KafkaDuplicataConsumer consumer, ILogger<Worker> logger)
    {
        _consumer = consumer;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        return Task.Run(() => _consumer.StartConsuming(stoppingToken));
    }
}
