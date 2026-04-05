using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResumeRadar.Api.Data;
using System.Security.Claims;

namespace ResumeRadar.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public AuthController(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    [HttpGet("login")]
    public IActionResult Login()
    {
        var redirectUri = _config["ClientUrl"] ?? "/";
        return Challenge(new AuthenticationProperties { RedirectUri = redirectUri }, "GitHub");
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok();
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var gitHubId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _db.Users.FirstOrDefaultAsync(u => u.GitHubId == gitHubId);
        if (user is null) return Unauthorized();

        return Ok(new
        {
            user.Username,
            user.AvatarUrl,
            hasResume = user.ResumeText is not null
        });
    }
}
