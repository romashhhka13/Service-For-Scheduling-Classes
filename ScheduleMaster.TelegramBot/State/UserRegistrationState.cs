namespace ScheduleMaster.TelegramBot.States
{
    public class UserRegistrationState
    {
        public string? Surname { get; set; }
        public string? Name { get; set; }
        public string? MiddleName { get; set; }
        public string? Email { get; set; }

        public string Role { get; set; } = "user";

        public string? Faculty { get; set; }
        public string? GroupName { get; set; }

        public int? FacultyId { get; set; }
        public int? GroupId { get; set; }

        public RegistrationStep Step { get; set; } = RegistrationStep.Surname;
        public long ChatId { get; set; }
    }

    public enum RegistrationStep
    {
        Surname,
        Name,
        MiddleName,
        Email,
        Faculty,
        Group,
        Complete
    }
}
