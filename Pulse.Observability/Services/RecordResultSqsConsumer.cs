using Amazon.SQS;
using Amazon.SQS.Model;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pulse.Observability.Commands;
using Pulse.Observability.Entities;
using Pulse.Shared.DTOs;
using System.Text.Json;

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
                MaxNumberOfMessages = 10,//
                WaitTimeSeconds = 20 
            }, stoppingToken);

            foreach (var message in response.Messages)
            {
                try
                {
                    var envelope = JsonSerializer.Deserialize<SnsEnvelope>(message.Body);
                    var dto = JsonSerializer.Deserialize<AlertNotificationDto>(envelope!.Message);

                    var result = new CheckResult
                    {
                        EndpointId = dto!.Result.EndpointId,
                        Status = dto.Result.Status,
                        StatusCode = dto.Result.StatusCode,
                        LatencyMs = dto.Result.LatencyMs,
                        SslIssuer = dto.Result.SslIssuer,
                        SslExpiresAt = dto.Result.SslExpiresAt,
                        SslDaysRemaining = dto.Result.SslDaysRemaining,
                        ErrorMessage = dto.Result.ErrorMessage
                    };

                    using var scope = _scopeFactory.CreateScope();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    await mediator.Send(new RecordCheckResultCommand(result), stoppingToken);

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