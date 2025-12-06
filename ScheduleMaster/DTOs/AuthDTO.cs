using System.ComponentModel.DataAnnotations;

namespace ScheduleMaster.DTOs
{
    public class RegisterRequestDTO
    {
        [MaxLength(254)]
        public required string Email { get; set; }
        [MaxLength(512)]
        public required string Password { get; set; }
        [MaxLength(64)]
        public required string Surname { get; set; }
        [MaxLength(64)]
        public required string Name { get; set; }
        [MaxLength(64)]
        public string? MiddleName { get; set; }
        [MaxLength(32)]
        public required string Role { get; set; } = "user";
        [MaxLength(255)]
        public string? Faculty { get; set; }
        [MaxLength(64)]
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
        [MaxLength(254)]
        public required string Email { get; set; }
        [MaxLength(512)]
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