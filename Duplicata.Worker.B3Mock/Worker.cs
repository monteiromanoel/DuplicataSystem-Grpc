using Duplicata.Worker.B3Mock.Kafka;

namespace Duplicata.Worker.B3Mock;

public class Worker : BackgroundService
{
    private readonly B3MockConsumer _consumer;
    private readonly ILogger<Worker> _logger;

    public Worker(B3MockConsumer consumer, ILogger<Worker> logger)
    {
        _consumer = consumer;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker B3Mock iniciado - simulando respostas da B3");
        return Task.Run(() => _consumer.Start(stoppingToken), stoppingToken);
    }
}
