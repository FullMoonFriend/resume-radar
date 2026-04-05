namespace ResumeRadar.Api.Models;

public class Analysis
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? JobTitle { get; set; }
    public string? Company { get; set; }
    public required string JobPostingText { get; set; }
    public required string StrengthMatches { get; set; }  // JSON array of strings
    public int MatchScore { get; set; }                    // 0-100
    public required string SkillGaps { get; set; }         // JSON array of {skill, note}
    public required string TailoredBullets { get; set; }   // JSON array of strings
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
}
