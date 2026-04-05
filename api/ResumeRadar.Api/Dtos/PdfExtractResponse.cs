namespace ResumeRadar.Api.Dtos;

public class PdfExtractResponse
{
    public bool Success { get; set; }
    public string? Text { get; set; }
    public string? Error { get; set; }
}
