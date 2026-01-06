namespace ScheduleMaster.Services.ExternalApi
{
    public class FacultyResponse
    {
        public bool State { get; set; }
        public List<Faculty> Rows { get; set; } = new();
    }

    public class Faculty
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
    }

    public class FacultyGroupsResponse
    {
        public bool State { get; set; }
        public List<StudyGroup> Rows { get; set; } = new();
    }

    public class StudyGroup
    {
        public int Id { get; set; }                 // "id"
        public string Code { get; set; } = null!;   // "code"
        public int QualificationType { get; set; }  // "qualificationType"
        public bool HasSpecializations { get; set; }// "hasSpecializations"
        public int FacultyId { get; set; }          // "facultyId"
        public string DateBegin { get; set; } = null!; // "dateBegin"
        public string DateEnd { get; set; } = null!;   // "dateEnd"
    }




}