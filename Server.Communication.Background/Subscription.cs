using System.Threading.Channels;

namespace Server.Communication.Background;

public class Subscription<Subject> :IDisposable
{
    private readonly Channel<Subject> _channel;
    private readonly Action<Subscription<Subject>> _removeSubscription;
    
    public Subscription(Action<Subscription<Subject>> removeSubscription)
    {
        _channel = Channel.CreateBounded<Subject>(1);
        _removeSubscription = removeSubscription;
    }

    public void Dispose()
    {
        _removeSubscription(this);
    }

    public async ValueTask<Subject> ReadAsync(CancellationToken cancellationToken = default)
    {
        var subject = default(Subject);
        if (cancellationToken == null || !cancellationToken.IsCancellationRequested)
        {
            var canRead = await _channel.Reader.WaitToReadAsync();
            if (canRead)
            {
                subject = await _channel.Reader.ReadAsync();
            }
        }

        return subject;
    }

    internal async Task NotifyAsync(Subject subject, CancellationToken cancellationToken)
    {
        await _channel.Writer.WaitToWriteAsync(cancellationToken);
        _channel.Writer.TryWrite(subject);
    }

    internal void Close()
    {
        _channel.Writer.TryComplete();
    }
}