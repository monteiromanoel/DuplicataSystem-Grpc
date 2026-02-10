using Duplicata.Application.Interfaces;
using Duplicata.Application.UseCases;
using Duplicata.GrpcService.Services;
using Duplicata.Infrastructure;
using Duplicata.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();
builder.Services.AddScoped<IDuplicataRepository, InMemoryDuplicataRepository>();
builder.Services.AddScoped<CreateDuplicataUseCase>();
builder.Services.AddScoped<IEventPublisher, KafkaEventPublisher>();


var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGrpcService<DuplicataGrpcService>();
app.MapGrpcReflectionService();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
