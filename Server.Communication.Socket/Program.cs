using System.Net.Mime;
using Microsoft.AspNetCore.WebSockets;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddWebSockets(configure => { });
builder.Services.AddProducerBackgroundService();
builder.Services.AddScoped<WebSocketService, GreetSocketService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseWebSockets();
app.UseHttpsRedirection();

app.MapGet("webSocket", async (HttpContext context, WebSocketService wsService)=>
{
    context.Response.StatusCode = await wsService.StartStream(context.WebSockets);
});

app.Run();
