using Microsoft.EntityFrameworkCore;
using StudentMangement.Contexts;
using StudentMangement.Model;
using System;

namespace StudentMangement.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly StudentMangementContext _context;
        public StudentRepository(StudentMangementContext context)
        {
            _context = context;
        }

        public async Task<Student> GetByIdAsync(Guid id)
        {
            return await _context.Students.Include(s => s.StudentClasses).ThenInclude(sc => sc.Class).FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Student>> GetAllAsync()
        {
            return await _context.Students.Include(s => s.StudentClasses).ThenInclude(sc => sc.Class).ToListAsync();
        }

        public async Task AddAsync(Student student) => await _context.Students.AddAsync(student);

        public void Update(Student student) => _context.Students.Update(student);

        public void Delete(Student student) => _context.Students.Remove(student);

        public async Task<PaginatedList<Student>> GetStudentsAsync(int pageNumber, int pageSize, string searchQuery, string sortBy, bool isAscending)
        {
            var query = _context.Students.AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                query = query.Where(s =>
                    s.FirstName.Contains(searchQuery) ||
                    s.LastName.Contains(searchQuery) ||
                    s.EmailId.Contains(searchQuery));
            }

            // Sorting
            query = sortBy switch
            {
                "FirstName" => isAscending ? query.OrderBy(s => s.FirstName) : query.OrderByDescending(s => s.FirstName),
                "LastName" => isAscending ? query.OrderBy(s => s.LastName) : query.OrderByDescending(s => s.LastName),
                "EmailId" => isAscending ? query.OrderBy(s => s.EmailId) : query.OrderByDescending(s => s.EmailId),
                _ => query.OrderBy(s => s.Id) // Default sorting by Id
            };

            // Total Count
            var totalCount = await query.CountAsync();

            // Pagination
            var students = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedList<Student>(students, totalCount, pageNumber, pageSize);
        }
    }
}
