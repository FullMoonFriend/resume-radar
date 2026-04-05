namespace ResumeRadar.Api.Models;

public class User
{
    public int Id { get; set; }
    public required string GitHubId { get; set; }
    public required string Username { get; set; }
    public string? AvatarUrl { get; set; }
    public string? ResumeText { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public List<Analysis> Analyses { get; set; } = [];
}
