using Application;
using Infrastructure.Kafka;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Presentation.Grpc;
using Presentation.Kafka;

namespace ServiceHost;

internal class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'DefaultConnection' is not configured. " +
                $"Please check appsettings.json and appsettings.{builder.Environment.EnvironmentName}.json files.");
        }

        builder.Services.AddApplication();
        builder.Services.AddPersistence(builder.Configuration);
        builder.Services.AddKafka(builder.Configuration);
        builder.Services.AddGrpcPresentation();
        builder.Services.AddKafkaPresentation();

        builder.Services.AddGrpc(options =>
        {
            options.EnableDetailedErrors = builder.Environment.IsDevelopment();
        });

        WebApplication app = builder.Build();

        app.MapGrpcService<Presentation.Grpc.Services.TutorGrpcService>();
        app.MapGrpcService<Presentation.Grpc.Services.ScheduleGrpcService>();
        app.MapGrpcService<Presentation.Grpc.Services.ValidationGrpcService>();

        app.Run();
    }
}