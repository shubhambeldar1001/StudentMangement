using StudentMangement.Model;

namespace StudentMangement.Repositories
{
    public interface IClassRepository
    {
        Task<Class> GetByIdAsync(Guid id);
        Task<IEnumerable<Class>> GetAllAsync();
        Task AddAsync(Class @class);
        void Update(Class @class);
        void Delete(Class @class);
    }
}
