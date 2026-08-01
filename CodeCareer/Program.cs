using CodeCareer;
using CodeCareer.Areas.User.Data;
using CodeCareer.Areas.User.Services.Implementations.MySqlAdoNetServices;
using CodeCareer.Areas.User.Services.Implementations.MySqlEfServices;
using CodeCareer.Areas.User.Services.Interfaces;
using CodeCareer.Infrastructure;
using CodeCareer.Middleware;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.Never;

    });

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Authorizate";
        options.LogoutPath = "/LogoutUser";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<IUserService, UserMySqlAdoNetService>();
builder.Services.AddScoped<IPublicationService, PublicationMySqlAdoNetService>();
builder.Services.AddScoped<ICommentService, CommentMySqlAdoNetService>();
builder.Services.AddScoped<ITagService, TagMySqlEfService>();
builder.Services.AddScoped<ITaskService, TaskMySqlEfService>();
builder.Services.AddScoped<ISectionService, SectionMySqlEfService>();
builder.Services.AddScoped<ITopicService, TopicMySqlEfService>();
builder.Services.AddScoped<INoteService, NoteMySqlEfService>();
builder.Services.AddScoped<IProgressService, ProgressMySqlEfService>();
builder.Services.AddScoped<ICourseService, CourseMySqlEfService>();
builder.Services.AddScoped<IChatHistoryService, ChatHistoryMySqlEfService>();

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddExceptionHandler<SmartExceptionHandler>();
builder.Services.AddProblemDetails();

Constants.Initialize(builder.Configuration);

var app = builder.Build();

LearningDatabaseInitializer.Initialize();

app.UseStaticFiles();
app.UseExceptionHandler();
app.UseLoggingMiddleware();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );

app.Run();
