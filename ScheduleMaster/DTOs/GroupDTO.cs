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

    public class GroupWithDetailsDTO
    {
        public Guid GroupId { get; set; }
        public required string GroupName { get; set; }
        public required string StudioName { get; set; }
        public int MemberCount { get; set; }
        public int ScheduleCount { get; set; }
    }
}