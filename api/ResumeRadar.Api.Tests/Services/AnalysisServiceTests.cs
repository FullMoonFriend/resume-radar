// api/ResumeRadar.Api.Tests/Services/AnalysisServiceTests.cs
using System.Text.Json;
using Moq;
using ResumeRadar.Api.Services;

namespace ResumeRadar.Api.Tests.Services;

public class AnalysisServiceTests
{
    [Fact]
    public void ParseAnalysisResponse_WithValidJson_ReturnsResult()
    {
        var json = JsonSerializer.Serialize(new
        {
            matchScore = 82,
            strengthMatches = new[] { "C# experience", "Azure cloud" },
            skillGaps = new[] { new { skill = "Docker", note = "Not mentioned on resume" } },
            tailoredBullets = new[] { "Built carrier integration APIs..." }
        });

        var result = AnalysisService.ParseAnalysisResponse(json);

        Assert.Equal(82, result.MatchScore);
        Assert.Equal(2, result.StrengthMatches.Count);
        Assert.Single(result.SkillGaps);
        Assert.Single(result.TailoredBullets);
    }

    [Fact]
    public void ParseAnalysisResponse_WithInvalidJson_ThrowsException()
    {
        Assert.Throws<JsonException>(() => AnalysisService.ParseAnalysisResponse("not json"));
    }

    [Fact]
    public void BuildPrompt_IncludesResumeAndJobPosting()
    {
        var prompt = AnalysisService.BuildPrompt("My resume text", "Job description here");

        Assert.Contains("My resume text", prompt);
        Assert.Contains("Job description here", prompt);
        Assert.Contains("matchScore", prompt); // instructs JSON format
    }
}
