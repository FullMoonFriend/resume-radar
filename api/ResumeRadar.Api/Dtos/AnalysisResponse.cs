// api/ResumeRadar.Api/Dtos/AnalysisResponse.cs
using ResumeRadar.Api.Services;

namespace ResumeRadar.Api.Dtos;

public class AnalysisResponse
{
    public int Id { get; set; }
    public string? JobTitle { get; set; }
    public string? Company { get; set; }
    public int MatchScore { get; set; }
    public List<string> StrengthMatches { get; set; } = [];
    public List<SkillGap> SkillGaps { get; set; } = [];
    public List<string> TailoredBullets { get; set; } = [];
    public DateTime CreatedAt { get; set; }
}
