using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using StudentMangement.Model;
using StudentMangement.Repositories;
using System.Globalization;

[ApiController]
[Route("api/[controller]")]
public class ImportStudentsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _environment;

    public ImportStudentsController(IUnitOfWork unitOfWork, IWebHostEnvironment environment)
    {
        _unitOfWork = unitOfWork;
        _environment = environment;
    }

    [HttpPost("bulk-import")]
    public async Task<IActionResult> BulkImport(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is required.");
        if (file.Length > 5 * 1024 * 1024) 
            return BadRequest("File size exceeds the 5 MB limit.");

        var importedStudents = new List<Student>();
        var errors = new List<string>();

        try
        {
            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                BadDataFound = null,
                HeaderValidated = null,
            });

            var csvRecords = csv.GetRecords<ImportStudentModel>().ToList();

            foreach (var record in csvRecords)
            {
                if (!IsValidStudent(record, out var validationError))
                {
                    errors.Add($"Row {csvRecords.IndexOf(record) + 1}: {validationError}");
                    continue;
                }

                var student = new Student
                {
                    Id = Guid.NewGuid(),
                    FirstName = record.FirstName,
                    LastName = record.LastName,
                    PhoneNumber = record.PhoneNumber,
                    EmailId = record.EmailId,
                };

                // Validate duplicates
                var exists = await _unitOfWork.Students.GetAllAsync();
                if (exists.Any(s => s.PhoneNumber == student.PhoneNumber || s.EmailId == student.EmailId))
                {
                    errors.Add($"Row {csvRecords.IndexOf(record) + 1}: Duplicate PhoneNumber or EmailId.");
                    continue;
                }

                importedStudents.Add(student);
            }

            // Add valid students
            foreach (var student in importedStudents)
            {
                await _unitOfWork.Students.AddAsync(student);
            }
            await _unitOfWork.CompleteAsync();

            return Ok(new
            {
                Message = "Import completed.",
                SuccessfulImports = importedStudents.Count,
                Errors = errors
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    private bool IsValidStudent(ImportStudentModel model, out string errorMessage)
    {
        errorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(model.FirstName) || string.IsNullOrWhiteSpace(model.LastName))
        {
            errorMessage = "FirstName and LastName are required.";
            return false;
        }
        if (!long.TryParse(model.PhoneNumber, out _) || model.PhoneNumber.Length != 10)
        {
            errorMessage = "PhoneNumber must be a 10-digit number.";
            return false;
        }
        if (!IsValidEmail(model.EmailId))
        {
            errorMessage = "Invalid EmailId format.";
            return false;
        }
        return true;
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}

// CSV Import Model
public class ImportStudentModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string EmailId { get; set; }
}
