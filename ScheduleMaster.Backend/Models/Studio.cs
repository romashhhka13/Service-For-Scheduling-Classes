
namespace ScheduleMaster.Models
{

    public class Studio
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public Guid? StudioCategoryId { get; set; }
    }

}