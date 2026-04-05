// api/ResumeRadar.Api/Services/AnalysisService.cs
using System.Text.Json;
using Anthropic.SDK;
using Anthropic.SDK.Constants;
using Anthropic.SDK.Messaging;

namespace ResumeRadar.Api.Services;

public class AnalysisService : IAnalysisService
{
    private readonly IConfiguration _config;

    public AnalysisService(IConfiguration config) => _config = config;

    public async Task<AnalysisResult> AnalyzeAsync(string resumeText, string jobPostingText)
    {
        var apiKey = _config["Anthropic:ApiKey"]!;
        using var client = new AnthropicClient(new APIAuthentication(apiKey));
        var prompt = BuildPrompt(resumeText, jobPostingText);

        var parameters = new MessageParameters
        {
            Model = AnthropicModels.Claude46Sonnet,
            MaxTokens = 2048,
            Stream = false,
            Messages = [new Message(RoleType.User, prompt)],
            System = [new SystemMessage("You are a resume analysis assistant. Always respond with valid JSON only, no markdown fences.")]
        };

        var response = await client.Messages.GetClaudeMessageAsync(parameters);
        var text = response.Message.ToString().Trim();
        return ParseAnalysisResponse(text);
    }

    public static string BuildPrompt(string resumeText, string jobPostingText)
    {
        return $"""
            Analyze how well this resume matches the job posting. Return a JSON object with exactly these fields:

            - "matchScore": integer 0-100 representing overall match percentage
            - "strengthMatches": array of strings, each describing a strong skill/experience match (3-5 items)
            - "skillGaps": array of objects with "skill" and "note" fields, each describing a gap between the resume and job requirements (2-4 items)
            - "tailoredBullets": array of strings, each a tailored talking point the candidate could use in a cover letter or interview (2-3 items)

            RESUME:
            {resumeText}

            JOB POSTING:
            {jobPostingText}

            Respond with JSON only.
            """;
    }

    public static AnalysisResult ParseAnalysisResponse(string json)
    {
        // Strip markdown code fences if Claude wraps the JSON in them
        json = json.Trim();
        if (json.StartsWith("```"))
        {
            var firstNewline = json.IndexOf('\n');
            if (firstNewline >= 0)
                json = json[(firstNewline + 1)..];
            if (json.EndsWith("```"))
                json = json[..^3];
            json = json.Trim();
        }

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var parsed = JsonSerializer.Deserialize<AnalysisResponseJson>(json, options)
            ?? throw new JsonException("Failed to parse analysis response");

        return new AnalysisResult(
            parsed.MatchScore,
            parsed.StrengthMatches,
            parsed.SkillGaps.Select(g => new SkillGap(g.Skill, g.Note)).ToList(),
            parsed.TailoredBullets
        );
    }

    private record AnalysisResponseJson(
        int MatchScore,
        List<string> StrengthMatches,
        List<SkillGapJson> SkillGaps,
        List<string> TailoredBullets
    );

    private record SkillGapJson(string Skill, string Note);
}
