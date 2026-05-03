namespace Pulse.StatusPages.DTOs;

public class AddEndpointToStatusPageRequest
{
    public Guid StatusPageId { get; set; }
    public Guid EndpointId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}