public class StatusPage
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty; // URL-friendly version of Name e.g. "my-production-api"
    public bool IsPublic { get; set; } // true = accessible via public slug URL, false = owner only by page id
    public DateTime CreatedAt { get; set; }
    public ICollection<StatusPageEndpoint> Endpoints { get; set; } = [];
}