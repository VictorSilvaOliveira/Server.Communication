namespace Server.Communication.Background;

public class Topic<Subject> : IDisposable
{
    private readonly List<Subscription<Subject>> _subscribers;

    public TopicStatus Status { get; private set; }

    public Topic()
    {
        Status = TopicStatus.Opened;
        _subscribers = new List<Subscription<Subject>>();
    }

    public void Dispose()
    {
        _subscribers.Clear();
    }

    public Subscription<Subject> CreateSubscription()
    {
        var subscription = new Subscription<Subject>(RemoveSubscription);
        _subscribers.Add(subscription);
        if (Status == TopicStatus.Closed)
        {
            subscription.Close();
        }
        return subscription;
    }

    public async ValueTask NotifyAsync(Subject subject, CancellationToken cancellationToken = default)
    {
        Status = TopicStatus.Opened;
        foreach (var subscriber in _subscribers)
        {
            await subscriber.NotifyAsync(subject, cancellationToken);
        }
    }

    private void RemoveSubscription(Subscription<Subject> subscription)
    {
        _subscribers.Remove(subscription);
    }

}
