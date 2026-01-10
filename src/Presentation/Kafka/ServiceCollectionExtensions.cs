using Microsoft.Extensions.DependencyInjection;

namespace Presentation.Kafka;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKafkaPresentation(this IServiceCollection services)
    {
        return services;
    }
}