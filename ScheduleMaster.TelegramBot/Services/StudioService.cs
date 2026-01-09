using Microsoft.Extensions.Logging;
using ScheduleMaster.TelegramBot.Constants;
using ScheduleMaster.TelegramBot.DTOs;
using ScheduleMaster.TelegramBot.Services;
using ScheduleMaster.TelegramBot.States;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Linq;


namespace ScheduleMaster.TelegramBot.Services
{
    public class StudioService
    {
        private readonly TelegramBotClient _bot;
        private readonly ApiClient _apiClient;
        private readonly MenuStateService _menuStateService;
        private readonly ILogger<StudioService> _logger;
        private readonly MenuService _menuService;

        public StudioService(TelegramBotClient bot, ApiClient apiClient,
            MenuStateService menuStateService, ILogger<StudioService> logger, MenuService menuService)
        {
            _bot = bot;
            _apiClient = apiClient;
            _menuStateService = menuStateService;
            _logger = logger;
            _menuService = menuService;
        }

        // создание студии
        public async Task HandleCreateStudioAsync(long chatId)
        {
            var state = _menuStateService.GetState(chatId) ?? new MenuState { ChatId = chatId };
            state.StudioStep = StudioMenuStep.CreateStudioTitle;
            _menuStateService.SetState(chatId, state);
            await _bot.SendTextMessageAsync(chatId, "📝 <b>Введите название студии:</b>", parseMode: ParseMode.Html);
        }

        public async Task ShowStudioCategoriesAsync(long chatId)
        {
            var categories = await _apiClient.GetCategoriesAsync();
            if (!categories.Any())
            {
                await _bot.SendTextMessageAsync(chatId, "❌ Категории не найдены.");
                return;
            }

            var rows = new List<IEnumerable<KeyboardButton>>();
            for (int i = 0; i < categories.Count; i += 2)
            {
                var row = new List<KeyboardButton>();
                row.Add(new KeyboardButton(categories[i].Category));  // Первая кнопка

                if (i + 1 < categories.Count)
                    row.Add(new KeyboardButton(categories[i + 1].Category));  // Вторая кнопка

                rows.Add(row);
            }

            var keyboard = new ReplyKeyboardMarkup(rows)
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };
            await _bot.SendTextMessageAsync(chatId, "🗂️ <b>Выберите категорию:</b>",
                parseMode: ParseMode.Html, replyMarkup: keyboard);
        }


        public async Task CreateStudioViaApiAsync(string title, long chatId)
        {
            var state = _menuStateService.GetState(chatId);
            if (state?.SelectedStudioCategoryId == null)
            {
                await _bot.SendTextMessageAsync(chatId, "❌ Категория не выбрана.");
                return;
            }

            var user = await _apiClient.GetUserByChatIdAsync(chatId);
            if (user?.Id == null)
            {
                await _bot.SendTextMessageAsync(chatId, "❌ Пользователь не найден.");
                return;
            }

            var dto = new CreateStudioBotRequestDTO
            {
                Title = title,
                CategoryId = state.SelectedStudioCategoryId.Value
            };

            try
            {
                var studioId = await _apiClient.CreateStudioAsync(dto, user.Id);
                await _bot.SendTextMessageAsync(chatId,
                    $"✅ <b>Студия '{title}'</b> создана!",
                    parseMode: ParseMode.Html);

                // Сброс состояния
                state.StudioStep = null;
                state.SelectedStudioCategoryId = null;
                state.PendingStudioName = null;
                _menuStateService.SetState(chatId, state);

                // await _menuService.ShowMyStudiosAsync(chatId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateStudio error. ChatId: {ChatId}, Title: {Title}", chatId, title);
                await _bot.SendTextMessageAsync(chatId, "❌ Ошибка создания студии. Попробуйте позже.");
            }
        }




        // public async Task HandleJoinStudioAsync(long chatId)
        // {

        // }


        public async Task HandleEditStudioAsync(long chatId)
        {
            await _bot.SendTextMessageAsync(chatId, "🔧 <b>Редактировать студию</b>\n\nСкоро...");
        }

        public async Task HandleInviteStudioAsync(long chatId)
        {
            await _bot.SendTextMessageAsync(chatId, "🔗 <b>Пригласить в студию</b>\n\nСкоро...");
        }

        public async Task HandleShowMembersAsync(long chatId)
        {
            await _bot.SendTextMessageAsync(chatId, "👥 <b>Участники студии</b>\n\nСкоро...");
        }

        public async Task HandleCreateEventAsync(long chatId)
        {
            await _bot.SendTextMessageAsync(chatId, "📅 <b>Создать событие</b>\n\nСкоро...");
        }

        public async Task HandleDeleteStudioAsync(long chatId)
        {
            await _bot.SendTextMessageAsync(chatId, "🗑️ <b>Удалить студию</b>\n\nСкоро...");
        }

    }
}
