using System.Text.Json.Serialization;

namespace ScheduleMaster.TelegramBot.DTOs
{

    // USER //
    public class CreateUserDto
    {
        public required string Surname { get; set; }
        public required string Name { get; set; }
        public string? MiddleName { get; set; }
        public string? Email { get; set; }
        public string? Faculty { get; set; }
        public string? GroupName { get; set; }
    }

    public class UserByChatIdDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public string Surname { get; set; } = "";
        public bool IsLeader { get; set; }
    }

    public class UserResponseDto
    {
        public Guid Id { get; set; }
        public string Surname { get; set; } = "";
        public string Name { get; set; } = "";
        public string? MiddleName { get; set; }
        public string Email { get; set; } = "";
        public string Role { get; set; } = "";
        public string Faculty { get; set; } = "";
        public string StudyGroup { get; set; } = "";
        public bool IsLeader { get; set; }
    }


    // STUDIO

    public class CreateStudioBotRequestDTO
    {
        public string Title { get; set; } = string.Empty;
        public Guid? CategoryId { get; set; }
    }

    public class CreateStudioResponse  // Добавь в DTOs или ApiClient
    {
        [JsonPropertyName("data")]
        public string? Data { get; set; }
    }

    public class GetCategoriesResponseDTO
    {
        public Guid Id { get; set; }
        public required string Category { get; set; }
    }

    public class GetUserStudiosResponse
    {
        public string Message { get; set; } = "";
        public List<StudioDto> Data { get; set; } = new();
    }

    public class StudioDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = "";
        public Guid? StudioCategoryId { get; set; }
        public int MemberCount { get; set; }
        public bool CurrentUserIsLeader { get; set; }
    }

    public class StudioDetailDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = "";
        public int MemberCount { get; set; }
        public bool CurrentUserIsLeader { get; set; }
        public List<UserResponseDto>? Members { get; set; }
    }



    // ExternalAPI
    public class GetFacultiesResponse
    {
        public string Message { get; set; } = "";
        public List<FacultyDto> Data { get; set; } = new();
    }

    public class FacultyDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Code { get; set; } = "";
    }

    public class GetFacultyGroupsResponse
    {
        public string Message { get; set; } = "";
        public List<StudyGroupDto> Data { get; set; } = new();
    }

    public class StudyGroupDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public int QualificationType { get; set; }
        public bool HasSpecializations { get; set; }
        public int FacultyId { get; set; }
        public string DateBegin { get; set; } = string.Empty;
        public string DateEnd { get; set; } = string.Empty;
    }
}