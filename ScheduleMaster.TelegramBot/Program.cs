using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Polling;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using ScheduleMaster.TelegramBot.Handlers.Commands;
using ScheduleMaster.TelegramBot.Handlers;
using ScheduleMaster.TelegramBot.States;
using ScheduleMaster.Services;
using ScheduleMaster.TelegramBot.Services;



// Логирование
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/bot-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

string botToken = config["Telegram:BotToken"]
    ?? throw new ArgumentNullException("Telegram:BotToken");

var services = new ServiceCollection();
services.AddLogging(builder =>
{
    builder.ClearProviders();
    builder.AddSerilog();
});
services.AddMemoryCache();

// Регистрируем конфигурацию
services.AddSingleton<IConfiguration>(config);
services.AddHttpClient<ApiClient>(client =>
{
    client.BaseAddress = new Uri(config["ApiBaseUrl"] ?? "http://localhost:5003/");
});

// Регистрация зависимостей
services.AddSingleton<MessageHandler>();
services.AddSingleton<TelegramBotClient>(new TelegramBotClient(botToken));
services.AddTransient<MenuStateService>();
services.AddTransient<MenuService>();
services.AddSingleton<UserRegistrationStateService>();
services.AddTransient<UserRegistrationService>();
services.AddTransient<StudioService>();
services.AddTransient<BeginCommandHandler>();
services.AddTransient<IBotCommandHandler, BeginCommandHandler>();
services.AddTransient<StartCommandHandler>();
services.AddTransient<IBotCommandHandler, StartCommandHandler>();
services.AddTransient<MenuCommandHandler>();
services.AddTransient<IBotCommandHandler, MenuCommandHandler>();
services.AddTransient<CommandRouter>();
services.AddTransient<MenuButtonHandler>();


var serviceProvider = services.BuildServiceProvider();

var messageHandler = serviceProvider.GetRequiredService<MessageHandler>();
var botClient = serviceProvider.GetRequiredService<TelegramBotClient>();
var commandRouter = serviceProvider.GetRequiredService<CommandRouter>();
var registrationStateService = serviceProvider.GetRequiredService<UserRegistrationStateService>();
var regService = serviceProvider.GetRequiredService<UserRegistrationService>();
var menuStateService = serviceProvider.GetRequiredService<MenuStateService>();
var menuBtnHandler = serviceProvider.GetRequiredService<MenuButtonHandler>();

var me = await botClient.GetMeAsync();
Log.Information($"Бот @{me.Username} запущен");

var cts = new CancellationTokenSource();
var receiverOptions = new ReceiverOptions() { AllowedUpdates = Array.Empty<UpdateType>() };

botClient.StartReceiving(
    updateHandler: (bot, update, ct) => messageHandler.HandleUpdateAsync(update, ct),
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

Console.ReadLine();
cts.Cancel();


static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    Log.Error(exception, "Polling error: {Message}", exception.Message);
    return Task.CompletedTask;
}
