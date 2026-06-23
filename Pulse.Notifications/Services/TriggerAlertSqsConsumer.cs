using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pulse.Notifications.Interfaces;

namespace Pulse.Notifications.Services;

public class TriggerAlertSqsConsumer : BackgroundService
{
    private readonly IAmazonSQS _sqsClient;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly string _queueUrl;

    public TriggerAlertSqsConsumer(IAmazonSQS sqsClient, IServiceScopeFactory scopeFactory, IConfiguration config)
    {
        _sqsClient = sqsClient;
        _scopeFactory = scopeFactory;
        _queueUrl = config["Aws:Sqs:TriggerAlertQueueUrl"]!;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine($"[TriggerAlertSqsConsumer] Starting poll loop on {_queueUrl}");

        while (!stoppingToken.IsCancellationRequested)
        {
            var response = await _sqsClient.ReceiveMessageAsync(new ReceiveMessageRequest
            {
                QueueUrl = _queueUrl,
                MaxNumberOfMessages = 10,
                WaitTimeSeconds = 20
            }, stoppingToken);

            if (response.Messages == null || !response.Messages.Any()) continue;

            foreach (var message in response.Messages)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var processor = scope.ServiceProvider.GetRequiredService<ITriggerAlertMessageProcessor>();
                    await processor.ProcessAsync(message.Body, stoppingToken);
                    await _sqsClient.DeleteMessageAsync(_queueUrl, message.ReceiptHandle, stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[TriggerAlertSqsConsumer] Failed to process message {message.MessageId}: {ex.Message}");
                }
            }
        }
    }
}