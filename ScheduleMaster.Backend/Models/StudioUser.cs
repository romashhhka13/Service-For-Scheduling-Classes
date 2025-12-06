namespace ScheduleMaster.Models
{
    public class StudioUser
    {
        public Guid StudentId { get; set; }
        public Guid StudioId { get; set; }
        public bool IsLeader { get; set; }
    }

}