using SQLite;

namespace WGU.C971.Models
{
    public class Course
    {
        [PrimaryKey, AutoIncrement] public int Id { get; set; }
        [Indexed] public int TermId { get; set; }

        [NotNull] public string Title { get; set; } = "";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime DueDate { get; set; }

        public CourseStatus Status { get; set; }

        [NotNull] public string InstructorName { get; set; } = "";
        [NotNull] public string InstructorEmail { get; set; } = "";
        [NotNull] public string InstructorPhone { get; set; } = "";

        public string? Notes { get; set; }
    }
}
