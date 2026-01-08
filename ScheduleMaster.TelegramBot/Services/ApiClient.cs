using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using ScheduleMaster.TelegramBot.DTOs;

namespace ScheduleMaster.TelegramBot.Services
{

    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public ApiClient(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _baseUrl = config["Api:BaseUrl"] ?? "http://localhost:5003";
        }


        public async Task<List<FacultyDto>> GetFacultiesAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/schedule/faculties");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var facultiesResponse = JsonSerializer.Deserialize<GetFacultiesResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,  // ← camelCase → PascalCase
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase  // ← PascalCase → camelCase
            });

            return facultiesResponse?.Data ?? new List<FacultyDto>();
        }

        public async Task<List<StudyGroupDto>> GetGroupsByFacultyAsync(int facultyId)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/schedule/faculties/{facultyId}/groups");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var groupsResponse = JsonSerializer.Deserialize<GetFacultyGroupsResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,  // ← camelCase → PascalCase
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase  // ← PascalCase → camelCase
            });

            return groupsResponse?.Data ?? new List<StudyGroupDto>();
        }

        public async Task<bool> CreateUserAsync(CreateUserDto dto, long chatId)
        {
            var payload = new
            {
                surname = dto.Surname,
                name = dto.Name,
                middleName = dto.MiddleName,
                email = dto.Email,
                faculty = dto.Faculty,
                groupName = dto.GroupName,
                chatId = chatId,
                role = "user"
            };

            var response = await _httpClient.PostAsJsonAsync(
                $"{_baseUrl}/api/auth/register-bot",
                payload);

            return response.IsSuccessStatusCode;
        }

        public async Task<UserByChatIdDto?> GetUserByChatIdAsync(long chatId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/user/by-chat/{chatId}");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var user = JsonSerializer.Deserialize<UserByChatIdDto>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return user;
            }
            catch (HttpRequestException)
            {
                return null;
            }
            catch (JsonException)
            {
                return null;
            }
        }


    }
}