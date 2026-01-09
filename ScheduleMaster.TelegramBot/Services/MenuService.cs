using ScheduleMaster.TelegramBot.Constants;
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

        // Для незарегистрированных пользователей
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

        // Главное меню
        public async Task ShowMainMenuAsync(long chatId)
        {
            var user = await _apiClient.GetUserByChatIdAsync(chatId);

            if (user == null)
            {
                await ShowUnregisteredMenuAsync(chatId);
                return;
            }

            var state = new MenuState { ChatId = chatId, SelectedUserId = user.Id, CurrentStep = MenuStep.MainMenu };
            _stateService.SetState(chatId, state);

            var mainKeyboard2 = new ReplyKeyboardMarkup(new KeyboardButton[][]
            {
                new[] { new KeyboardButton(ButtonNames.Profile), new KeyboardButton(ButtonNames.Studios) },
                new[] { new KeyboardButton(ButtonNames.Calendar) }
            })
            { ResizeKeyboard = true };

            await _botClient.SendTextMessageAsync(chatId,
                $"🎉 Привет <b>{user.Name} {user.Surname}</b>!\n\nВыберите действие:",
                parseMode: ParseMode.Html, replyMarkup: mainKeyboard2);
        }

        // Профиль
        public async Task ShowProfileAsync(long chatId)
        {
            var state = new MenuState { ChatId = chatId, CurrentStep = MenuStep.Profile };
            _stateService.SetState(chatId, state);

            var profileKeyboard = new ReplyKeyboardMarkup(new KeyboardButton[][]
            {
                new[] { new KeyboardButton(ButtonNames.EditProfile), new KeyboardButton(ButtonNames.Back) }
            })
            { ResizeKeyboard = true };

            await _botClient.SendTextMessageAsync(chatId,
                "👤 <b>Профиль</b>\n\nЗдесь будет информация о тебе\n\n🔄 <i>Заглушка</i>",
                parseMode: ParseMode.Html, replyMarkup: profileKeyboard);
        }

        // Студии
        public async Task ShowStudiosMenuAsync(long chatId)
        {
            var state = new MenuState
            {
                ChatId = chatId,
                SelectedUserId = (await _apiClient.GetUserByChatIdAsync(chatId))?.Id,
                CurrentStep = MenuStep.Studios,
                StudioStep = ScheduleMaster.TelegramBot.States.StudioMenuStep.StudioMainMenu
            };
            _stateService.SetState(chatId, state);

            var studiosKeyboard = new ReplyKeyboardMarkup(new KeyboardButton[][]
            {
                new[] { new KeyboardButton(ButtonNames.CreateStudio), new KeyboardButton(ButtonNames.JoinStudio) },
                new[] { new KeyboardButton(ButtonNames.MyStudios), new KeyboardButton(ButtonNames.StudiosMember) },
                new[] { new KeyboardButton(ButtonNames.Back) }
            })
            { ResizeKeyboard = true };

            await _botClient.SendTextMessageAsync(chatId,
                "🏢 <b>Студии</b>\n\nВыбери действие:",
                parseMode: ParseMode.Html, replyMarkup: studiosKeyboard);
        }


        // Мои студии
        public async Task ShowMyStudiosAsync(long chatId)
        {
            var user = await _apiClient.GetUserByChatIdAsync(chatId);
            if (user?.Id == null) return;

            var studiosResponse = await _apiClient.GetStudiosAsLeaderAsync(user.Id);
            var studios = studiosResponse.Data ?? new();

            if (!studios.Any())
            {
                var keyboard = new ReplyKeyboardMarkup(new KeyboardButton[][]
                {
            new KeyboardButton[] { new KeyboardButton("➕ Создать студию") },
            new KeyboardButton[] { new KeyboardButton("◀️ Назад") }
                })
                { ResizeKeyboard = true };

                await _botClient.SendTextMessageAsync(chatId,
                    "<b>📚 Мои студии</b>\n\n<i>Нет студий — создайте первую!</i>",
                    parseMode: ParseMode.Html, replyMarkup: keyboard);
                return;
            }

            var inlineRows = new List<IEnumerable<InlineKeyboardButton>>();

            // Inline кнопки = студии (по 1 в ряд)
            foreach (var studio in studios)
            {
                inlineRows.Add(new[] { InlineKeyboardButton.WithCallbackData(studio.Title, $"studio_select:{studio.Id}") });
            }

            // Кнопка "Назад"
            // inlineRows.Add(new[] { InlineKeyboardButton.WithCallbackData("◀️ Назад", "studios_back") });

            var inlineKeyboard = new InlineKeyboardMarkup(inlineRows);

            // Reply клавиатура только "Создать"
            var replyKeyboard = new ReplyKeyboardMarkup(new KeyboardButton[][]
            {
        new KeyboardButton[] { new KeyboardButton("➕ Создать студию") }
            })
            { ResizeKeyboard = true };

            await _botClient.SendTextMessageAsync(chatId,
            $"<b>📚 Мои студии ({studios.Count})</b>",
            parseMode: ParseMode.Html,
            replyMarkup: inlineKeyboard);

        }




        // public async Task ShowStudioActionsAsync(long chatId, string studioTitle)
        // {
        //     var state = _stateService.GetState(chatId);
        //     state.StudioStep = StudioMenuStep.MyStudiosDetail;
        //     _stateService.SetState(chatId, state);

        //     var keyboard = new ReplyKeyboardMarkup(new KeyboardButton[][]
        //     {
        //         new[] { new KeyboardButton(ButtonNames.EditStudio), new KeyboardButton(ButtonNames.InviteStudio) },
        //         new[] { new KeyboardButton(ButtonNames.ShowMembers), new KeyboardButton(ButtonNames.CreateEvent) },
        //         new[] { new KeyboardButton(ButtonNames.DeleteStudio), new KeyboardButton(ButtonNames.Back) }
        //     })
        //     { ResizeKeyboard = true };
        //     ;

        //     await _botClient.SendTextMessageAsync(chatId,
        //         $"<b>⚙️ {studioTitle}</b>",
        //         parseMode: ParseMode.Html, replyMarkup: keyboard);
        // }


        // Календарь
        public async Task ShowCalendarMenuAsync(long chatId)
        {
            var state = new MenuState { ChatId = chatId, CurrentStep = MenuStep.Calendar };
            _stateService.SetState(chatId, state);

            var calendarKeyboard = new ReplyKeyboardMarkup(new KeyboardButton[][]
            {
                new[] { new KeyboardButton(ButtonNames.WeekEvents), new KeyboardButton(ButtonNames.DayEvents) },
                new[] { new KeyboardButton(ButtonNames.Back) }
            })
            { ResizeKeyboard = true };

            await _botClient.SendTextMessageAsync(chatId,
                "📅 <b>Календарь</b>\n\nВыбери период:",
                parseMode: ParseMode.Html, replyMarkup: calendarKeyboard);
        }

        // Кнопка назад возвращает в главное меню
        public async Task GoBackToMainAsync(long chatId)
        {
            await ShowMainMenuAsync(chatId);
        }
    }
}
