using Microsoft.Extensions.Logging;
using ScheduleMaster.TelegramBot.Handlers.Commands;

namespace ScheduleMaster.TelegramBot.Handlers
{

    public class CommandRouter
    {
        private readonly Dictionary<string, IBotCommandHandler> _handlers;
        private readonly ILogger<CommandRouter> _logger;

        public CommandRouter(IEnumerable<IBotCommandHandler> handlers, ILogger<CommandRouter> logger)
        {
            _handlers = handlers.ToDictionary(h => h.Command, h => h);
            _logger = logger;
        }

        public async Task<bool> TryHandleAsync(string command, long chatId)
        {
            if (_handlers.TryGetValue(command, out var handler))
            {
                _logger.LogInformation("Выполняем команду {Command} для {ChatId}", command, chatId);
                await handler.HandleAsync(chatId);
                return true;
            }

            _logger.LogDebug("Неизвестная команда: {Command}", command);
            return false;
        }
    }
}