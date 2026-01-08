using ScheduleMaster.TelegramBot.DTOs;
using ScheduleMaster.TelegramBot.States;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ScheduleMaster.TelegramBot.Services
{
    public class MenuService
    {
        private readonly TelegramBotClient _botClient;
        private readonly ApiClient _apiClient;
        private readonly MenuStateService _stateService;

        public MenuService(TelegramBotClient botClient, ApiClient apiClient, MenuStateService stateService)
        {
            _botClient = botClient;
            _apiClient = apiClient;
            _stateService = stateService;
        }

        public async Task ShowUnregisteredMenuAsync(long chatId)
        {
            var keyboard = new ReplyKeyboardMarkup(new KeyboardButton[][]
            {
                new[] { new KeyboardButton("/begin") }
            })
            { ResizeKeyboard = true };

            await _botClient.SendTextMessageAsync(chatId,
                "👋 Привет! Нажми <b>/begin</b> чтобы начать регистрацию 👇",
                parseMode: ParseMode.Html, replyMarkup: keyboard);
        }

        public async Task ShowMainMenuAsync(long chatId)
        {
            var user = await _apiClient.GetUserByChatIdAsync(chatId);

            if (user == null)
            {
                await ShowUnregisteredMenuAsync(chatId); // ← Редирект!
                return;
            }

            var state = new MenuState { ChatId = chatId, CurrentStep = MenuStep.MainMenu };
            _stateService.SetState(chatId, state);

            var mainKeyboard2 = new ReplyKeyboardMarkup(new KeyboardButton[][]
            {
                new[] { new KeyboardButton("👤 Профиль"), new KeyboardButton("🏢 Студии") },
                new[] { new KeyboardButton("📅 Календарь") }
            })
            { ResizeKeyboard = true };

            await _botClient.SendTextMessageAsync(chatId,
                $"🎉 Привет <b>{user.Name} {user.Surname}</b>!\n\nВыберите действие:",
                parseMode: ParseMode.Html, replyMarkup: mainKeyboard2);
        }

        public async Task ShowProfileAsync(long chatId)
        {
            var state = new MenuState { ChatId = chatId, CurrentStep = MenuStep.Profile };
            _stateService.SetState(chatId, state);

            var profileKeyboard = new ReplyKeyboardMarkup(new KeyboardButton[][]
            {
                new[] { new KeyboardButton("✏️ Редактировать"), new KeyboardButton("◀️ Назад") }
            })
            { ResizeKeyboard = true };

            await _botClient.SendTextMessageAsync(chatId,
                "👤 <b>Профиль</b>\n\nЗдесь будет информация о тебе\n\n🔄 <i>Заглушка</i>",
                parseMode: ParseMode.Html, replyMarkup: profileKeyboard);
        }

        public async Task ShowStudiosMenuAsync(long chatId)
        {
            var state = new MenuState
            {
                ChatId = chatId,
                CurrentStep = MenuStep.Studios,
                StudioStep = ScheduleMaster.TelegramBot.States.StudioMenuStep.StudioMainMenu
            };
            _stateService.SetState(chatId, state);

            var studiosKeyboard = new ReplyKeyboardMarkup(new KeyboardButton[][]
            {
                new[] { new KeyboardButton("➕ Создать студию"), new KeyboardButton("📝 Вступить в студию") },
                new[] { new KeyboardButton("📋 Мои студии"), new KeyboardButton("👥 Студии (участники)") },
                new[] { new KeyboardButton("◀️ Назад") }
            })
            { ResizeKeyboard = true };

            await _botClient.SendTextMessageAsync(chatId,
                "🏢 <b>Студии</b>\n\nВыбери действие:",
                parseMode: ParseMode.Html, replyMarkup: studiosKeyboard);
        }

        public async Task ShowCalendarMenuAsync(long chatId)
        {
            var state = new MenuState { ChatId = chatId, CurrentStep = MenuStep.Calendar };
            _stateService.SetState(chatId, state);

            var calendarKeyboard = new ReplyKeyboardMarkup(new KeyboardButton[][]
            {
                new[] { new KeyboardButton("📋 События на неделю"), new KeyboardButton("📅 События на день") },
                new[] { new KeyboardButton("◀️ Назад") }
            })
            { ResizeKeyboard = true };

            await _botClient.SendTextMessageAsync(chatId,
                "📅 <b>Календарь</b>\n\nВыбери период:",
                parseMode: ParseMode.Html, replyMarkup: calendarKeyboard);
        }

        public async Task GoBackToMainAsync(long chatId)
        {
            await ShowMainMenuAsync(chatId);
        }
    }
}
