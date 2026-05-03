namespace Pulse.StatusPages.DTOs;

public class CreateStatusPageRequest
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
}