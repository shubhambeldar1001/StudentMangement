using StudentMangement.Contexts;
using System;

namespace StudentMangement.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StudentMangementContext _context;
        public IStudentRepository Students { get; }
        public IClassRepository Classes { get; }

        public UnitOfWork(StudentMangementContext context, IStudentRepository students, IClassRepository classes)
        {
            _context = context;
            Students = students;
            Classes = classes;
        }

        public async Task CompleteAsync() => await _context.SaveChangesAsync();
    }
}
