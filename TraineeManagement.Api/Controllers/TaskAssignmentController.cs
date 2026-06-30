using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.DTO;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Services;
using Microsoft.AspNetCore.Authorization;

namespace TraineeManagement.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/task-assignments")]

public class TaskAssignmentController : ControllerBase 
{
    private readonly ITaskAssignmentService _service;
    public TaskAssignmentController(ITaskAssignmentService service) {
        _service = service;
    }

    [HttpGet] 
    public async Task<ActionResult> GetAll() 
    {
        var response = await _service.GetAllAssignments();
        return Ok(response);
    }

    [HttpGet("{id:int}")] 
    public async Task<ActionResult> GetById(int id) 
    {
        var response = await _service.GetByIdAssignment(id);
        if (response == null) return NotFound();
        return Ok(response);
    }

    [HttpPost] 
    public async Task<ActionResult> Create(CreateTaskAssignmentRequest request)
    {
        return Ok(await _service.CreateAssignment(request));
    }

    [HttpPut("{id:int}/status")] 
    public async Task<ActionResult> UpdateTaskAssignment(int id, UpdateTaskAssignmentRequest request) 
    {
        var updated = await _service.UpdateStatus(id, request);
        if (updated == false) return NotFound();
        return Ok(new {updated, message = "Task Status Updated successfully"});
    }
}