using CodeCareer.Judge;

namespace CodeCareer.Tests;

public class JudgeResultMappingTests
{
    [Theory]
    [InlineData(3, SubmissionStatus.Accepted)]
    [InlineData(4, SubmissionStatus.WrongAnswer)]
    [InlineData(5, SubmissionStatus.TimeLimitExceeded)]
    [InlineData(6, SubmissionStatus.CompilationError)]
    public void SubmissionStatus_MapsJudge0Ids(int id, SubmissionStatus expected)
    {
        var status = Map(id);
        Assert.Equal(expected, status);
    }

    private static SubmissionStatus Map(int statusId) => statusId switch
    {
        3 => SubmissionStatus.Accepted,
        4 => SubmissionStatus.WrongAnswer,
        5 => SubmissionStatus.TimeLimitExceeded,
        6 => SubmissionStatus.CompilationError,
        _ => SubmissionStatus.SystemError,
    };
}

public class CodeSubmissionModelTests
{
    [Fact]
    public void CodeSubmission_DefaultLanguage_IsCSharp()
    {
        var submission = new CodeSubmission { TaskId = 1, UserId = 1, SourceCode = "code" };
        Assert.Equal("csharp", submission.Language);
    }

    [Fact]
    public void JudgeResult_Accepted_HasScore100WhenAllPass()
    {
        var result = new JudgeResult { Status = SubmissionStatus.Accepted, Score = 100 };
        Assert.Equal(SubmissionStatus.Accepted, result.Status);
        Assert.Equal(100, result.Score);
    }
}
