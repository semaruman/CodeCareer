using CodeCareer;
using CodeCareer.Areas.User.Data;
using CodeCareer.Configuration;
using CodeCareer.Infrastructure;
using CodeCareer.Judge;
using CodeCareer.Middleware;
using CodeCareer.Security;
using CodeCareer.Areas.User.Services.Implementations;
using CodeCareer.Areas.User.Services.Implementations.MySqlEfServices;
using CodeCareer.Areas.User.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

Constants.Initialize(builder.Configuration);

builder.Services.AddOptions<AiOptions>().Bind(builder.Configuration.GetSection(AiOptions.SectionName)).ValidateDataAnnotations();
builder.Services.AddOptions<JudgeOptions>().Bind(builder.Configuration.GetSection(JudgeOptions.SectionName));
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is required.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var serverVersion = builder.Environment.IsEnvironment("Testing")
        ? new MySqlServerVersion(new Version(8, 0, 0))
        : ServerVersion.AutoDetect(connectionString);
    options.UseMySql(connectionString, serverVersion);
});

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute());
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Authorizate";
        options.LogoutPath = "/LogoutUser";
        options.AccessDeniedPath = "/Authorizate";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole(Roles.Admin));
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("login", httpContext => RateLimitPartition.GetFixedWindowLimiter(
        httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new FixedWindowRateLimiterOptions { PermitLimit = 10, Window = TimeSpan.FromMinutes(1) }));
    options.AddPolicy("ai-chat", httpContext => RateLimitPartition.GetFixedWindowLimiter(
        httpContext.User.Identity?.Name ?? httpContext.Connection.RemoteIpAddress?.ToString() ?? "anon",
        _ => new FixedWindowRateLimiterOptions { PermitLimit = 20, Window = TimeSpan.FromMinutes(1) }));
    options.AddPolicy("submit-code", httpContext => RateLimitPartition.GetFixedWindowLimiter(
        httpContext.User.Identity?.Name ?? "anon",
        _ => new FixedWindowRateLimiterOptions { PermitLimit = 30, Window = TimeSpan.FromMinutes(1) }));
    options.AddPolicy("write-content", httpContext => RateLimitPartition.GetFixedWindowLimiter(
        httpContext.User.Identity?.Name ?? httpContext.Connection.RemoteIpAddress?.ToString() ?? "anon",
        _ => new FixedWindowRateLimiterOptions { PermitLimit = 30, Window = TimeSpan.FromMinutes(1) }));
});

builder.Services.AddHttpClient("Judge0", (sp, client) =>
{
    var opts = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<JudgeOptions>>().Value;
    client.BaseAddress = new Uri(opts.BaseUrl.TrimEnd('/') + "/");
    client.Timeout = TimeSpan.FromSeconds(opts.RequestTimeoutSeconds);
    if (!string.IsNullOrEmpty(opts.AuthToken))
    {
        client.DefaultRequestHeaders.Add("X-Auth-Token", opts.AuthToken);
    }
});

builder.Services.AddHttpClient("AiChat", (sp, client) =>
{
    var opts = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<AiOptions>>().Value;
    client.BaseAddress = new Uri(opts.BaseUrl.TrimEnd('/') + "/");
    client.Timeout = TimeSpan.FromSeconds(opts.RequestTimeoutSeconds);
});

builder.Services.AddScoped<IUserService, UserMySqlEfService>();
builder.Services.AddScoped<IPublicationService, PublicationMySqlEfService>();
builder.Services.AddScoped<ICommentService, CommentMySqlEfService>();
builder.Services.AddScoped<ITagService, TagMySqlEfService>();
builder.Services.AddScoped<ITaskService, TaskMySqlEfService>();
builder.Services.AddScoped<ISectionService, SectionMySqlEfService>();
builder.Services.AddScoped<ITopicService, TopicMySqlEfService>();
builder.Services.AddScoped<INoteService, NoteMySqlEfService>();
builder.Services.AddScoped<IProgressService, ProgressMySqlEfService>();
builder.Services.AddScoped<ICourseService, CourseMySqlEfService>();
builder.Services.AddScoped<IChatHistoryService, ChatHistoryMySqlEfService>();
builder.Services.AddScoped<ISubmissionService, SubmissionMySqlEfService>();
builder.Services.AddScoped<INotificationService, NotificationMySqlEfService>();
builder.Services.AddScoped<IAchievementService, AchievementMySqlEfService>();

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSingleton<LoginLockoutService>();
builder.Services.AddScoped<ICodeJudge, Judge0CodeJudge>();
builder.Services.AddScoped<IMarkdownSanitizer, MarkdownSanitizer>();
builder.Services.AddScoped<IFileStorage, LocalFileStorage>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddExceptionHandler<SmartExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>("database");

var app = builder.Build();

if (!app.Environment.IsEnvironment("Testing"))
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
        LegacyDataMigrator.Migrate(db);
        if (app.Environment.IsDevelopment())
        {
            DevelopmentDataSeeder.Seed(db);
        }
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Errors/500");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseStatusCodePagesWithReExecute("/Errors/404");
app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.UseSecurityHeaders();
app.UseRequestId();
app.UseLoggingMiddleware();

app.MapHealthChecks("/health");
app.MapControllerRoute(name: "errors", pattern: "Errors/{action}", defaults: new { controller = "Errors" });
app.MapControllerRoute(name: "areas", pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.Run();

public partial class Program;
