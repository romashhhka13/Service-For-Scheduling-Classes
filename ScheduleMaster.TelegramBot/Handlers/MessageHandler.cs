using Microsoft.Extensions.Logging;
using ScheduleMaster.Services;
using ScheduleMaster.TelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using ScheduleMaster.TelegramBot.States;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.Enums;

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
        private readonly MenuStateService _menuStateService;
        private readonly StudioService _studioService;



        public MessageHandler(
            TelegramBotClient botClient,
            CommandRouter commandRouter,
            UserRegistrationService regService,
            MenuButtonHandler menuHandler,
            UserRegistrationStateService regStateService,
            ILogger<MessageHandler> logger,
            ApiClient apiClient,
            MenuService menuService,
            MenuStateService menuStateService,
            StudioService studioService)
        {
            _botClient = botClient;
            _commandRouter = commandRouter;
            _regService = regService;
            _menuHandler = menuHandler;
            _regStateService = regStateService;
            _logger = logger;
            _apiClient = apiClient;
            _menuService = menuService;
            _menuStateService = menuStateService;
            _studioService = studioService;

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

                // 5. Создание студии
                var menuState = _menuStateService.GetState(chatId);
                if (menuState?.StudioStep == StudioMenuStep.CreateStudioTitle)
                {
                    menuState.PendingStudioName = text.Trim();
                    menuState.StudioStep = StudioMenuStep.CreateStudioCategory;
                    _menuStateService.SetState(chatId, menuState);
                    await _studioService.ShowStudioCategoriesAsync(chatId);
                    return;
                }

                if (menuState?.StudioStep == StudioMenuStep.CreateStudioCategory)
                {
                    var categories = await _apiClient.GetCategoriesAsync();
                    var selected = categories.FirstOrDefault(c => c.Category == text);
                    if (selected == null)
                    {
                        await _botClient.SendTextMessageAsync(chatId, "❌ Выберите категорию из списка.");
                        return;
                    }
                    menuState.SelectedStudioCategoryId = selected.Id;
                    _menuStateService.SetState(chatId, menuState);
                    await _studioService.CreateStudioViaApiAsync(menuState.PendingStudioName!, chatId);
                    return;
                }



                // 6. Главное меню
                await _menuService.ShowMainMenuAsync(chatId);

            }
            else if (update.CallbackQuery != null)
            {
                await _regService.ProcessCallbackAsync(
                    update.CallbackQuery.Message!.Chat.Id,
                    update.CallbackQuery.Data ?? "", ct);
                await _botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, cancellationToken: ct);

                var data = update.CallbackQuery.Data ?? "";
                if (data == "studios_back")
                {
                    await _menuService.ShowStudiosMenuAsync(update.CallbackQuery.Message.Chat.Id);
                }
                else if (data.StartsWith("studio_select:"))
                {
                    var studioIdString = data.Replace("studio_select:", "");
                    if (Guid.TryParse(studioIdString, out var studioId))
                    {
                        var state = _menuStateService.GetState(update.CallbackQuery.Message.Chat.Id);
                        state.SelectedStudioId = studioId;  // Guid? или int?
                        _menuStateService.SetState(update.CallbackQuery.Message.Chat.Id, state);
                    }
                }
            }
        }

    }
}
