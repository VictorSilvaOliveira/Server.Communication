using Microsoft.Extensions.Hosting;

namespace Server.Communication.Background;

public class ProducerBackgroundService: BackgroundService
{
    private readonly Topic<string> _topic;

    public ProducerBackgroundService(Topic<string> topic)
    {
        _topic= topic;
    }

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        return Task.Run(async () => 
        {
            for(var i = 0; !cancellationToken.IsCancellationRequested && i < 20; i++)
            {
                var message = $"{{ \"controller\": {i}, \"dateTime\": \"{DateTime.Now}\" }}";
                await _topic.NotifyAsync(message);
                await Task.Delay(5 * 1000);
            }
        });
    }
}