using Duplicata.GrpcService;
using Grpc.Net.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// gRPC client
builder.Services.AddGrpcClient<DuplicataService.DuplicataServiceClient>(o =>
{
    o.Address = new Uri("https://localhost:7004"); // porta do GrpcService
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();
