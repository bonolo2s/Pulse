namespace Pulse.StatusPages.DTOs;

public class UpdateStatusPageRequest
{
    public string Name { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
}