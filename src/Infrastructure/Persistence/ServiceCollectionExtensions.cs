using Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<PersistenceContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<ITutorRepository, Repositories.TutorRepository>();
        services.AddScoped<IScheduleSlotRepository, Repositories.ScheduleSlotRepository>();
        services.AddScoped<ISubjectRepository, Repositories.SubjectRepository>();
        services.AddScoped<ITutorValidationService, TutorValidationService>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}