using System.Text.Json.Serialization;

namespace Pulse.Shared.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EndpointStatus
    {
        Operational,
        Degraded,
        Downtime
    }
}