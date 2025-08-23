using SQLite;

namespace WGU.C971.Models
{
    public class Assessments
    {
        [PrimaryKey, AutoIncrement] public int Id { get; set; }
        [Indexed] public int CourseId { get; set; }

        public AssessmentType Type { get; set; }
        [NotNull] public string Title { get; set; } = "Assessment";

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public bool StartAlertEnabled { get; set; }
        public bool EndAlertEnabled { get; set; }
        public int? StartAlertId { get; set; }
        public int? EndAlertId { get; set; }
    }
}
