using Microsoft.Extensions.Logging;
using ScheduleMaster.TelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ScheduleMaster.TelegramBot.Handlers.Commands
{

    public class StartCommandHandler : IBotCommandHandler
    {
        private readonly TelegramBotClient _botClient;
        private readonly ILogger<StartCommandHandler> _logger;
        private readonly ApiClient _apiClient;

        public string Command => "/start";

        public StartCommandHandler(TelegramBotClient botClient, ILogger<StartCommandHandler> logger, ApiClient apiClient)
        {
            _botClient = botClient;
            _logger = logger;
            _apiClient = apiClient;
        }

        public async Task HandleAsync(long chatId)
        {
            _logger.LogInformation("Команда /start для {ChatId}", chatId);

            var user = await _apiClient.GetUserByChatIdAsync(chatId);
            if (user != null)
            {
                await _botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: $"Привет, {user.Surname} {user.Name}! Ты уже зарегистрирован!",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                    disableWebPagePreview: true);
                return;
            }

            await _botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "👋 Привет! Нажми <b>/begin</b> для того, чтобы начать",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                disableWebPagePreview: true);
        }
    }
}