namespace CodeCareer.Judge;

public enum SubmissionStatus
{
    Pending,
    Running,
    Accepted,
    WrongAnswer,
    CompilationError,
    RuntimeError,
    TimeLimitExceeded,
    MemoryLimitExceeded,
    SystemError
}

public sealed class CodeSubmission
{
    public int TaskId { get; init; }
    public int UserId { get; init; }
    public string Language { get; init; } = "csharp";
    public string SourceCode { get; init; } = string.Empty;
}

public sealed class TestCaseResult
{
    public int Index { get; init; }
    public bool IsHidden { get; init; }
    public SubmissionStatus Status { get; init; }
    public string? ActualOutput { get; init; }
    public string? ExpectedOutput { get; init; }
    public string? ErrorMessage { get; init; }
    public double? ExecutionTime { get; init; }
    public int? MemoryUsed { get; init; }
}

public sealed class JudgeResult
{
    public SubmissionStatus Status { get; init; }
    public int Score { get; init; }
    public double? ExecutionTime { get; init; }
    public int? MemoryUsed { get; init; }
    public IReadOnlyList<TestCaseResult> TestResults { get; init; } = Array.Empty<TestCaseResult>();
    public string? Message { get; init; }
}

public interface ICodeJudge
{
    Task<JudgeResult> ExecuteAsync(CodeSubmission submission, CancellationToken cancellationToken = default);
}
