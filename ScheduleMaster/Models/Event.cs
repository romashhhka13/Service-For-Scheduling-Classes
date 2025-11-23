namespace ScheduleMaster.Models
{

    public class Event
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string? Location { get; set; }
    }
}