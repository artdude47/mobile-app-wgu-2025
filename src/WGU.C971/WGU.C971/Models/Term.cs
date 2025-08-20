using SQLite;

namespace WGU.C971.Models
{
    public class Term
    {
        [PrimaryKey, AutoIncrement] public int Id { get; set; }
        [Indexed, NotNull] public string Title { get; set; } = "";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
