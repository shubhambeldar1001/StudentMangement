using Microsoft.AspNetCore.Mvc;
using StudentMangement.Model;
using StudentMangement.Repositories;

[ApiController]
[Route("api/[controller]")]
public class ClassController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public ClassController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var @class = await _unitOfWork.Classes.GetByIdAsync(id);
        if (@class == null) return NotFound();
        return Ok(@class);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Class @class)
    {
        await _unitOfWork.Classes.AddAsync(@class);
        await _unitOfWork.CompleteAsync();
        return CreatedAtAction(nameof(GetById), new { id = @class.Id }, @class);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, Class @class)
    {
        var existingClass = await _unitOfWork.Classes.GetByIdAsync(id);
        if (existingClass == null) return NotFound();

        existingClass.Name = @class.Name;
        existingClass.Description = @class.Description;

        _unitOfWork.Classes.Update(existingClass);
        await _unitOfWork.CompleteAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var @class = await _unitOfWork.Classes.GetByIdAsync(id);
        if (@class == null) return NotFound();

        _unitOfWork.Classes.Delete(@class);
        await _unitOfWork.CompleteAsync();

        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _unitOfWork.Classes.GetAllAsync());
}
