using Duplicata.Worker.Consumer;
using Duplicata.Worker.Consumer.Kafka;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<DuplicataCreationConsumer>();
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();
