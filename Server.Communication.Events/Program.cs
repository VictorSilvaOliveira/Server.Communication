using Server.Communication.Background;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProducerBackgroundService();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();{}
app.MapGet("eventServer", async(HttpContext context, Subscription<string> subscription, CancellationToken cancellationToken)=>
{
    var response = context.Response;
    response.Headers.ContentType = "text/event-stream";

    while (!cancellationToken.IsCancellationRequested)
    {
        var text = await subscription.ReadAsync();
        await response.WriteAsync($"data: {text}\r\r");
        await response.Body.FlushAsync();
    }
});

app.Run();
