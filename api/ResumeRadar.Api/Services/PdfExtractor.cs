using System.Text;
using UglyToad.PdfPig;

namespace ResumeRadar.Api.Services;

public class PdfExtractor : IPdfExtractor
{
    public Task<PdfExtractResult> ExtractTextAsync(Stream pdfStream)
    {
        try
        {
            using var document = PdfDocument.Open(pdfStream);
            var sb = new StringBuilder();

            foreach (var page in document.GetPages())
            {
                sb.AppendLine(page.Text);
            }

            var text = sb.ToString().Trim();
            if (string.IsNullOrWhiteSpace(text))
            {
                return Task.FromResult(new PdfExtractResult(false, null,
                    "Could not extract text from this PDF. It may be a scanned image. Please paste your resume text instead."));
            }

            return Task.FromResult(new PdfExtractResult(true, text, null));
        }
        catch (Exception)
        {
            return Task.FromResult(new PdfExtractResult(false, null,
                "Failed to read this PDF. Please paste your resume text instead."));
        }
    }
}
