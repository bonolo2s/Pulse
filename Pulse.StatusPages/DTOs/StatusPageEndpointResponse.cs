namespace Pulse.StatusPages.DTOs;

public class StatusPageEndpointResponse
{
    public Guid Id { get; set; }
    public Guid StatusPageId { get; set; }
    public Guid EndpointId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}