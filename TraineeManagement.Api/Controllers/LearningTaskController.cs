using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.DTO;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Services;
using Microsoft.AspNetCore.Authorization;
// using System.Threading.Tasks;

namespace TraineeManagement.Api.Controllers {
  [Authorize]
  [ApiController]
  [Route("api/learning-tasks")]

  public class LearningTaskController : ControllerBase {
    private readonly ILearningTaskService _service;

    public LearningTaskController(ILearningTaskService service) {
      _service = service;
    }

    [HttpGet]
    public async Task<ActionResult> GetAll(int? pageNumber, int? pageSize,
                                           string? search,
                                           LearningTaskStatus? status) {
      var (result, count) =
          await _service.GetAllTasks(pageNumber, pageSize, search, status);
      if (result == null)
        return NotFound();
      return Ok(new PagedTaskResponse() { PageNumber = pageNumber ?? 1,
                                          PageSize = pageSize ?? 10,
                                          TotalRecords = count,
                                          Data = result });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult> GetById(int id) {
      var response = await _service.GetByIdTask(id);
      if (response == null)
        return NotFound();
      return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult> Create(CreateLearningTaskRequest request) {
      return Ok(await _service.AddNewTask(request));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateTask(
        int id, UpdateLearningTaskRequest request) 
    {
      var updated = await _service.UpdateTask(id, request);
      if (updated == null)
        return NotFound();
      return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteTask(int id) {
      if (await _service.DeleteTask(id))
        return NoContent();
      return NotFound();
    }
  }
}