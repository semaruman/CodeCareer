namespace CodeCareer.Infrastructure;

public interface IMarkdownSanitizer
{
    string SanitizeHtml(string html);
}
