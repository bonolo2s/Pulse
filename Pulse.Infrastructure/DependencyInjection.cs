using Amazon.SimpleNotificationService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pulse.Infrastructure.Messaging;
using Pulse.Shared.Interfaces;

namespace Pulse.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAWSService<IAmazonSimpleNotificationService>();
        services.AddSingleton<ISnsPublisher>(sp =>
        {
            var sns = sp.GetRequiredService<IAmazonSimpleNotificationService>();
            var topicArn = configuration["AWS__SNS__ALERTTOPICARN"]!;// where to
            return new SnsAlertPublisher(sns, topicArn);
        });

        return services;
    }
}