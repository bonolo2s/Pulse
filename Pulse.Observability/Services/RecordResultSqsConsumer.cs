using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pulse.Observability.Interfaces;

namespace Pulse.Observability.Services;

public class RecordResultSqsConsumer : BackgroundService
{
    private readonly IAmazonSQS _sqsClient;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly string _queueUrl;

    public RecordResultSqsConsumer(IAmazonSQS sqsClient, IServiceScopeFactory scopeFactory, IConfiguration config)
    {
        _sqsClient = sqsClient;
        _scopeFactory = scopeFactory;
        _queueUrl = config["Sqs:RecordResultQueueUrl"]!;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var response = await _sqsClient.ReceiveMessageAsync(new ReceiveMessageRequest
            {
                QueueUrl = _queueUrl,
                MaxNumberOfMessages = 10,
                WaitTimeSeconds = 20
            }, stoppingToken);

            foreach (var message in response.Messages)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var processor = scope.ServiceProvider.GetRequiredService<IRecordResultMessageProcessor>();
                    await processor.ProcessAsync(message.Body, stoppingToken);

                    await _sqsClient.DeleteMessageAsync(_queueUrl, message.ReceiptHandle, stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to process message {message.MessageId}: {ex.Message}");
                }
            }
        }
    }
}