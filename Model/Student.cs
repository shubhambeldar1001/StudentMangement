using System.ComponentModel.DataAnnotations;

namespace StudentMangement.Model
{
    public class Student
    {
        public Guid Id { get; set; }
        [Required, MaxLength(50)]
        //name is requred
        public required string FirstName { get; set; }
        [Required, MaxLength(50)]
        public required string LastName { get; set; }
        [Required, StringLength(10, MinimumLength = 10), RegularExpression(@"^[0-9]+$", ErrorMessage = "Phone number must be numeric.")]
        public required string PhoneNumber { get; set; }
        [Required, EmailAddress]
        public string? EmailId { get; set; }
        public ICollection<StudentClass>? StudentClasses { get; set; }
    }

}
