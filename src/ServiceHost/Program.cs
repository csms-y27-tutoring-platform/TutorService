using Application;
using Infrastructure.Kafka;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Presentation.Grpc;
using Presentation.Kafka;

namespace ServiceHost;

internal static class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddApplication();
        builder.Services.AddPersistence(builder.Configuration);
        builder.Services.AddKafka(builder.Configuration);
        builder.Services.AddGrpcPresentation();
        builder.Services.AddKafkaPresentation();

        WebApplication app = builder.Build();

        app.MapGrpcService<Presentation.Grpc.Services.TutorGrpcService>();
        app.MapGrpcService<Presentation.Grpc.Services.ScheduleGrpcService>();
        app.MapGrpcService<Presentation.Grpc.Services.ValidationGrpcService>();

        app.Run();
    }
}