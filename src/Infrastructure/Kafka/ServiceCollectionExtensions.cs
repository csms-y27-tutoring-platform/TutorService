using Application.Abstractions;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Kafka;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKafka(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        IConfigurationSection kafkaConfig = configuration.GetSection("Kafka");
        string? bootstrapServers = kafkaConfig["BootstrapServers"];

        services.AddSingleton<IProducer<Null, byte[]>>(sp =>
        {
            var config = new ProducerConfig
            {
                BootstrapServers = bootstrapServers,
                Acks = Acks.All,
                EnableIdempotence = true,
                MessageSendMaxRetries = 3,
                RetryBackoffMs = 1000,
            };

            return new ProducerBuilder<Null, byte[]>(config).Build();
        });

        services.AddScoped<ITutorEventPublisher, Tutors.EventPublisher>();

        return services;
    }
}