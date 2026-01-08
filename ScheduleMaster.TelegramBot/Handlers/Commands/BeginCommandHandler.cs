using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using ScheduleMaster.Services;
using ScheduleMaster.TelegramBot.States;
using ScheduleMaster.TelegramBot.Services;

namespace ScheduleMaster.TelegramBot.Handlers.Commands
{
    public class BeginCommandHandler : IBotCommandHandler
    {
        private readonly TelegramBotClient _botClient;
        private readonly ILogger<BeginCommandHandler> _logger;
        private readonly UserRegistrationStateService _stateService;
        private readonly ApiClient _apiClient;

        public string Command => "/begin";

        public BeginCommandHandler(
            TelegramBotClient botClient,
            ILogger<BeginCommandHandler> logger,
            UserRegistrationStateService stateService,
            ApiClient apiClient)
        {
            _botClient = botClient;
            _logger = logger;
            _stateService = stateService;
            _apiClient = apiClient;
        }

        public async Task HandleAsync(long chatId)
        {
            _logger.LogInformation("Команда /begin для {ChatId}", chatId);

            var existingUser = await _apiClient.GetUserByChatIdAsync(chatId);
            if (existingUser != null)
            {
                await _botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: $"Привет, {existingUser.Surname} {existingUser.Name}! Ты уже зарегистрирован!",
                    parseMode: ParseMode.Html);
                return;
            }

            var state = new UserRegistrationState
            {
                ChatId = chatId,
                Step = RegistrationStep.Surname
            };
            _stateService.SetState(chatId, state);

            await _botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "📝 <b>Регистрация</b>\n\n" +
                      "Введите Фамилию:",
                parseMode: ParseMode.Html);
        }
    }
}
