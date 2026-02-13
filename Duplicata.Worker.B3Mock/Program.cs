using Duplicata.Infrastructure.Kafka;
using Duplicata.Worker.B3Mock;
using Duplicata.Worker.B3Mock.Kafka;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection(KafkaSettings.SectionName));
builder.Services.Configure<B3MockOptions>(builder.Configuration.GetSection(B3MockOptions.SectionName));
builder.Services.AddSingleton<B3MockConsumer>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();

var bootstrapServers = builder.Configuration.GetSection(KafkaSettings.SectionName)["BootstrapServers"] ?? "localhost:29092";
await KafkaTopicEnsurer.EnsureTopicsAsync(bootstrapServers);

await host.RunAsync();
