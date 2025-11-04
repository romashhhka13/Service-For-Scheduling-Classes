namespace ScheduleMaster.DTOs
{
    public class ScheduleDTO
    {
        public Guid Id { get; set; }
        public Guid StudioId { get; set; }
        public Guid? GroupId { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string? Location { get; set; }
        public string? WeekType { get; set; }
        public bool IsRecurring { get; set; }
    }

    public class CreateScheduleDTO
    {
        public Guid StudioId { get; set; }
        public Guid? GroupId { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string? Location { get; set; }
        public string? WeekType { get; set; }
        public bool IsRecurring { get; set; }
    }

    public class UpdateScheduleDTO
    {
        public Guid? GroupId { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public string? Location { get; set; }
        public string? WeekType { get; set; }
        public bool? IsRecurring { get; set; }
    }
}
