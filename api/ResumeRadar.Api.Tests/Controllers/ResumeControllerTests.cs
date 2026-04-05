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

namespace ResumeRadar.Api.Tests.Controllers;

public class ResumeControllerTests
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
    public async Task SaveResume_StoresTextOnUser()
    {
        var db = CreateDb();
        db.Users.Add(new User { GitHubId = "12345", Username = "testuser" });
        await db.SaveChangesAsync();

        var controller = new ResumeController(db, new Mock<IPdfExtractor>().Object);
        controller.ControllerContext.HttpContext = new DefaultHttpContext { User = CreateUser() };

        var result = await controller.SaveResume(new ResumeUploadRequest { ResumeText = "My resume text" });

        Assert.IsType<OkResult>(result);
        var user = await db.Users.FirstAsync(u => u.GitHubId == "12345");
        Assert.Equal("My resume text", user.ResumeText);
    }

    [Fact]
    public async Task GetResume_ReturnsStoredText()
    {
        var db = CreateDb();
        db.Users.Add(new User { GitHubId = "12345", Username = "testuser", ResumeText = "Stored resume" });
        await db.SaveChangesAsync();

        var controller = new ResumeController(db, new Mock<IPdfExtractor>().Object);
        controller.ControllerContext.HttpContext = new DefaultHttpContext { User = CreateUser() };

        var result = await controller.GetResume();

        var ok = Assert.IsType<OkObjectResult>(result);
        var json = System.Text.Json.JsonSerializer.Serialize(ok.Value);
        Assert.Contains("Stored resume", json);
    }
}
