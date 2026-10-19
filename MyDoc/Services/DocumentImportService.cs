using System.Text;
using System.Text.Encodings.Web;
using Markdig;

namespace MyDoc.Services;
public interface IDocumentImportService { Task<(bool Ok, string Html, string Error)> ConvertAsync(IFormFile? file, CancellationToken ct = default); }
public sealed class DocumentImportService(IHtmlSafetyService safety) : IDocumentImportService
{
    public async Task<(bool Ok, string Html, string Error)> ConvertAsync(IFormFile? file, CancellationToken ct = default)
    {
        if (file is null || file.Length == 0) return (false, "", "Choose a non-empty .txt or .md file.");
        if (file.Length > 1_048_576) return (false, "", "Files must be 1 MB or smaller.");
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (ext is not ".txt" and not ".md") return (false, "", "Only .txt and .md files are supported.");
        using var reader = new StreamReader(file.OpenReadStream(), new UTF8Encoding(false, true), detectEncodingFromByteOrderMarks: true);
        string text;
        try { text = await reader.ReadToEndAsync(ct); } catch (DecoderFallbackException) { return (false, "", "The file must be valid UTF-8 text."); }
        if (string.IsNullOrWhiteSpace(text)) return (false, "", "The uploaded file contains no text.");
        var html = ext == ".md" ? Markdown.ToHtml(text, new MarkdownPipelineBuilder().DisableHtml().Build()) : "<p>" + HtmlEncoder.Default.Encode(text).Replace("\r\n", "<br>").Replace("\n", "<br>") + "</p>";
        return (true, safety.Sanitize(html), "");
    }
}
