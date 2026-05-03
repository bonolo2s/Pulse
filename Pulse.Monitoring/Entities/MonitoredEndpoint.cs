public class MonitoredEndpoint
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
    public string Method { get; set; } // HTTP, HTTPS, TCP, DNS
    public int IntervalSeconds { get; set; }
    public int TimeoutMs { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}