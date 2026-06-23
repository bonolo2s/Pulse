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
        _queueUrl = config["Aws:Sqs:RecordResultQueueUrl"]!;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine($"[RecordResultSqsConsumer] Starting poll loop on {_queueUrl}");

        while (!stoppingToken.IsCancellationRequested)
        {
            var response = await _sqsClient.ReceiveMessageAsync(new ReceiveMessageRequest
            {
                QueueUrl = _queueUrl,
                MaxNumberOfMessages = 10,
                WaitTimeSeconds = 20
            }, stoppingToken);

            if (response.Messages.Count == 0)
            {
                Console.WriteLine("[RecordResultSqsConsumer] Poll returned no messages.");//
            }

            foreach (var message in response.Messages)
            {
                Console.WriteLine($"[RecordResultSqsConsumer] Received message {message.MessageId}");//
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var processor = scope.ServiceProvider.GetRequiredService<IRecordResultMessageProcessor>();

                    Console.WriteLine($"[RecordResultSqsConsumer] Processing message {message.MessageId}");//
                    await processor.ProcessAsync(message.Body, stoppingToken);
                    Console.WriteLine($"[RecordResultSqsConsumer] Processed message {message.MessageId}");//

                    await _sqsClient.DeleteMessageAsync(_queueUrl, message.ReceiptHandle, stoppingToken);
                    Console.WriteLine($"[RecordResultSqsConsumer] Deleted message {message.MessageId}");//
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[RecordResultSqsConsumer] Failed to process message {message.MessageId}: {ex.Message}");
                }
            }
        }
    }
}