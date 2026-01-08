using Microsoft.Extensions.Logging;
using ScheduleMaster.Services;
using ScheduleMaster.TelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ScheduleMaster.TelegramBot.Handlers
{
    public class MessageHandler
    {
        private static readonly string[] AllowedUnregisteredCommands = { "/begin", "/start", "/menu", "/help" };

        private readonly TelegramBotClient _botClient;
        private readonly CommandRouter _commandRouter;
        private readonly UserRegistrationService _regService;
        private readonly MenuButtonHandler _menuHandler;
        private readonly UserRegistrationStateService _regStateService;
        private readonly ILogger<MessageHandler> _logger;
        private readonly ApiClient _apiClient;
        private readonly MenuService _menuService;

        public MessageHandler(
            TelegramBotClient botClient,
            CommandRouter commandRouter,
            UserRegistrationService regService,
            MenuButtonHandler menuHandler,
            UserRegistrationStateService regStateService,
            ILogger<MessageHandler> logger,
            ApiClient apiClient,
            MenuService menuService)
        {
            _botClient = botClient;
            _commandRouter = commandRouter;
            _regService = regService;
            _menuHandler = menuHandler;
            _regStateService = regStateService;
            _logger = logger;
            _apiClient = apiClient;
            _menuService = menuService;

        }

        public async Task HandleUpdateAsync(Update update, CancellationToken ct)
        {
            if (update.Message?.Text is { } text && update.Message.Chat.Id is { } chatId)
            {
                _logger.LogInformation("Сообщение '{Text}' от {ChatId}", text, chatId);

                // 1. Регистрация
                if (_regStateService.GetState(chatId) != null)
                {
                    await _regService.ProcessMessageAsync(chatId, text, ct);
                    return;
                }

                // 2. Команды
                if (await _commandRouter.TryHandleAsync(text, chatId))
                    return;

                // 3. Проверка user
                var user = await _apiClient.GetUserByChatIdAsync(chatId);
                if (user == null)
                {
                    await _menuService.ShowUnregisteredMenuAsync(chatId);
                    return;
                }

                // 4. Кнопки
                if (await _menuHandler.HandleButtonAsync(chatId, text))
                    return;

                // 5. Главное меню
                await _menuService.ShowMainMenuAsync(chatId);

            }
            else if (update.CallbackQuery != null)
            {
                await _regService.ProcessCallbackAsync(
                    update.CallbackQuery.Message!.Chat.Id,
                    update.CallbackQuery.Data ?? "", ct);
                await _botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, cancellationToken: ct);
            }
        }

    }
}
