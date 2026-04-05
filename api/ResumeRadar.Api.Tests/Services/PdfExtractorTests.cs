using ResumeRadar.Api.Services;
using UglyToad.PdfPig.Writer;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Core;
using UglyToad.PdfPig.Fonts.Standard14Fonts;

namespace ResumeRadar.Api.Tests.Services;

public class PdfExtractorTests
{
    private readonly PdfExtractor _extractor = new();

    [Fact]
    public async Task ExtractText_WithValidPdf_ReturnsText()
    {
        // Create a minimal valid PDF in memory using PdfPig
        using var stream = new MemoryStream();
        var builder = new PdfDocumentBuilder();
        var page = builder.AddPage(PageSize.Letter);
        var font = builder.AddStandard14Font(Standard14Font.Helvetica);
        page.AddText("Software Developer with 10 years experience", 12, new PdfPoint(50, 700), font);
        var pdfBytes = builder.Build();
        stream.Write(pdfBytes);
        stream.Position = 0;

        var result = await _extractor.ExtractTextAsync(stream);

        Assert.True(result.Success);
        Assert.Contains("Software Developer", result.Text);
    }

    [Fact]
    public async Task ExtractText_WithEmptyStream_ReturnsFailure()
    {
        using var stream = new MemoryStream();

        var result = await _extractor.ExtractTextAsync(stream);

        Assert.False(result.Success);
        Assert.Null(result.Text);
    }
}
