using System.Text.Json.Serialization;

namespace Pulse.Lambda.Models;

public class HealthCheckEvent
{
    [JsonPropertyName("intervalSeconds")]
    public int IntervalSeconds { get; init; }
}