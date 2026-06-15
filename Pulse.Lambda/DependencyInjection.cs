using Amazon.SimpleNotificationService;
using Microsoft.Extensions.DependencyInjection;
using Pulse.Infrastructure.Messaging;
using Pulse.Lambda.Interfaces;
using Pulse.Lambda.Services;
using Pulse.Shared.Interfaces;

namespace Pulse.Lambda;

public static class DependencyInjection
{
    public static IServiceCollection AddLambdaServices(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddSingleton<IAmazonSimpleNotificationService>(_ =>
        {
            var config = new AmazonSimpleNotificationServiceConfig
            {
                ServiceURL = "http://host.docker.internal:4566",
                AuthenticationRegion = "eu-west-1"
            };
            return new AmazonSimpleNotificationServiceClient("test", "test", config);//
        });
        services.AddSingleton<ISnsPublisher>(provider =>
        {
            var sns = provider.GetRequiredService<IAmazonSimpleNotificationService>();
            var topicArn = Environment.GetEnvironmentVariable("AWS__SNS__ALERTTOPICARN")
                ?? throw new InvalidOperationException("SNS_TOPIC_ARN not set.");
            return new SnsAlertPublisher(sns, topicArn);
        });
        services.AddSingleton<LatencyTracker>();
        services.AddSingleton<SslInspector>();
        services.AddScoped<IHealthCheckService, HealthCheckService>();
        services.AddScoped<IEndpointRepository>(provider =>
        {
            var connectionString = Environment.GetEnvironmentVariable("DB__CONNECTIONSTRING")
                ?? throw new InvalidOperationException("DB__CONNECTIONSTRING not set.");
            return new EndpointRepository(connectionString);
        });
        return services;
    }
}