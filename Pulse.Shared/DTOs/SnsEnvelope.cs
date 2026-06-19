namespace Pulse.Shared.DTOs;

public class SnsEnvelope
{
    public string Type { get; set; } = null!;
    public string Message { get; set; } = null!;
    public string? SubscribeURL { get; set; }
}