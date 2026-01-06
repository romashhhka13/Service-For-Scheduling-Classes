using Microsoft.EntityFrameworkCore;
using ScheduleMaster.Data;
using ScheduleMaster.Services;
using ScheduleMaster.Security;
using ScheduleMaster.Extensions;
using Microsoft.AspNetCore.CookiePolicy;
using ScheduleMaster.DbSeader;
using StackExchange.Redis;
using ScheduleMaster.Services.Cache;
using ScheduleMaster.Services.ExternalApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddDbContext<ScheduleMasterDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Redis
var redisHost = builder.Configuration["Redis:Host"] ?? "localhost";
var redisPort = int.Parse(builder.Configuration["Redis:Port"] ?? "6379");

builder.Services.AddSingleton<IConnectionMultiplexer>(provider =>
{
    var configuration = ConfigurationOptions.Parse($"{redisHost}:{redisPort}");
    return ConnectionMultiplexer.Connect(configuration);
});
builder.Services.AddScoped<ICacheService, RedisCacheService>();
builder.Services.AddHttpClient<IScheduleApiService, ScheduleApiService>();


builder.Services.AddScoped<StudioService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<GroupService>();
builder.Services.AddScoped<EventService>();
builder.Services.AddScoped<JwtProvider>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddApiAuthentication(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()
              .SetIsOriginAllowed(_ => true);
    });
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    // using (var scope = app.Services.CreateScope())
    // {
    //     var services = scope.ServiceProvider;

    //     var context = services.GetRequiredService<ScheduleMasterDbContext>();

    //     Console.WriteLine("Начинаем инициализацию данных...");
    //     await DbSeeder.SeedAsync(context);
    //     Console.WriteLine("Инициализация данных завершена.");
    // }

    app.MapOpenApi("/openapi/v1.json");
    app.UseSwaggerUi(options =>
    {
        options.DocumentPath = "/openapi/v1.json";
    });
}

app.UseHttpsRedirection();

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
    HttpOnly = HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.Always
});

app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
// app.MapHangfireDashboard();
app.Run();