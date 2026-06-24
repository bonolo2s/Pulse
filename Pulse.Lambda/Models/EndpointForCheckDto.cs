namespace Pulse.Lambda.Models;
public class EndpointForCheckDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string Url { get; init; } = string.Empty;
    public int IntervalSeconds { get; init; }
}