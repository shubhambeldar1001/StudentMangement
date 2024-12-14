using Microsoft.EntityFrameworkCore;
using StudentMangement.Model;
using System.Collections.Generic;
//using System.Data.Entity;
using System.Reflection.Emit;

namespace StudentMangement.Contexts
{
    public class StudentMangementContext : DbContext
    {
            public StudentMangementContext(DbContextOptions<StudentMangementContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; }
        public DbSet<Class> Classes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
            modelBuilder.Entity<Student>()
                  .HasIndex(s => s.PhoneNumber)
                  .IsUnique();

            modelBuilder.Entity<Student>()
                .HasIndex(s => s.EmailId)
                .IsUnique();

            modelBuilder.Entity<StudentClass>()
                .HasKey(sc => new { sc.StudentId, sc.ClassId });

            modelBuilder.Entity<StudentClass>()
                .HasOne(sc => sc.Student)
                .WithMany(s => s.StudentClasses)
                .HasForeignKey(sc => sc.StudentId);

            modelBuilder.Entity<StudentClass>()
                .HasOne(sc => sc.Class)
                .WithMany(c => c.StudentClass)
                .HasForeignKey(sc => sc.ClassId);
        }
        }
}
