using Microsoft.Extensions.Logging;
using ScheduleMaster.TelegramBot.Constants;
using ScheduleMaster.TelegramBot.Services;
using ScheduleMaster.TelegramBot.States;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ScheduleMaster.TelegramBot.Handlers
{
    public class MenuButtonHandler
    {
        private readonly MenuService _menuService;
        // private readonly MenuStateService _stateService;
        private readonly ILogger<MenuButtonHandler> _logger;
        private readonly TelegramBotClient _botClient;
        private readonly StudioService _studioService;
        // private readonly ApiClient _apiClient;

        public MenuButtonHandler(MenuService menuService, /*MenuStateService stateService,*/
         ILogger<MenuButtonHandler> logger, TelegramBotClient botClient, StudioService studioService)
        {
            _menuService = menuService;
            // _stateService = stateService;
            _logger = logger;
            _botClient = botClient;
            _studioService = studioService;
        }

        public async Task<bool> HandleButtonAsync(long chatId, string text)
        {
            _logger.LogInformation("Кнопка '{Text}' для {ChatId}", text, chatId);

            var knownButtons = new[]
            {
                ButtonNames.Profile, ButtonNames.Studios, ButtonNames.Calendar,
                ButtonNames.EditProfile, ButtonNames.Back,
                ButtonNames.CreateStudio, ButtonNames.JoinStudio,
                ButtonNames.MyStudios, ButtonNames.StudiosMember,
                ButtonNames.WeekEvents, ButtonNames.DayEvents
            };

            if (!knownButtons.Contains(text))
            {
                _logger.LogWarning("Неизвестная кнопка: {ButtonText} for {ChatId}", text, chatId);
                return false;
            }

            // var state = _stateService.GetState(chatId);

            // НАЗАД всегда в главное меню
            if (text == ButtonNames.Back)
            {
                await _menuService.GoBackToMainAsync(chatId);
                return true;
            }

            // Остальные кнопки
            switch (text)
            {
                case ButtonNames.Profile:
                    await _menuService.ShowProfileAsync(chatId);
                    break;
                case ButtonNames.Studios:
                    await _menuService.ShowStudiosMenuAsync(chatId);
                    break;
                case ButtonNames.Calendar:
                    await _menuService.ShowCalendarMenuAsync(chatId);
                    break;

                // 2.1. Создать студию
                case ButtonNames.CreateStudio:
                    await _studioService.HandleCreateStudioAsync(chatId);
                    return true;

                // 2.2.Просмотр студий
                case ButtonNames.MyStudios:
                    await _menuService.ShowMyStudiosAsync(chatId);
                    return true;

                // 2.2.1. Редактировать
                case ButtonNames.EditStudio:
                    // await studioService.HandleEditStudioAsync(chatId);
                    await _botClient.SendTextMessageAsync(chatId, "Редактировать — скоро!");
                    break;

                // 2.2.2. Пригласить
                case ButtonNames.InviteStudio:
                    await _studioService.HandleInviteStudioAsync(chatId);
                    break;

                // 2.2.3. Показать участников
                case ButtonNames.ShowMembers:
                    await _studioService.HandleShowMembersAsync(chatId);
                    break;

                // 2.2.4. Создать событие
                case ButtonNames.CreateEvent:
                    await _botClient.SendTextMessageAsync(chatId, "Создать событие — скоро!");
                    break;

                // 2.2.5. Удалить студию
                case ButtonNames.DeleteStudio:
                    // await studioService.HandleDeleteStudioAsync(chatId);
                    await _botClient.SendTextMessageAsync(chatId, "Удалить — скоро!");
                    break;


                default:
                    await _botClient.SendTextMessageAsync(chatId, "🔄 <i>Заглушка в разработке</i>",
                        parseMode: ParseMode.Html);
                    break;
            }

            return true;
        }
    }


}
