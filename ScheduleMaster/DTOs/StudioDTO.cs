namespace ScheduleMaster.DTOs
{
    public class StudioDTO
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Category { get; set; }
        public Guid AdministratorId { get; set; }
    }

    public class CreateStudioDTO
    {
        public required string Name { get; set; }
        public required string Category { get; set; }
        public Guid AdministratorId { get; set; }
    }

    public class UpdateStudioDTO
    {
        public string? Name { get; set; }
        public string? Category { get; set; }
    }
}