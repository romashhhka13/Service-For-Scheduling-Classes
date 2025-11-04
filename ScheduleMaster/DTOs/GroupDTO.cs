namespace ScheduleMaster.DTOs
{
    public class GroupDTO
    {
        public Guid Id { get; set; }
        public Guid StudioId { get; set; }
        public required string Name { get; set; }
    }

    public class CreateGroupDTO
    {
        public Guid StudioId { get; set; }
        public required string Name { get; set; }
    }

    public class UpdateGroupDTO
    {
        public string? Name { get; set; }
    }
}