using Confluent.Kafka;
using Duplicata.Worker.Status.Kafka;

namespace Duplicata.Worker.Status;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly DuplicataStatusConsumer _consumer;

    public Worker(ILogger<Worker> logger, DuplicataStatusConsumer consumer)
    {
        _logger = logger;
        _consumer = consumer;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker DuplicataStatusConsumer iniciado em: {time}", DateTimeOffset.Now);
        _consumer.Start(stoppingToken);
        return Task.CompletedTask;
    }
}
