namespace CodeCareer.Configuration;

public class JudgeOptions
{
    public const string SectionName = "Judge";

    public string BaseUrl { get; set; } = "http://localhost:2358";
    public string AuthToken { get; set; } = string.Empty;
    public int DefaultTimeLimitSeconds { get; set; } = 5;
    public int DefaultMemoryLimitKb { get; set; } = 128000;
    public int RequestTimeoutSeconds { get; set; } = 30;
    public int PollIntervalMs { get; set; } = 500;
    public int MaxPollAttempts { get; set; } = 60;
}
