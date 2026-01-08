using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using ScheduleMaster.TelegramBot.States;
using ScheduleMaster.TelegramBot.Services;
using ScheduleMaster.TelegramBot.DTOs;
using Microsoft.Extensions.Logging;
using ScheduleMaster.Services;
using ScheduleMaster.TelegramBot.Handlers.Commands;

namespace ScheduleMaster.TelegramBot.Services
{
    public class UserRegistrationService
    {
        private readonly TelegramBotClient _bot;
        private readonly UserRegistrationStateService _stateService;
        private readonly ApiClient _apiClient;
        private readonly ILogger<UserRegistrationService> _logger;
        private readonly MenuService _menuService;

        public UserRegistrationService(
            TelegramBotClient bot,
            UserRegistrationStateService stateService,
            ApiClient apiClient,
            ILogger<UserRegistrationService> logger,
            MenuService menuService)
        {
            _bot = bot;
            _stateService = stateService;
            _apiClient = apiClient;
            _logger = logger;
            _menuService = menuService;
        }

        public async Task ProcessMessageAsync(long chatId, string text, CancellationToken ct)
        {
            var state = _stateService.GetState(chatId);
            if (state == null)
            {
                _logger.LogWarning("Нет состояния регистрации для {ChatId}", chatId);
                return;
            }

            switch (state.Step)
            {
                case RegistrationStep.Surname:
                    await HandleSurnameAsync(chatId, text, state, ct);
                    break;
                case RegistrationStep.Name:
                    await HandleNameAsync(chatId, text, state, ct);
                    break;
                case RegistrationStep.MiddleName:
                    await HandleMiddleNameAsync(chatId, text, state, ct);
                    break;
                case RegistrationStep.Email:
                    await HandleEmailAsync(chatId, text, state, ct);
                    break;
                case RegistrationStep.Faculty:
                case RegistrationStep.Group:
                    await _bot.SendTextMessageAsync(chatId,
                        "👆 Используй кнопки ниже для выбора", cancellationToken: ct);
                    break;
            }
        }

        public async Task ProcessCallbackAsync(long chatId, string callbackData, CancellationToken ct)
        {
            var state = _stateService.GetState(chatId);
            if (state == null) return;

            if (callbackData.StartsWith("faculty:"))
            {
                await HandleFacultyCallbackAsync(chatId, callbackData, state, ct);
            }
            else if (callbackData.StartsWith("group:"))
            {
                await HandleGroupCallbackAsync(chatId, callbackData, state, ct);
            }
            else if (callbackData == "skip")
            {
                await HandleSkipAsync(chatId, state, ct);
            }
        }

        // Фамилия
        private async Task HandleSurnameAsync(long chatId, string surname, UserRegistrationState state, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(surname))
            {
                await _bot.SendTextMessageAsync(chatId, "❌ Фамилия обязательна! Попробуй ещё раз", cancellationToken: ct);
                return;
            }

            state.Surname = surname.Trim();
            state.Step = RegistrationStep.Name;
            _stateService.SetState(chatId, state);

            await _bot.SendTextMessageAsync(chatId,
                $"📝 <b>Твои данные:</b>\n\n" +
                $"Фамилия: <b>{state.Surname}</b>\n\n" +
                "Введите Имя:",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, cancellationToken: ct);
        }

        // Имя
        private async Task HandleNameAsync(long chatId, string name, UserRegistrationState state, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                await _bot.SendTextMessageAsync(chatId, "❌ Имя обязательно! Попробуй ещё раз", cancellationToken: ct);
                return;
            }

            state.Name = name.Trim();
            state.Step = RegistrationStep.MiddleName;
            _stateService.SetState(chatId, state);

            await _bot.SendTextMessageAsync(chatId,
                $"📝 <b>Твои данные:</b>\n\n" +
                $"Фамилия: <b>{state.Surname}</b>\n" +
                $"Имя: <b>{state.Name}</b>\n\n" +
                "Введите Отчество или <b>пропустить</b>:",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, cancellationToken: ct);

            // await SendMiddleNameInputAsync(chatId, state, ct);
        }

        // Отчество
        private async Task HandleMiddleNameAsync(long chatId, string middleName, UserRegistrationState state, CancellationToken ct)
        {
            state.MiddleName = middleName.Trim() != "пропустить" ? middleName.Trim() : null;
            state.Step = RegistrationStep.Email;
            _stateService.SetState(chatId, state);


            await _bot.SendTextMessageAsync(chatId,
                $"📝 <b>Твои данные:</b>\n\n" +
                $"Фамилия: <b>{state.Surname}</b>\n" +
                $"Имя: <b>{state.Name}</b>\n" +
                $"Отчество: <b>{state.MiddleName ?? "Пропущено"}</b>\n\n" +
                "Введите Email или <b>пропустить</b>:",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, cancellationToken: ct);
        }



        // Email
        private async Task HandleEmailAsync(long chatId, string email, UserRegistrationState state, CancellationToken ct)
        {
            state.Email = email.Trim() != "пропустить" ? email.Trim() : null;
            state.Step = RegistrationStep.Faculty;
            _stateService.SetState(chatId, state);

            await SendFacultySelectionAsync(chatId, state, ct);
        }



        private async Task SendFacultySelectionAsync(long chatId, UserRegistrationState state, CancellationToken ct)
        {
            try
            {
                var faculties = await _apiClient.GetFacultiesAsync();

                var keyboardRows = new List<List<InlineKeyboardButton>>();
                for (int i = 0; i < faculties.Count; i += 2) // 2 кнопки в ряд
                {
                    var row = faculties
                        .Skip(i)
                        .Take(2)
                        .Select(f => InlineKeyboardButton.WithCallbackData(
                            $"{f.Code} {f.Name}", $"faculty:{f.Id}"))
                        .ToList();
                    keyboardRows.Add(row);
                }

                // Добавляем "Пропустить" в последнюю строку
                if (keyboardRows.Any())
                {
                    keyboardRows[keyboardRows.Count - 1].Add(
                        InlineKeyboardButton.WithCallbackData("⏭️ Пропустить", "skip"));
                }

                var markup = new InlineKeyboardMarkup(keyboardRows);

                var summary = $"📝 <b>Твои данные:</b>\n\n" +
                              $"Фамилия: <b>{state.Surname}</b>\n" +
                              $"Имя: <b>{state.Name}</b>\n" +
                              $"Отчество: {state.MiddleName ?? "Не указано"}\n" +
                              $"Email: {state.Email ?? "Не указан"}\n\n" +
                              "🎓 Выберите факультет:";

                _logger.LogInformation("Отправляем {FacultyCount} факультетов для chatId {ChatId}",
                    faculties.Count, chatId);

                _logger.LogInformation("Markup rows: {RowsCount}", keyboardRows.Count);

                await _bot.SendTextMessageAsync(chatId, summary,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                    replyMarkup: markup, cancellationToken: ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения факультетов для {ChatId}", chatId);
                await _bot.SendTextMessageAsync(chatId,
                    "❌ Ошибка загрузки факультетов. Попробуй позже.", cancellationToken: ct);
            }
        }

        private async Task HandleFacultyCallbackAsync(long chatId, string callbackData, UserRegistrationState state, CancellationToken ct)
        {
            var facultyId = int.Parse(callbackData.Split(':')[1]);
            var faculties = await _apiClient.GetFacultiesAsync();
            var faculty = faculties.FirstOrDefault(f => f.Id == facultyId);

            if (faculty != null)
            {
                state.FacultyId = facultyId;
                state.Faculty = faculty.Name;
                state.Step = RegistrationStep.Group;
                _stateService.SetState(chatId, state);

                await SendGroupsSelectionAsync(chatId, state, ct);
            }
        }

        private async Task SendGroupsSelectionAsync(long chatId, UserRegistrationState state, CancellationToken ct)
        {
            try
            {
                var groups = await _apiClient.GetGroupsByFacultyAsync(state.FacultyId!.Value);

                var keyboardRows = new List<List<InlineKeyboardButton>>();
                for (int i = 0; i < groups.Count; i += 2)
                {
                    var row = groups
                        .Skip(i)
                        .Take(2)
                        .Select(g => InlineKeyboardButton.WithCallbackData(
                            $"{g.Code}", $"group:{g.Id}"))
                        .ToList();
                    keyboardRows.Add(row);
                }

                keyboardRows.Add(new List<InlineKeyboardButton>
                {
                    InlineKeyboardButton.WithCallbackData("⏭️ Пропустить", "skip")
                });

                var markup = new InlineKeyboardMarkup(keyboardRows);

                var summary = $"📝 <b>Твои данные:</b>\n\n" +
                              $"Фамилия: <b>{state.Surname}</b>\n" +
                              $"Имя: <b>{state.Name}</b>\n" +
                              $"Отчество: {state.MiddleName ?? "Не указано"}\n" +
                              $"Email: {state.Email ?? "Не указан"}\n" +
                              $"Факультет: <b>{state.Faculty}</b>\n\n" +
                              "👥 Выберите учебную группу:";

                await _bot.SendTextMessageAsync(chatId, summary,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                    replyMarkup: markup, cancellationToken: ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения групп для {ChatId}", chatId);
                await _bot.SendTextMessageAsync(chatId,
                    "❌ Ошибка загрузки групп. Попробуй позже.", cancellationToken: ct);
            }
        }

        private async Task HandleGroupCallbackAsync(long chatId, string callbackData, UserRegistrationState state, CancellationToken ct)
        {
            var groupId = int.Parse(callbackData.Split(':')[1]);
            var groups = await _apiClient.GetGroupsByFacultyAsync(state.FacultyId!.Value);
            var group = groups.FirstOrDefault(g => g.Id == groupId);

            if (group != null)
            {
                state.GroupId = groupId;
                state.GroupName = group.Code;
                state.Step = RegistrationStep.Complete;
                _stateService.SetState(chatId, state);

                await CompleteRegistrationAsync(chatId, state, ct);
            }
        }

        private async Task HandleSkipAsync(long chatId, UserRegistrationState state, CancellationToken ct)
        {
            if (state.Step == RegistrationStep.MiddleName)
            {
                state.Step = RegistrationStep.Email;
            }
            else if (state.Step == RegistrationStep.Email)
            {
                state.Step = RegistrationStep.Faculty;
            }
            else if (state.Step == RegistrationStep.Faculty)
            {
                state.Step = RegistrationStep.Complete;
            }
            else if (state.Step == RegistrationStep.Group)
            {
                state.Step = RegistrationStep.Complete;
            }
            _stateService.SetState(chatId, state);
            await CompleteRegistrationAsync(chatId, state, ct);
        }

        private async Task CompleteRegistrationAsync(long chatId, UserRegistrationState state, CancellationToken ct)
        {
            try
            {
                // Отправляем на бэк
                var success = await _apiClient.CreateUserAsync(new CreateUserDto
                {
                    Surname = state.Surname!,
                    Name = state.Name!,
                    MiddleName = state.MiddleName,
                    Email = state.Email,
                    Faculty = state.Faculty,
                    GroupName = state.GroupName
                }, chatId);

                if (success)
                {
                    var summary = $"✅ <b>Регистрация завершена!</b>\n\n" +
                                  $"<b>{state.Surname} {state.Name}</b>\n" +
                                  $"{state.MiddleName ?? "Не указано"}\n" +
                                  $"{state.Email ?? "Не указан"}\n" +
                                  $"🎓 {state.Faculty ?? "Не выбран"}\n" +
                                  $"👥 {state.GroupName ?? "Не выбрана"}\n\n" +
                                  "🎉 УРА!";

                    await _bot.SendTextMessageAsync(chatId, summary,
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, cancellationToken: ct);
                }
                else
                {
                    await _bot.SendTextMessageAsync(chatId,
                        "❌ Ошибка сохранения на сервере. Попробуй /begin заново.", cancellationToken: ct);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка завершения регистрации для {ChatId}", chatId);
                await _bot.SendTextMessageAsync(chatId,
                    "❌ Ошибка регистрации. Попробуй /begin заново.", cancellationToken: ct);
            }
            finally
            {
                _stateService.RemoveState(chatId);
                await _menuService.ShowMainMenuAsync(chatId);
            }
        }
    }
}
