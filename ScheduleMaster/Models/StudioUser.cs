namespace ScheduleMaster.Models
{
    public class StudioUser
    {
        public Guid StudentId { get; set; }
        public Guid StudioId { get; set; }
        public required string StudioRole { get; set; }
    }

}