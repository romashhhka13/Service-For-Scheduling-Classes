
namespace ScheduleMaster.Models
{

    public class Studio
    {
        public Guid Id { get; set; }
        public Guid AdministratorId { get; set; }
        public required string Name { get; set; }
        public required string Category { get; set; }
    }

}