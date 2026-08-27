namespace CodeCareer.Judge;

/// <summary>
/// Extension point for frontend coding challenges (HTML/CSS/JS validation).
/// Full browser-based judge is out of scope; implement IFrontendChallengeValidator when needed.
/// </summary>
public interface IFrontendChallengeValidator
{
    Task<FrontendValidationResult> ValidateAsync(FrontendChallengeSubmission submission, CancellationToken cancellationToken = default);
}

public sealed class FrontendChallengeSubmission
{
    public int TaskId { get; init; }
    public string Html { get; init; } = string.Empty;
    public string Css { get; init; } = string.Empty;
    public string JavaScript { get; init; } = string.Empty;
}

public sealed class FrontendValidationResult
{
    public bool Passed { get; init; }
    public IReadOnlyList<string> Messages { get; init; } = Array.Empty<string>();
}
