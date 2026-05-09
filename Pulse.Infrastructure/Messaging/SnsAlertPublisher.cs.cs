using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Pulse.Shared.DTOs;
using Pulse.Shared.Interfaces;
using System.Text.Json;

namespace Pulse.Infrastructure.Messaging;

public class SnsAlertPublisher : ISnsPublisher
{
    private readonly IAmazonSimpleNotificationService _sns;
    private readonly string _topicArn;

    public SnsAlertPublisher(IAmazonSimpleNotificationService sns, string topicArn)
    {
        _sns = sns;
        _topicArn = topicArn;
    }

    public async Task PublishAlertAsync(AlertNotificationDto message)
    {
        var request = new PublishRequest
        {
            TopicArn = _topicArn,
            Message = JsonSerializer.Serialize(message),
            Subject = $"Pulse Alert: Endpoint {message.EndpointId}",
            MessageAttributes = new Dictionary<string, MessageAttributeValue>
            {
                ["status"] = new MessageAttributeValue
                {
                    DataType = "String",
                    StringValue = message.Result.Status.ToString()
                }
            }
        };

        await _sns.PublishAsync(request);
    }
}