using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResumeRadar.Api.Data;
using ResumeRadar.Api.Dtos;
using ResumeRadar.Api.Services;
using System.Security.Claims;

namespace ResumeRadar.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/resume")]
public class ResumeController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IPdfExtractor _pdfExtractor;

    public ResumeController(AppDbContext db, IPdfExtractor pdfExtractor)
    {
        _db = db;
        _pdfExtractor = pdfExtractor;
    }

    [HttpGet]
    public async Task<IActionResult> GetResume()
    {
        var user = await GetCurrentUser();
        if (user is null) return Unauthorized();

        return Ok(new { resumeText = user.ResumeText });
    }

    [HttpPost]
    public async Task<IActionResult> SaveResume([FromBody] ResumeUploadRequest request)
    {
        var user = await GetCurrentUser();
        if (user is null) return Unauthorized();

        user.ResumeText = request.ResumeText;
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("extract-pdf")]
    [RequestSizeLimit(5 * 1024 * 1024)] // 5 MB
    public async Task<IActionResult> ExtractPdf(IFormFile file)
    {
        if (file.Length == 0)
            return BadRequest(new PdfExtractResponse { Success = false, Error = "No file uploaded." });

        if (file.ContentType != "application/pdf" && !file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new PdfExtractResponse { Success = false, Error = "Only PDF files are accepted." });

        // Validate PDF magic bytes
        using var stream = file.OpenReadStream();
        var header = new byte[5];
        var bytesRead = await stream.ReadAsync(header.AsMemory(0, 5));
        if (bytesRead < 5 || header[0] != 0x25 || header[1] != 0x50 || header[2] != 0x44 || header[3] != 0x46 || header[4] != 0x2D)
            return BadRequest(new PdfExtractResponse { Success = false, Error = "File does not appear to be a valid PDF." });

        stream.Position = 0;
        var result = await _pdfExtractor.ExtractTextAsync(stream);

        return Ok(new PdfExtractResponse
        {
            Success = result.Success,
            Text = result.Text,
            Error = result.Error
        });
    }

    private async Task<Models.User?> GetCurrentUser()
    {
        var gitHubId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return await _db.Users.FirstOrDefaultAsync(u => u.GitHubId == gitHubId);
    }
}
