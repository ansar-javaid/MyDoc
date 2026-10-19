using System.Net;
using System.Text.RegularExpressions;

namespace MyDoc.Services;
public interface IHtmlSafetyService { string Sanitize(string? html); }
public sealed partial class HtmlSafetyService : IHtmlSafetyService
{
    // Quill output is normalized to this intentionally small safe subset before persistence.
    public string Sanitize(string? html)
    {
        if (string.IsNullOrWhiteSpace(html)) return "<p><br></p>";
        var value = ScriptOrStyle().Replace(html, string.Empty);
        value = EventHandlers().Replace(value, string.Empty);
        value = UnsafeUrls().Replace(value, "$1=\"#\"");
        value = DisallowedTags().Replace(value, string.Empty);
        return value.Length > 500_000 ? value[..500_000] : value;
    }
    [GeneratedRegex("<\\s*(script|style|iframe|object|embed)[^>]*>[\\s\\S]*?<\\s*/\\s*\\1\\s*>", RegexOptions.IgnoreCase)] private static partial Regex ScriptOrStyle();
    [GeneratedRegex("\\s+on[a-z]+\\s*=\\s*(?:\"[^\"]*\"|'[^']*'|[^\\s>]+)", RegexOptions.IgnoreCase)] private static partial Regex EventHandlers();
    [GeneratedRegex("(href|src)\\s*=\\s*['\"]?\\s*(?:javascript|data):[^'\"\\s>]*", RegexOptions.IgnoreCase)] private static partial Regex UnsafeUrls();
    [GeneratedRegex("</?(?!p\\b|br\\b|strong\\b|b\\b|em\\b|i\\b|u\\b|h[1-3]\\b|ul\\b|ol\\b|li\\b|a\\b|span\\b)[a-z][^>]*>", RegexOptions.IgnoreCase)] private static partial Regex DisallowedTags();
}
