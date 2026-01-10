using Application.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateTutorUseCase>();
        services.AddScoped<UpdateTutorUseCase>();
        services.AddScoped<GetTutorUseCase>();
        services.AddScoped<DeactivateTutorUseCase>();
        services.AddScoped<CreateScheduleSlotUseCase>();
        services.AddScoped<ValidateSlotUseCase>();
        services.AddScoped<ReserveSlotUseCase>();
        services.AddScoped<ReleaseSlotUseCase>();

        return services;
    }
}