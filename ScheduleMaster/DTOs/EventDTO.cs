namespace ScheduleMaster.DTOs
{
    public class CreateEventRequestDTO
    {
        public string Title { get; set; } = null!;
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string? Location { get; set; }
    }


    public class UpdateEventRequestDTO
    {
        public string? Title { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public string? Location { get; set; }
    }

    public class EventResponseDTO
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string? Location { get; set; }
    }

}
