using System.Net.WebSockets;

public abstract class WebSocketService
{
    public async Task<int> StartStream(WebSocketManager webSocketManager)
    {
        if (webSocketManager.IsWebSocketRequest)
        {
            using var ws  = await webSocketManager.AcceptWebSocketAsync();
            var cancellationToken = new CancellationTokenSource();

            var sendTask = SendMessageToSocket(ws, cancellationToken);

            await ReceiveMessageFromSocket(ws);

            await cancellationToken.CancelAsync();

            await sendTask.WaitAsync(cancellationToken.Token);

            await ws.CloseAsync(
                WebSocketCloseStatus.NormalClosure,
                "",
                CancellationToken.None
            );

            return StatusCodes.Status200OK;
        }
        else
        {
            return StatusCodes.Status400BadRequest;
        }
    }
    
    protected abstract Task SendMessageToSocket(WebSocket webSocket, CancellationTokenSource cancellationToken);

    protected abstract Task<WebSocketReceiveResult> ReceiveMessageFromSocket(WebSocket webSocket);

}