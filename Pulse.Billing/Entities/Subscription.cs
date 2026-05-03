public class Subscription
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Plan { get; set; } // Free | Pro
    public int EndpointLimit { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; }
}