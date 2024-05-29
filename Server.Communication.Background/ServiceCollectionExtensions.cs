using Server.Communication.Background;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProducerBackgroundService(this IServiceCollection service)
    {
        service.AddHostedService<ProducerBackgroundService>();
        service.AddSingleton(typeof(Topic<>));
        service.AddScoped((serviceProvider) => 
        {
            var topic = serviceProvider.GetRequiredService<Topic<string>>();
            return topic.CreateSubscription();
        });

        return service;
    }
}