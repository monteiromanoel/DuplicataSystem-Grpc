using Duplicata.Worker.Consumer;
using Duplicata.Worker.Consumer.Kafka;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<KafkaDuplicataConsumer>();
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();
