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
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        // ExternalAPI

        public async Task<List<FacultyDto>> GetFacultiesAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/schedule/faculties");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var facultiesResponse = JsonSerializer.Deserialize<GetFacultiesResponse>(json, _jsonOptions);

            return facultiesResponse?.Data ?? new List<FacultyDto>();
        }

        public async Task<List<StudyGroupDto>> GetGroupsByFacultyAsync(int facultyId)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/schedule/faculties/{facultyId}/groups");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var groupsResponse = JsonSerializer.Deserialize<GetFacultyGroupsResponse>(json, _jsonOptions);

            return groupsResponse?.Data ?? new List<StudyGroupDto>();
        }

        // USER

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
                var user = JsonSerializer.Deserialize<UserByChatIdDto>(json, _jsonOptions);
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

        // Studio
        public async Task<Guid> CreateStudioAsync(CreateStudioBotRequestDTO dto, Guid userId)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/user/{userId}/studio", dto);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var studioResponse = JsonSerializer.Deserialize<CreateStudioResponse>(json, _jsonOptions);

            if (Guid.TryParse(studioResponse?.Data, out var studioId))
                return studioId;

            throw new FormatException($"Invalid studio ID: {studioResponse?.Data}");
        }

        public async Task<List<GetCategoriesResponseDTO>> GetCategoriesAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/studio/categories");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<GetCategoriesResponseDTO>>(json, _jsonOptions);
        }


        public async Task<GetUserStudiosResponse> GetStudiosAsLeaderAsync(Guid userId)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/user/studios_as_leader/{userId}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<GetUserStudiosResponse>(json, _jsonOptions);
        }



        // public async Task<StudioDetailDto> GetStudioDetailAsync(Guid studioId, Guid userId)
        // {
        //     var response = await _httpClient.GetAsync($"{_baseUrl}/api/studio/{studioId}");
        //     response.EnsureSuccessStatusCode();
        //     var json = await response.Content.ReadAsStringAsync();
        //     return JsonSerializer.Deserialize<StudioDetailDto>(json, _jsonOptions);
        // }

        // public async Task<List<UserResponseDto>> GetStudioUsersAsync(Guid studioId, Guid userId)
        // {
        //     var response = await _httpClient.GetAsync($"{_baseUrl}/api/studio/{studioId}/users");
        //     response.EnsureSuccessStatusCode();
        //     var json = await response.Content.ReadAsStringAsync();
        //     return JsonSerializer.Deserialize<List<UserResponseDto>>(json, _jsonOptions);
        // }


    }
}