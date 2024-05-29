using System.Text;
using System.Net.WebSockets;
using Server.Communication.Background;

public class GreetSocketService : WebSocketService
{
    private Subscription<string> _subscription;

    public GreetSocketService(Subscription<string> subscription)
    {
        _subscription = subscription;
    }

    protected override async Task<WebSocketReceiveResult> ReceiveMessageFromSocket(WebSocket webSocket)
    {
        WebSocketReceiveResult receiveResult;
        do
        {
            var bytes = Encoding.UTF8.GetBytes("Hello World!");
            var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);

            var buffer = new byte[1024 * 4];

            receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer),
                CancellationToken.None
            );

        } while (!receiveResult.CloseStatus.HasValue);

        return receiveResult;
    }

    protected override Task SendMessageToSocket(WebSocket webSocket, CancellationTokenSource cancallationToken)
    {
        return Task.Run(async () =>
        {
            while(!cancallationToken.IsCancellationRequested)
            {
                var responseRaw = await _subscription.ReadAsync();
                var responseByte = Encoding.UTF8.GetBytes(responseRaw);
                await webSocket.SendAsync(
                    new ArraySegment<byte>(responseByte, 0, responseByte.Count()),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None
                );
            }
        }, cancallationToken.Token);
    }
}