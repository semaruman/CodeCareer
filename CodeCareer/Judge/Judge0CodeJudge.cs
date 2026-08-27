using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using CodeCareer.Areas.User.Models;
using CodeCareer.Areas.User.Services.Interfaces;
using CodeCareer.Configuration;
using Microsoft.Extensions.Options;

namespace CodeCareer.Judge;

public class Judge0CodeJudge : ICodeJudge
{
    private static readonly Dictionary<string, int> LanguageIds = new(StringComparer.OrdinalIgnoreCase)
    {
        ["csharp"] = 51,
        ["c#"] = 51,
        ["python"] = 71,
        ["python3"] = 71,
        ["cpp"] = 54,
        ["c++"] = 54,
        ["java"] = 62,
        ["javascript"] = 63,
        ["js"] = 63,
    };

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ITaskService _taskService;
    private readonly JudgeOptions _options;
    private readonly ILogger<Judge0CodeJudge> _logger;

    public Judge0CodeJudge(
        IHttpClientFactory httpClientFactory,
        ITaskService taskService,
        IOptions<JudgeOptions> options,
        ILogger<Judge0CodeJudge> logger)
    {
        _httpClientFactory = httpClientFactory;
        _taskService = taskService;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<JudgeResult> ExecuteAsync(CodeSubmission submission, CancellationToken cancellationToken = default)
    {
        var task = _taskService.GetById(submission.TaskId);
        if (task == null)
        {
            return new JudgeResult
            {
                Status = SubmissionStatus.SystemError,
                Message = "Задача не найдена",
            };
        }

        if (!LanguageIds.TryGetValue(submission.Language, out var languageId))
        {
            return new JudgeResult
            {
                Status = SubmissionStatus.SystemError,
                Message = $"Язык '{submission.Language}' не поддерживается",
            };
        }

        var testResults = new List<TestCaseResult>();
        var passed = 0;
        double? maxTime = null;
        int? maxMemory = null;

        for (var i = 0; i < task.InputStrings.Count; i++)
        {
            var isHidden = i >= 2;
            var stdin = task.InputStrings[i] ?? string.Empty;
            var expected = (task.OutputStrings.ElementAtOrDefault(i) ?? string.Empty).Trim().Replace("\r\n", "\n");

            if (string.IsNullOrWhiteSpace(expected) && string.IsNullOrWhiteSpace(stdin))
            {
                continue;
            }

            var caseResult = await RunSingleCaseAsync(
                submission.SourceCode,
                languageId,
                stdin,
                expected,
                i,
                isHidden,
                cancellationToken);

            testResults.Add(caseResult);

            if (caseResult.Status == SubmissionStatus.Accepted)
            {
                passed++;
            }
            else if (caseResult.Status != SubmissionStatus.SystemError || !isHidden)
            {
                return BuildFinalResult(caseResult.Status, passed, task.InputStrings.Count, testResults, maxTime, maxMemory, caseResult.ErrorMessage);
            }

            if (caseResult.ExecutionTime.HasValue)
            {
                maxTime = maxTime.HasValue ? Math.Max(maxTime.Value, caseResult.ExecutionTime.Value) : caseResult.ExecutionTime;
            }

            if (caseResult.MemoryUsed.HasValue)
            {
                maxMemory = maxMemory.HasValue ? Math.Max(maxMemory.Value, caseResult.MemoryUsed.Value) : caseResult.MemoryUsed;
            }
        }

        var allPassed = passed > 0 && passed == testResults.Count;
        return BuildFinalResult(
            allPassed ? SubmissionStatus.Accepted : SubmissionStatus.WrongAnswer,
            passed,
            testResults.Count,
            testResults,
            maxTime,
            maxMemory,
            allPassed ? null : "Не все тесты пройдены");
    }

    private async Task<TestCaseResult> RunSingleCaseAsync(
        string sourceCode,
        int languageId,
        string stdin,
        string expectedOutput,
        int index,
        bool isHidden,
        CancellationToken cancellationToken)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("Judge0");
            var createPayload = new
            {
                source_code = sourceCode,
                language_id = languageId,
                stdin,
                expected_output = expectedOutput,
                cpu_time_limit = _options.DefaultTimeLimitSeconds,
                memory_limit = _options.DefaultMemoryLimitKb,
            };

            using var createResponse = await client.PostAsJsonAsync("submissions?base64_encoded=false&wait=false", createPayload, cancellationToken);
            if (!createResponse.IsSuccessStatusCode)
            {
                var errorBody = await createResponse.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning("Judge0 create failed: {Status} {Body}", createResponse.StatusCode, errorBody);
                return new TestCaseResult
                {
                    Index = index,
                    IsHidden = isHidden,
                    Status = SubmissionStatus.SystemError,
                    ErrorMessage = "Judge0 недоступен. Убедитесь, что сервис judge запущен.",
                };
            }

            var created = await createResponse.Content.ReadFromJsonAsync<Judge0TokenResponse>(cancellationToken: cancellationToken);
            if (created?.Token == null)
            {
                return new TestCaseResult
                {
                    Index = index,
                    IsHidden = isHidden,
                    Status = SubmissionStatus.SystemError,
                    ErrorMessage = "Пустой ответ Judge0",
                };
            }

            Judge0SubmissionResult? result = null;
            for (var attempt = 0; attempt < _options.MaxPollAttempts; attempt++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Delay(_options.PollIntervalMs, cancellationToken);

                using var pollResponse = await client.GetAsync($"submissions/{created.Token}?base64_encoded=false", cancellationToken);
                if (!pollResponse.IsSuccessStatusCode)
                {
                    continue;
                }

                result = await pollResponse.Content.ReadFromJsonAsync<Judge0SubmissionResult>(cancellationToken: cancellationToken);
                if (result?.Status?.Id is >= 1 and <= 2)
                {
                    continue;
                }

                break;
            }

            if (result?.Status == null)
            {
                return new TestCaseResult
                {
                    Index = index,
                    IsHidden = isHidden,
                    Status = SubmissionStatus.SystemError,
                    ErrorMessage = "Превышено время ожидания Judge0",
                };
            }

            var status = MapJudge0Status(result.Status.Id);
            return new TestCaseResult
            {
                Index = index,
                IsHidden = isHidden,
                Status = status,
                ActualOutput = isHidden ? null : result.Stdout,
                ExpectedOutput = isHidden ? null : expectedOutput,
                ErrorMessage = result.CompileOutput ?? result.Stderr,
                ExecutionTime = ParseDouble(result.Time),
                MemoryUsed = ParseInt(result.Memory),
            };
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Judge0 execution failed for test case {Index}", index);
            return new TestCaseResult
            {
                Index = index,
                IsHidden = isHidden,
                Status = SubmissionStatus.SystemError,
                ErrorMessage = ex.Message,
            };
        }
    }

    private static JudgeResult BuildFinalResult(
        SubmissionStatus status,
        int passed,
        int total,
        IReadOnlyList<TestCaseResult> testResults,
        double? executionTime,
        int? memoryUsed,
        string? message)
    {
        var score = total == 0 ? 0 : (int)Math.Round(100.0 * passed / total);
        if (status == SubmissionStatus.Accepted)
        {
            score = 100;
        }

        return new JudgeResult
        {
            Status = status,
            Score = score,
            ExecutionTime = executionTime,
            MemoryUsed = memoryUsed,
            TestResults = testResults,
            Message = message,
        };
    }

    private static SubmissionStatus MapJudge0Status(int? statusId) => statusId switch
    {
        3 => SubmissionStatus.Accepted,
        4 => SubmissionStatus.WrongAnswer,
        5 => SubmissionStatus.TimeLimitExceeded,
        6 => SubmissionStatus.CompilationError,
        7 or 8 or 9 or 10 or 11 or 12 => SubmissionStatus.RuntimeError,
        13 => SubmissionStatus.SystemError,
        14 => SubmissionStatus.SystemError,
        15 => SubmissionStatus.SystemError,
        _ => SubmissionStatus.SystemError,
    };

    private static double? ParseDouble(string? value) =>
        double.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var d) ? d : null;

    private static int? ParseInt(string? value) =>
        int.TryParse(value, out var i) ? i : null;

    private sealed class Judge0TokenResponse
    {
        [JsonPropertyName("token")]
        public string? Token { get; set; }
    }

    private sealed class Judge0SubmissionResult
    {
        [JsonPropertyName("status")]
        public Judge0Status? Status { get; set; }

        [JsonPropertyName("stdout")]
        public string? Stdout { get; set; }

        [JsonPropertyName("stderr")]
        public string? Stderr { get; set; }

        [JsonPropertyName("compile_output")]
        public string? CompileOutput { get; set; }

        [JsonPropertyName("time")]
        public string? Time { get; set; }

        [JsonPropertyName("memory")]
        public string? Memory { get; set; }
    }

    private sealed class Judge0Status
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
    }
}
