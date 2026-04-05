using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using ResumeRadar.Api.Controllers;
using ResumeRadar.Api.Data;
using ResumeRadar.Api.Dtos;
using ResumeRadar.Api.Models;
using ResumeRadar.Api.Services;
using System.Security.Claims;
using System.Text.Json;

namespace ResumeRadar.Api.Tests.Controllers;

public class AnalysisControllerTests
{
    private static AppDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static ClaimsPrincipal CreateUser(string gitHubId = "12345")
    {
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, gitHubId) };
        return new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
    }

    [Fact]
    public async Task Analyze_WithNoResume_ReturnsBadRequest()
    {
        var db = CreateDb();
        db.Users.Add(new User { GitHubId = "12345", Username = "testuser" });
        await db.SaveChangesAsync();

        var mockService = new Mock<IAnalysisService>();
        var controller = new AnalysisController(db, mockService.Object);
        controller.ControllerContext.HttpContext = new DefaultHttpContext { User = CreateUser() };

        var result = await controller.Analyze(new AnalyzeRequest { JobPostingText = "Some job" });

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Analyze_ExceedingDailyLimit_ReturnsTooManyRequests()
    {
        var db = CreateDb();
        var user = new User { GitHubId = "12345", Username = "testuser", ResumeText = "My resume" };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        // Add 10 analyses for today
        for (int i = 0; i < 10; i++)
        {
            db.Analyses.Add(new Analysis
            {
                UserId = user.Id,
                JobPostingText = "job",
                StrengthMatches = "[]",
                SkillGaps = "[]",
                TailoredBullets = "[]",
                MatchScore = 50,
                CreatedAt = DateTime.UtcNow
            });
        }
        await db.SaveChangesAsync();

        var mockService = new Mock<IAnalysisService>();
        var controller = new AnalysisController(db, mockService.Object);
        controller.ControllerContext.HttpContext = new DefaultHttpContext { User = CreateUser() };

        var result = await controller.Analyze(new AnalyzeRequest { JobPostingText = "Another job" });

        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(429, statusResult.StatusCode);
    }

    [Fact]
    public async Task GetHistory_ReturnsUserAnalyses()
    {
        var db = CreateDb();
        var user = new User { GitHubId = "12345", Username = "testuser" };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        db.Analyses.Add(new Analysis
        {
            UserId = user.Id,
            JobTitle = "Dev",
            Company = "Acme",
            JobPostingText = "job",
            StrengthMatches = "[]",
            SkillGaps = "[]",
            TailoredBullets = "[]",
            MatchScore = 80
        });
        await db.SaveChangesAsync();

        var controller = new AnalysisController(db, new Mock<IAnalysisService>().Object);
        controller.ControllerContext.HttpContext = new DefaultHttpContext { User = CreateUser() };

        var result = await controller.GetHistory();

        var ok = Assert.IsType<OkObjectResult>(result);
        var json = JsonSerializer.Serialize(ok.Value);
        Assert.Contains("Dev", json);
        Assert.Contains("Acme", json);
    }
}
