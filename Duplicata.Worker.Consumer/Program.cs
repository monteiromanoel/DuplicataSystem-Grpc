using Duplicata.Infrastructure.Kafka;
using Duplicata.Worker.Consumer;
using Duplicata.Worker.Consumer.Kafka;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.Configure<KafkaSettings>(context.Configuration.GetSection(KafkaSettings.SectionName));
        services.AddSingleton<DuplicataCreationConsumer>();
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();
