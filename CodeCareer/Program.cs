using CodeCareer.Areas.User.Services.Implementations.JsonServices;
using CodeCareer.Areas.User.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        // Сохранять значения по умолчанию при сериализации
        options.JsonSerializerOptions.DefaultIgnoreCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.Never;

        // Или игнорировать только null значения
        // options.JsonSerializerOptions.DefaultIgnoreCondition = 
        //     System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// Добавляем аутентификацию
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Authorizate";
        options.LogoutPath = "/Account/Logout";
        options.ExpireTimeSpan = TimeSpan.FromDays(7); // запоминать на неделю
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<IUserService, UserJsonService>();
builder.Services.AddScoped<IPublicationService, PublicationJsonService>();
builder.Services.AddScoped<ITagService, TagJsonService>();

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );

app.Run();
