using Server.Communication.GRPC.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddProducerBackgroundService();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseGrpcWeb();
app.MapGrpcService<GreeterService>().EnableGrpcWeb();

app.Run();
