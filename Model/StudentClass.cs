namespace StudentMangement.Model
{
    public class StudentClass
    {
        public Guid StudentId { get; set; }
        public Student? Student { get; set; }
        public Guid ClassId { get; set; }
        public Class? Class { get; set; }
    }

}
