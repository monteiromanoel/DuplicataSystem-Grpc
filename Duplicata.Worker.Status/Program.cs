using Duplicata.Application.Interfaces;
using Duplicata.Application.UseCases;
using Duplicata.Infrastructure.Kafka;
using Duplicata.Infrastructure.Persistance;
using Duplicata.Infrastructure.Repositories;
using Duplicata.Worker.Status;
using Duplicata.Worker.Status.Kafka;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection(KafkaSettings.SectionName));
builder.Services.AddDbContext<DuplicataDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DuplicataDb"))
);

builder.Services.AddSingleton<IDuplicataRepository, DuplicataRepository>();
builder.Services.AddScoped<UpdateDuplicataStatusUseCase>();
builder.Services.AddSingleton<DuplicataStatusConsumer>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
