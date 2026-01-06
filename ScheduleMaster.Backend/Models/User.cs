namespace ScheduleMaster.Models
{
    public class User
    {

        public Guid Id { get; set; }
        public required string Surname { get; set; }
        public required string Name { get; set; }
        public string? MiddleName { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public required string Role { get; set; }
        public string? Faculty { get; set; }
        public string? GroupName { get; set; }
        public long? ChatId { get; set; }
    }

}