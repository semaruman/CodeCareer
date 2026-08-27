using Ganss.Xss;

namespace CodeCareer.Infrastructure;

public class MarkdownSanitizer : IMarkdownSanitizer
{
    private static readonly HtmlSanitizer Sanitizer = CreateSanitizer();

    private static HtmlSanitizer CreateSanitizer()
    {
        var sanitizer = new HtmlSanitizer();
        sanitizer.AllowedTags.Add("pre");
        sanitizer.AllowedTags.Add("code");
        sanitizer.AllowedAttributes.Add("class");
        return sanitizer;
    }

    public string SanitizeHtml(string html) => Sanitizer.Sanitize(html ?? string.Empty);
}
