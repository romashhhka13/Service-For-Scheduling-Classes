namespace ScheduleMaster.DTOs
{
    public class CreateGroupRequestDTO
    {
        public string Title { get; set; } = null!;
        public Guid StudioId { get; set; }
    }

    public class UpdateGroupRequestDTO
    {
        public string? Title { get; set; }
    }
}