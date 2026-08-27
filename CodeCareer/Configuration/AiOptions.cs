namespace CodeCareer.Configuration;

public class AiOptions
{
    public const string SectionName = "AiChat";

    public string BaseUrl { get; set; } = "http://localhost:7300";
    public string ChatPath { get; set; } = "/api/chat";
    public string InternalApiKey { get; set; } = string.Empty;
    public int MaxPromptLength { get; set; } = 4000;
    public int RequestTimeoutSeconds { get; set; } = 90;
}
