namespace ScheduleMaster.DTOs
{
    public class UserResponseDTO
    {
        public Guid Id { get; set; }
        public required string Surname { get; set; }
        public required string Name { get; set; }
        public string? MiddleName { get; set; }
        public required string Email { get; set; }
        public required string Role { get; set; }
        public required string Faculty { get; set; }
        public required string studyGroup { get; set; }
        public bool isLeader { get; set; }
    }

    public class UpdateUserRequestDTO
    {
        public string? Surname { get; set; }
        public string? Name { get; set; }
        public string? MiddleName { get; set; }
        public string? Email { get; set; }
        public string? Faculty { get; set; }
        public string? GroupName { get; set; }
    }
}