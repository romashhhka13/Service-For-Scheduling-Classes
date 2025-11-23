namespace ScheduleMaster.Models
{

    public class Group
    {
        public Guid Id { get; set; }
        public Guid StudioId { get; set; }
        public required string Title { get; set; }
    }
}