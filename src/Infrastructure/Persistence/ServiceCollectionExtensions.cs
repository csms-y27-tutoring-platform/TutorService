using Application.Abstractions;
using Infrastructure.Persistence.Database;
using Infrastructure.Persistence.Database.Migrations;
using Infrastructure.Persistence.Database.Queries;
using Infrastructure.Persistence.Repositories;
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
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            IEnumerable<IConfigurationSection> allConnectionStrings = configuration.GetSection("ConnectionStrings").GetChildren();
            string availableConnections = string.Join(", ", allConnectionStrings.Select(c => c.Key));
            throw new InvalidOperationException(
                $"Database connection string 'DefaultConnection' is not configured. " +
                $"Available connection strings: {availableConnections}. " +
                "Please check your appsettings.json and ensure 'DefaultConnection' is set.");
        }

        Console.WriteLine($"Using database connection: {connectionString}");

        MigrationRunner.RunMigrations(connectionString);

        services.AddSingleton<IDatabaseConnectionFactory, ConnectionFactory>();

        services.AddScoped<ITutorQueries, NpgsqlTutorQueries>();
        services.AddScoped<IScheduleSlotQueries, NpgsqlScheduleSlotQueries>();
        services.AddScoped<ISubjectQueries, NpgsqlSubjectQueries>();

        services.AddScoped<ITutorRepository, TutorRepository>();
        services.AddScoped<IScheduleSlotRepository, ScheduleSlotRepository>();
        services.AddScoped<ISubjectRepository, SubjectRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<ITutorValidationService, TutorValidationService>();

        return services;
    }
}