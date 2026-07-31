using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("Llm", client =>
{
    var baseUrl = builder.Configuration["Llm:BaseUrl"] ?? "https://api.openai.com/v1/";
    client.BaseAddress = new Uri(baseUrl.EndsWith("/") ? baseUrl : baseUrl + "/");
    client.Timeout = TimeSpan.FromSeconds(90);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("MvcApp", policy =>
    {
        policy.WithOrigins(
                "https://localhost:7266",
                "http://localhost:5126",
                "https://localhost:44350",
                "http://localhost:43735",
                "https://localhost:7301",
                "http://localhost:7300")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();
app.UseCors("MvcApp");

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapPost("/api/chat", async (ChatRequest request, IHttpClientFactory httpClientFactory, IConfiguration config) =>
{
    if (request.Messages is null || request.Messages.Count == 0)
    {
        return Results.BadRequest(new { error = "messages required" });
    }

    var context = request.NoteContext ?? string.Empty;
    if (context.Length > 14000)
    {
        context = context[..14000];
    }

    var systemPrompt =
        "Ты учебный ассистент платформы CodeCareer. Отвечай на русском. " +
        "Опирайся ТОЛЬКО на текст конспекта ниже. Если ответа нет в конспекте — честно скажи об этом " +
        "и предложи уточнить вопрос или перейти к задачам темы. Не выдумывай факты вне материала.\n\n" +
        $"Конспект «{request.NoteTitle}» (id={request.NoteId}):\n{context}";

    var apiKey = config["Llm:ApiKey"];
    var model = config["Llm:Model"] ?? "gpt-4o-mini";

    // Без ключа — локальный fallback, чтобы UI можно было проверить
    if (string.IsNullOrWhiteSpace(apiKey) || apiKey == "CHANGE_ME")
    {
        var lastUser = request.Messages.LastOrDefault(m => m.Role == "user")?.Content ?? "";
        var snippet = context.Length > 400 ? context[..400] + "…" : context;
        var reply =
            $"(демо-режим без Llm:ApiKey) По конспекту «{request.NoteTitle}»: " +
            $"вы спросили «{lastUser}». Краткий фрагмент материала:\n{snippet}\n\n" +
            "Добавьте ключ LLM в user-secrets проекта CodeCareer.AiChat для реальных ответов.";
        return Results.Ok(new ChatResponse { Reply = reply });
    }

    var payload = new
    {
        model,
        messages = new object[]
            {
                new { role = "system", content = systemPrompt }
            }
            .Concat(request.Messages.Select(m => new { role = m.Role, content = m.Content }))
            .ToArray(),
        temperature = 0.3
    };

    var client = httpClientFactory.CreateClient("Llm");
    using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "chat/completions");
    httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
    httpRequest.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

    using var response = await client.SendAsync(httpRequest);
    var body = await response.Content.ReadAsStringAsync();
    if (!response.IsSuccessStatusCode)
    {
        return Results.Json(new { error = "LLM error", detail = body }, statusCode: (int)response.StatusCode);
    }

    using var doc = JsonDocument.Parse(body);
    var content = doc.RootElement
        .GetProperty("choices")[0]
        .GetProperty("message")
        .GetProperty("content")
        .GetString() ?? string.Empty;

    return Results.Ok(new ChatResponse { Reply = content.Trim() });
});

app.Run();

public sealed class ChatRequest
{
    [JsonPropertyName("noteId")]
    public int NoteId { get; set; }

    [JsonPropertyName("noteTitle")]
    public string? NoteTitle { get; set; }

    [JsonPropertyName("noteContext")]
    public string? NoteContext { get; set; }

    [JsonPropertyName("messages")]
    public List<ChatMessageDto>? Messages { get; set; }
}

public sealed class ChatMessageDto
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = "user";

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}

public sealed class ChatResponse
{
    [JsonPropertyName("reply")]
    public string Reply { get; set; } = string.Empty;
}
