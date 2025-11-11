using Microsoft.EntityFrameworkCore;
using ScheduleMaster.Data;
using ScheduleMaster.Services;
using ScheduleMaster.Security;
using ScheduleMaster.Extensions;
using Microsoft.AspNetCore.CookiePolicy;
using ScheduleMaster.DbSeader;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddDbContext<ScheduleMasterDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<StudioService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<GroupService>();
builder.Services.AddScoped<ScheduleService>();
builder.Services.AddScoped<JwtProvider>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddApiAuthentication(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<ScheduleMasterDbContext>();
        var userService = services.GetRequiredService<UserService>();
        var studioService = services.GetRequiredService<StudioService>();
        var groupService = services.GetRequiredService<GroupService>();
        var scheduleService = services.GetRequiredService<ScheduleService>();

        Console.WriteLine("Начинаем инициализацию данных...");
        await DbSeeder.SeedAsync(context, userService, studioService, groupService, scheduleService);
        Console.WriteLine("Инициализация данных завершена.");
    }

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();