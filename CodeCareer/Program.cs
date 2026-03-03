using CodeCareer.Areas.User.Services.Implementations.JsonServices;
using CodeCareer.Areas.User.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        // ��������� �������� �� ��������� ��� ������������
        options.JsonSerializerOptions.DefaultIgnoreCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.Never;

        // ��� ������������ ������ null ��������
        // options.JsonSerializerOptions.DefaultIgnoreCondition = 
        //     System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// ��������� ��������������
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Authorizate";
        options.LogoutPath = "/LogoutUser";
        options.ExpireTimeSpan = TimeSpan.FromDays(7); // ���������� �� ������
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<IUserService, UserJsonService>();
builder.Services.AddScoped<IPublicationService, PublicationJsonService>();
builder.Services.AddScoped<ITagService, TagJsonService>();
builder.Services.AddScoped<ITaskService, TaskJsonService>();

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );

app.Run();
