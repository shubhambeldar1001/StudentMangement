using System.ComponentModel.DataAnnotations;

namespace StudentMangement.Model
{
    public class Class
    {
        public Guid Id { get; set; }
        [Required, MaxLength(50)]
        public required string Name { get; set; }
        [MaxLength(100)]
        public string? Description { get; set; }
        public ICollection<StudentClass>? StudentClass { get; set; }
    }

}
