using SQLite;

namespace WGU.C971.Models
{
    public class Assessments
    {
        [PrimaryKey, AutoIncrement] public int Id { get; set; }
        [Indexed] public int CourseId { get; set; }

        public AssessmentType Type { get; set; }
        [NotNull] public string Name { get; set; } = "";

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
