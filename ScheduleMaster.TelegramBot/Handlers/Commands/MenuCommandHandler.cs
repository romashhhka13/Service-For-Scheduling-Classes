using Microsoft.Extensions.Logging;
using ScheduleMaster.TelegramBot.Services;

namespace ScheduleMaster.TelegramBot.Handlers.Commands
{
    public class MenuCommandHandler : IBotCommandHandler
    {
        private readonly MenuService _menuService;
        private readonly ILogger<MenuCommandHandler> _logger;

        public string Command => "/menu";

        public MenuCommandHandler(MenuService menuService, ILogger<MenuCommandHandler> logger)
        {
            _menuService = menuService;
            _logger = logger;
        }

        public async Task HandleAsync(long chatId)
        {
            _logger.LogInformation("Команда /menu для {ChatId}", chatId);
            await _menuService.ShowMainMenuAsync(chatId);
        }
    }
}
