using Microsoft.Extensions.Caching.Memory;
using ScheduleMaster.TelegramBot.States;

namespace ScheduleMaster.Services
{
    public class UserRegistrationStateService
    {
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);

        public UserRegistrationStateService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public UserRegistrationState? GetState(long chatId)
        {
            _cache.TryGetValue($"reg:{chatId}", out UserRegistrationState? state);
            return state;
        }

        public void SetState(long chatId, UserRegistrationState state)
        {
            _cache.Set($"reg:{chatId}", state, _cacheExpiration);
        }

        public void RemoveState(long chatId)
        {
            _cache.Remove($"reg:{chatId}");
        }
    }
}
