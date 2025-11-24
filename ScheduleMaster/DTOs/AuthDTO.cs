namespace ScheduleMaster.DTOs
{
    public class RegisterRequestDTO
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string Surname { get; set; }
        public required string Name { get; set; }
        public string? MiddleName { get; set; }
        public required string Role { get; set; } = "user";
        public string? Faculty { get; set; }
        public string? GroupName { get; set; }
    }

    public class RegisterResponseDTO
    {
        public required string Email { get; set; }
        public required string Surname { get; set; }
        public required string Name { get; set; }
        public string? MiddleName { get; set; }
        public required string Role { get; set; }
        public string? Faculty { get; set; }
        public string? GroupName { get; set; }
    }

    public class LoginRequestDTO
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class LoginResponseDTO
    {
        public Guid UserId { get; set; }
        public required string Email { get; set; }
        public required string Role { get; set; }
        public required string Token { get; set; }
        public int ExpiresIn { get; set; }
    }
}