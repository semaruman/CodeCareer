using CodeCareer.Areas.User.Services.Implementations.JsonServices;
using CodeCareer.Areas.User.Services.Interfaces;

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

builder.Services.AddScoped<IUserService, UserJsonService>();
builder.Services.AddScoped<IPublicationService, PublicationJsonService>();
builder.Services.AddScoped<ITagService, TagJsonService>();

var app = builder.Build();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );

app.Run();
