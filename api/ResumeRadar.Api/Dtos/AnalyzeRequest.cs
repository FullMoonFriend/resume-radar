// api/ResumeRadar.Api/Dtos/AnalyzeRequest.cs
namespace ResumeRadar.Api.Dtos;

public class AnalyzeRequest
{
    public string? JobTitle { get; set; }
    public string? Company { get; set; }
    public required string JobPostingText { get; set; }
}
