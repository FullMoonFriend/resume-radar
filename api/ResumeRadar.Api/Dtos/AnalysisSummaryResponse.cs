// api/ResumeRadar.Api/Dtos/AnalysisSummaryResponse.cs
namespace ResumeRadar.Api.Dtos;

public class AnalysisSummaryResponse
{
    public int Id { get; set; }
    public string? JobTitle { get; set; }
    public string? Company { get; set; }
    public int MatchScore { get; set; }
    public DateTime CreatedAt { get; set; }
}
