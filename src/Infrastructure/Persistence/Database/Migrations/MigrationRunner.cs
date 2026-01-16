using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence.Database.Migrations;

public static class MigrationRunner
{
    public static void RunMigrations(string connectionString)
    {
        ServiceProvider serviceProvider = new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddPostgres()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(typeof(CreateInitialTables).Assembly).For.Migrations())
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            .BuildServiceProvider(false);

        using IServiceScope scope = serviceProvider.CreateScope();
        IMigrationRunner runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

        if (runner.HasMigrationsToApplyUp())
        {
            runner.MigrateUp();
        }
    }
}