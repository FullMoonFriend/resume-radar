namespace ResumeRadar.Api.Services;

public record PdfExtractResult(bool Success, string? Text, string? Error);

public interface IPdfExtractor
{
    Task<PdfExtractResult> ExtractTextAsync(Stream pdfStream);
}
