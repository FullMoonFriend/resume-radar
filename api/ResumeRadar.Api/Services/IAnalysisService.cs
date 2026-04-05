// api/ResumeRadar.Api/Services/IAnalysisService.cs
namespace ResumeRadar.Api.Services;

public record SkillGap(string Skill, string Note);

public record AnalysisResult(
    int MatchScore,
    List<string> StrengthMatches,
    List<SkillGap> SkillGaps,
    List<string> TailoredBullets
);

public interface IAnalysisService
{
    Task<AnalysisResult> AnalyzeAsync(string resumeText, string jobPostingText);
}
