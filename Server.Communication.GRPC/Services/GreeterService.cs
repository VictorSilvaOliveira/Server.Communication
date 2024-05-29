using System.Text.Json;
using Grpc.Core;
using Server.Communication.Background;

namespace Server.Communication.GRPC.Services;

public class GreeterService : Greeter.GreeterBase
{
    private readonly ILogger<GreeterService> _logger;
    private readonly Subscription<string> _subscription;

    public GreeterService(ILogger<GreeterService> logger, Subscription<string> subscription)
    {
        _logger = logger;
        _subscription = subscription;
    }

    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        return Task.FromResult(new HelloReply
        {
            Message = "Hello " + request.Name
        });
    }

    public override async Task<IServerStreamWriter<HelloReply>> SayHelloS(HelloRequest request, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
    {
        var name = request.Name;

        var response = new HelloReply()
        {
            Message = "Hello " + name
        };

        await responseStream.WriteAsync(response);

        while (!context.CancellationToken.IsCancellationRequested)
        {
            var responseRaw = await _subscription.ReadAsync(context.CancellationToken);
            if (responseRaw != null && !context.CancellationToken.IsCancellationRequested)
            {
                var json = JsonSerializer.Deserialize<JsonDocument>(responseRaw);
                response.Message = "Pong" + json.RootElement.GetProperty("controller");
                await responseStream.WriteAsync(response);
            }
        }

        return responseStream;
    }
}
