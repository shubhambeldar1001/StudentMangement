using Microsoft.EntityFrameworkCore;
using StudentMangement.Contexts;
using StudentMangement.Model;
using System;

namespace StudentMangement.Repositories
{
    public class ClassRepository : IClassRepository
    {
        private readonly StudentMangementContext _context;
        public ClassRepository(StudentMangementContext context) => _context = context;

        public async Task<Class> GetByIdAsync(Guid id) => await _context.Classes.Include(c => c.StudentClass).ThenInclude(sc => sc.Student).FirstOrDefaultAsync(c => c.Id == id);

        public async Task<IEnumerable<Class>> GetAllAsync() => await _context.Classes.Include(c => c.StudentClass).ThenInclude(sc => sc.Student).ToListAsync();

        public async Task AddAsync(Class @class) => await _context.Classes.AddAsync(@class);

        public void Update(Class @class) => _context.Classes.Update(@class);

        public void Delete(Class @class) => _context.Classes.Remove(@class);
    }
}
