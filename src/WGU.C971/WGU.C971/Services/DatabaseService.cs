using SQLite;
using WGU.C971.Models;

namespace WGU.C971.Services
{
    public sealed class DatabaseService
    {
        private readonly SQLiteAsyncConnection _db;

        public DatabaseService()
        {
            var path = Path.Combine(FileSystem.AppDataDirectory, "c971.db3");
            _db = new SQLiteAsyncConnection(path);
        }

        public async Task InitAsync()
        {
            await _db.CreateTableAsync<Term>();
            await _db.CreateTableAsync<Course>();
            await _db.CreateTableAsync<Assessments>();
        }

        public Task<List<Term>> GetTermAsync() => _db.Table<Term>().ToListAsync();
        public Task<Term?> GetTermAsync(int id) => _db.FindAsync<Term>(id);
        public Task<int> SaveTermAsync(Term t) => t.Id == 0 ? _db.InsertAsync(t) : _db.UpdateAsync(t);
        public Task<int> DeleteTermAsync(Term t) => _db.DeleteAsync(t);

        public Task<List<Course>> GetCoursesForTermAsync(int termId) =>
            _db.Table<Course>().Where(c => c.TermId == termId).ToListAsync();
        public Task<int> GetCourseCountForTermAsync(int termId) =>
            _db.Table<Course>().Where(c => c.TermId == termId).CountAsync();
        public Task<Course?> GetCourseAsync(int id) => _db.FindAsync<Course>(id);
        public Task<int> SaveCourseAsync(Course c) => c.Id == 0 ? _db.InsertAsync(c) : _db.UpdateAsync(c);
        public Task<int> DeleteCourseAsync(Course c) => _db.DeleteAsync(c);

        public Task<List<Assessments>> GetAssessmentsForCourseAsync(int courseId) =>
            _db.Table<Assessments>().Where(a => a.CourseId == courseId).ToListAsync();
        public Task<int> SaveAssessmentAsync(Assessments a) => a.Id == 0 ? _db.InsertAsync(a) : _db.UpdateAsync(a);
        public Task<int> DeleteAssessmentAsync(Assessments a) => _db.DeleteAsync(a);
        public async Task<int> CountAssessmentsByTypeAsync(int courseId, AssessmentType type) =>
            await _db.Table<Assessments>().Where(a => a.CourseId == courseId && a.Type == type).CountAsync();
    }
}
