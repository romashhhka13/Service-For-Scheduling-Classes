using Microsoft.Extensions.Logging;
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
        private readonly MenuStateService _stateService;
        private readonly ILogger<MenuButtonHandler> _logger;
        private readonly TelegramBotClient _botClient;

        public MenuButtonHandler(MenuService menuService, MenuStateService stateService,
         ILogger<MenuButtonHandler> logger, TelegramBotClient botClient)
        {
            _menuService = menuService;
            _stateService = stateService;
            _logger = logger;
            _botClient = botClient;
        }

        public async Task<bool> HandleButtonAsync(long chatId, string text)
        {
            _logger.LogInformation("Кнопка '{Text}' для {ChatId}", text, chatId);

            var knownButtons = new[]
            {
                "👤 Профиль", "🏢 Студии", "📅 Календарь",
                "✏️ Редактировать", "◀️ Назад",
                "➕ Создать студию", "📝 Вступить в студию",
                "📋 Мои студии", "👥 Студии (участник)",
                "📋 События на неделю", "📅 События на день"
            };

            if (!knownButtons.Contains(text))
                return false;

            var state = _stateService.GetState(chatId);

            // НАЗАД всегда в главное меню
            if (text == "◀️ Назад")
            {
                await _menuService.GoBackToMainAsync(chatId);
                return true;
            }

            // Остальные кнопки
            switch (text)
            {
                case "👤 Профиль":
                    await _menuService.ShowProfileAsync(chatId);
                    break;
                case "🏢 Студии":
                    await _menuService.ShowStudiosMenuAsync(chatId);
                    break;
                case "📅 Календарь":
                    await _menuService.ShowCalendarMenuAsync(chatId);
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
