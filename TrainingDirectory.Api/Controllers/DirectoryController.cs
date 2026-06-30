using Microsoft.AspNetCore.Mvc;
using TrainingDirectory.Api.Services;
namespace TrainingDirectory.Api.Controllers;

[ApiController]
[Route("api/directory")]
public class DirectoryController : ControllerBase
{
    private readonly IDirectoryService _service;
    public DirectoryController(IDirectoryService service)
    {
        _service=service;
    }

    [HttpGet("trainees/{id:int}")]
    public async Task<IActionResult> GetTraineeProfile(int id)
    {
        var result=await _service.GetTraineeProfileAsync(id);
        if(result==null) return NotFound(new { message = $"Trainee profile with ID {id} not found." });
        return Ok(result);
    }
}
