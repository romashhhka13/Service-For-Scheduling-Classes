namespace ScheduleMaster.DTOs
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        public required string Surname { get; set; }
        public required string Name { get; set; }
        public string? MiddleName { get; set; }
        public required string Email { get; set; }
        public required string Role { get; set; }
        public string? Faculty { get; set; }
        public string? GroupName { get; set; }
        public short? Course { get; set; }
    }

    public class CreateUserDTO
    {
        public required string Surname { get; set; }
        public required string Name { get; set; }
        public string? MiddleName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string Role { get; set; }
        public string? Faculty { get; set; }
        public string? GroupName { get; set; }
        public short? Course { get; set; }
    }

    public class UpdateUserDTO
    {
        public string? Surname { get; set; }
        public string? Name { get; set; }
        public string? MiddleName { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; } // ?
        public string? Faculty { get; set; }
        public string? GroupName { get; set; }
        public short? Course { get; set; }
    }

    // public class UpdatePasswordDTO
    // {
    //     public string OldPassword { get; set; } = null!;
    //     public string NewPassword { get; set; } = null!;
    // }

}