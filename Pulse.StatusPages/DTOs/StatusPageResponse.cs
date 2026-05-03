namespace Pulse.StatusPages.DTOs;

public class StatusPageResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public DateTime CreatedAt { get; set; }
    public IEnumerable<StatusPageEndpointResponse> Endpoints { get; set; } = [];
}