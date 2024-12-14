using StudentMangement.Model;

namespace StudentMangement.Repositories
{
    public interface IStudentRepository
    {
        Task<Student> GetByIdAsync(Guid id);
        Task<IEnumerable<Student>> GetAllAsync();
        Task<PaginatedList<Student>> GetStudentsAsync(int pageNumber, int pageSize, string searchQuery, string sortBy, bool isAscending);
        Task AddAsync(Student student);
        void Update(Student student);
        void Delete(Student student);
    }

}
