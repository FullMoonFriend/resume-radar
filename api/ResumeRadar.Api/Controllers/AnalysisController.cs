// api/ResumeRadar.Api/Controllers/AnalysisController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResumeRadar.Api.Data;
using ResumeRadar.Api.Dtos;
using ResumeRadar.Api.Models;
using ResumeRadar.Api.Services;
using System.Security.Claims;
using System.Text.Json;

namespace ResumeRadar.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/analysis")]
public class AnalysisController : ControllerBase
{
    private const int DailyLimit = 10;
    private readonly AppDbContext _db;
    private readonly IAnalysisService _analysisService;

    public AnalysisController(AppDbContext db, IAnalysisService analysisService)
    {
        _db = db;
        _analysisService = analysisService;
    }

    [HttpPost]
    public async Task<IActionResult> Analyze([FromBody] AnalyzeRequest request)
    {
        var user = await GetCurrentUser();
        if (user is null) return Unauthorized();

        if (string.IsNullOrWhiteSpace(user.ResumeText))
            return BadRequest(new { error = "Please upload your resume first." });

        var todayStart = DateTime.UtcNow.Date;
        var todayCount = await _db.Analyses
            .CountAsync(a => a.UserId == user.Id && a.CreatedAt >= todayStart);

        if (todayCount >= DailyLimit)
            return StatusCode(429, new { error = "Daily analysis limit reached. Try again tomorrow.", remaining = 0 });

        var result = await _analysisService.AnalyzeAsync(user.ResumeText, request.JobPostingText);

        var analysis = new Analysis
        {
            UserId = user.Id,
            JobTitle = request.JobTitle,
            Company = request.Company,
            JobPostingText = request.JobPostingText,
            MatchScore = result.MatchScore,
            StrengthMatches = JsonSerializer.Serialize(result.StrengthMatches),
            SkillGaps = JsonSerializer.Serialize(result.SkillGaps),
            TailoredBullets = JsonSerializer.Serialize(result.TailoredBullets)
        };

        _db.Analyses.Add(analysis);
        await _db.SaveChangesAsync();

        return Ok(new AnalysisResponse
        {
            Id = analysis.Id,
            JobTitle = analysis.JobTitle,
            Company = analysis.Company,
            MatchScore = result.MatchScore,
            StrengthMatches = result.StrengthMatches,
            SkillGaps = result.SkillGaps,
            TailoredBullets = result.TailoredBullets,
            CreatedAt = analysis.CreatedAt
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetHistory()
    {
        var user = await GetCurrentUser();
        if (user is null) return Unauthorized();

        var analyses = await _db.Analyses
            .Where(a => a.UserId == user.Id)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new AnalysisSummaryResponse
            {
                Id = a.Id,
                JobTitle = a.JobTitle,
                Company = a.Company,
                MatchScore = a.MatchScore,
                CreatedAt = a.CreatedAt
            })
            .ToListAsync();

        return Ok(analyses);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetAnalysis(int id)
    {
        var user = await GetCurrentUser();
        if (user is null) return Unauthorized();

        var analysis = await _db.Analyses.FirstOrDefaultAsync(a => a.Id == id && a.UserId == user.Id);
        if (analysis is null) return NotFound();

        return Ok(new AnalysisResponse
        {
            Id = analysis.Id,
            JobTitle = analysis.JobTitle,
            Company = analysis.Company,
            MatchScore = analysis.MatchScore,
            StrengthMatches = JsonSerializer.Deserialize<List<string>>(analysis.StrengthMatches) ?? [],
            SkillGaps = JsonSerializer.Deserialize<List<SkillGap>>(analysis.SkillGaps) ?? [],
            TailoredBullets = JsonSerializer.Deserialize<List<string>>(analysis.TailoredBullets) ?? [],
            CreatedAt = analysis.CreatedAt
        });
    }

    [HttpGet("remaining")]
    public async Task<IActionResult> GetRemaining()
    {
        var user = await GetCurrentUser();
        if (user is null) return Unauthorized();

        var todayStart = DateTime.UtcNow.Date;
        var todayCount = await _db.Analyses
            .CountAsync(a => a.UserId == user.Id && a.CreatedAt >= todayStart);

        return Ok(new { remaining = DailyLimit - todayCount, limit = DailyLimit });
    }

    private async Task<User?> GetCurrentUser()
    {
        var gitHubId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return await _db.Users.FirstOrDefaultAsync(u => u.GitHubId == gitHubId);
    }
}
