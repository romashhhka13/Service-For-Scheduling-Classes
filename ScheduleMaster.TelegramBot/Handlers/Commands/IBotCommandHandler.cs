
namespace ScheduleMaster.TelegramBot.Handlers.Commands
{

    public interface IBotCommandHandler
    {
        string Command { get; }
        Task HandleAsync(long chatId);
    }
}