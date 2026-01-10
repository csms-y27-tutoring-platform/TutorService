using Microsoft.Extensions.DependencyInjection;

namespace Presentation.Grpc;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGrpcPresentation(this IServiceCollection services)
    {
        services.AddGrpc();
        services.AddGrpcReflection();

        services.AddScoped<Services.TutorGrpcService>();
        services.AddScoped<Services.ScheduleGrpcService>();
        services.AddScoped<Services.ValidationGrpcService>();

        return services;
    }
}