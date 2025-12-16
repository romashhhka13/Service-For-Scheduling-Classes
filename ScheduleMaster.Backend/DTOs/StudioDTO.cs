namespace ScheduleMaster.DTOs
{

    public class CreateStudioRequestDTO
    {
        public string Title { get; set; } = null!;
        public Guid? CategoryId { get; set; }
    }

    public class UpdateStudioRequestDTO
    {
        public string? Title { get; set; }
        public Guid? CategoryId { get; set; }
    }

    public class StudioResponseDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public Guid? CategoryId { get; set; }
        public int MemberCount { get; set; }
        public bool currentUserIsLeader { get; set; }
    }

    public class GetCategoriesResponseDTO
    {
        public Guid Id { get; set; }
        public required string Category { get; set; }
    }

}