using Microsoft.Extensions.Caching.Memory;
using ScheduleMaster.TelegramBot.States;

namespace ScheduleMaster.TelegramBot.Services
{
    public class MenuStateService
    {
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromHours(2);

        public MenuStateService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public MenuState? GetState(long chatId)
        {
            _cache.TryGetValue($"menu:{chatId}", out MenuState? state);
            return state;
        }

        public void SetState(long chatId, MenuState state)
        {
            _cache.Set($"menu:{chatId}", state, _cacheExpiration);
        }

        public void RemoveState(long chatId)
        {
            _cache.Remove($"menu:{chatId}");
        }
    }
}
