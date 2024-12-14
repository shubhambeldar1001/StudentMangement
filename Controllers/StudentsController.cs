using Microsoft.AspNetCore.Mvc;
using StudentMangement.Model;
using StudentMangement.Repositories;

namespace StudentMangement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStudentRepository _studentRepository;

        public StudentsController(IUnitOfWork unitOfWork,IStudentRepository studentRepository)
        {
            _unitOfWork = unitOfWork;
            _studentRepository = studentRepository; 
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var student = await _unitOfWork.Students.GetByIdAsync(id);
            if (student == null) return NotFound();
            return Ok(student);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Student student)
        {
            await _unitOfWork.Students.AddAsync(student);
            await _unitOfWork.CompleteAsync();
            return CreatedAtAction(nameof(GetById), new { id = student.Id }, student);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, Student student)
        {
            var existingStudent = await _unitOfWork.Students.GetByIdAsync(id);
            if (existingStudent == null) return NotFound();

            existingStudent.FirstName = student.FirstName;
            existingStudent.LastName = student.LastName;
            existingStudent.PhoneNumber = student.PhoneNumber;
            existingStudent.EmailId = student.EmailId;
            existingStudent.StudentClasses = student.StudentClasses;

            _unitOfWork.Students.Update(existingStudent);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var student = await _unitOfWork.Students.GetByIdAsync(id);
            if (student == null) return NotFound();

            _unitOfWork.Students.Delete(student);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(_studentRepository.GetAllAsync);
        }

        [HttpGet("by-class/{classId}")]
        public async Task<IActionResult> GetStudentsByClass(Guid classId)
        {
            var @class = await _unitOfWork.Classes.GetByIdAsync(classId);
            if (@class == null)
                return NotFound($"Class with ID {classId} not found.");

            var students = @class.StudentClass.Select(sc => sc.Student).ToList();
            return Ok(students);
        }

        [HttpPost("{studentId}/assign-class/{classId}")]
        public async Task<IActionResult> AssignStudentToClass(Guid studentId, Guid classId)
        {
            var student = await _unitOfWork.Students.GetByIdAsync(studentId);
            if (student == null)
                return NotFound($"Student with ID {studentId} not found.");

            var @class = await _unitOfWork.Classes.GetByIdAsync(classId);
            if (@class == null)
                return NotFound($"Class with ID {classId} not found.");

            // Check if already assigned
            if (student.StudentClasses.Any(sc => sc.ClassId == classId))
                return BadRequest("Student is already assigned to this class.");

            var studentClass = new StudentClass { StudentId = studentId, ClassId = classId };
            student.StudentClasses.Add(studentClass);

            await _unitOfWork.CompleteAsync();

            return Ok($"Student {student.FirstName} {student.LastName} assigned to class {@class.Name}.");
        }

        [HttpDelete("{studentId}/remove-class/{classId}")]
        public async Task<IActionResult> RemoveStudentFromClass(Guid studentId, Guid classId)
        {
            var student = await _unitOfWork.Students.GetByIdAsync(studentId);
            if (student == null)
                return NotFound($"Student with ID {studentId} not found.");

            var @class = await _unitOfWork.Classes.GetByIdAsync(classId);
            if (@class == null)
                return NotFound($"Class with ID {classId} not found.");

            var studentClass = student.StudentClasses.FirstOrDefault(sc => sc.ClassId == classId);
            if (studentClass == null)
                return BadRequest("Student is not assigned to this class.");

            student.StudentClasses.Remove(studentClass);

            await _unitOfWork.CompleteAsync();

            return Ok($"Student {student.FirstName} {student.LastName} removed from class {@class.Name}.");
        }

        [HttpGet("paginated")]
        public async Task<IActionResult> GetStudentsPaginated(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string searchQuery = null,
            [FromQuery] string sortBy = "FirstName",
            [FromQuery] bool isAscending = true)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                return BadRequest("PageNumber and PageSize must be greater than zero.");

            var result = await _unitOfWork.Students.GetStudentsAsync(pageNumber, pageSize, searchQuery, sortBy, isAscending);

            return Ok(new
            {
                result.Items,
                result.TotalCount,
                result.PageNumber,
                result.PageSize,
                result.TotalPages
            });
        }

    }
}
