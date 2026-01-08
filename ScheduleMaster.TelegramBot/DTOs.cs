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
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
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