using System.Text.Json;
using Microsoft.Extensions.Configuration;
using ScheduleMaster.Services.Cache;

namespace ScheduleMaster.Services.ExternalApi
{

    public class ScheduleApiService : IScheduleApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ICacheService _cacheService;
        private readonly string _baseUrl;
        private readonly int _cacheExpirationHours;

        // Ключи для Redis
        private const string FacultiesCacheKey = "faculties:all";
        private const string GroupsCacheKeyPrefix = "groups:faculty:";

        public ScheduleApiService(
        HttpClient httpClient,
        ICacheService cacheService,
        IConfiguration configuration)
        {
            _httpClient = httpClient;
            _cacheService = cacheService;

            _baseUrl = configuration["ExternalApi:ScheduleApi:BaseUrl"]
                ?? throw new ArgumentNullException("ExternalApi:ScheduleApi:BaseUrl");
            _cacheExpirationHours = int.Parse(
                configuration["ExternalApi:ScheduleApi:CacheExpirationHours"] ?? "24");

            // Основные заголовки, как у браузера
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/142.0.0.0 Safari/537.36");
            _httpClient.DefaultRequestHeaders.Referrer = new Uri("https://lk.gubkin.ru/schedule/");
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
            _httpClient.DefaultRequestHeaders.Add("Accept-Language", "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");

        }

        private async Task EnsureValidCookiesAsync()
        {
            var cookieCacheKey = "schedule:cookies";
            var cachedCookies = await _cacheService.GetAsync<string>(cookieCacheKey);

            if (!string.IsNullOrWhiteSpace(cachedCookies))
            {
                if (await TestCookiesAsync(cachedCookies))
                {
                    _httpClient.DefaultRequestHeaders.Remove("Cookie");
                    _httpClient.DefaultRequestHeaders.Add("Cookie", cachedCookies);
                    return;
                }
            }

            await RefreshCookiesAsync();
        }

        private async Task<bool> TestCookiesAsync(string cookies)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Remove("Cookie");
                _httpClient.DefaultRequestHeaders.Add("Cookie", cookies);

                var testUrl = $"{_baseUrl}?act=list&method=getFaculties";
                var response = await _httpClient.GetAsync(testUrl);

                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        private async Task RefreshCookiesAsync()
        {
            // Ходим на главную страницу расписания — получаем базовые куки
            var mainPageUrl = "https://lk.gubkin.ru/schedule/";
            var mainResponse = await _httpClient.GetAsync(mainPageUrl);

            // Извлекаем куки из ответа
            var cookieHeader = mainResponse.Headers.GetValues("Set-Cookie") ?? Enumerable.Empty<string>();
            var cookies = string.Join("; ", cookieHeader);

            // Сохраняем в Redis на 1 час (с запасом)
            await _cacheService.SetAsync("schedule:cookies", cookies, TimeSpan.FromHours(1));

            _httpClient.DefaultRequestHeaders.Remove("Cookie");
            _httpClient.DefaultRequestHeaders.Add("Cookie", cookies);
        }



        public async Task<List<Faculty>> GetFacultiesAsync()
        {

            await EnsureValidCookiesAsync();

            var cached = await _cacheService.GetAsync<List<Faculty>>(FacultiesCacheKey);
            if (cached != null)
                return cached;

            var url = $"{_baseUrl}?act=list&method=getFaculties";

            var response = await _httpClient.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();

            var wrapper = JsonSerializer.Deserialize<FacultyResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var faculties = wrapper?.Rows ?? new List<Faculty>();

            await _cacheService.SetAsync(
                FacultiesCacheKey,
                faculties,
                TimeSpan.FromHours(_cacheExpirationHours));

            return faculties;
        }


        public async Task<List<StudyGroup>> GetGroupsByFacultyAsync(int facultyId)
        {

            await EnsureValidCookiesAsync();

            var cacheKey = $"{GroupsCacheKeyPrefix}{facultyId}";

            var cachedGroups = await _cacheService.GetAsync<List<StudyGroup>>(cacheKey);
            if (cachedGroups != null)
                return cachedGroups;

            var url = $"{_baseUrl}?act=list&method=getFacultyGroups&facultyId={facultyId}";

            var response = await _httpClient.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();

            var wrapper = JsonSerializer.Deserialize<FacultyGroupsResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var groups = wrapper?.Rows ?? new List<StudyGroup>();

            await _cacheService.SetAsync(
                cacheKey,
                groups,
                TimeSpan.FromHours(_cacheExpirationHours)
            );

            return groups;
        }

    }
}