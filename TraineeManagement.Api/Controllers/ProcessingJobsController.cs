using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.DTO;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Services;
using Microsoft.AspNetCore.Authorization;

namespace TraineeManagement.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/processing-jobs")]
public class ProcessingJobsController : ControllerBase
{
    private readonly IProcessingJobService _service;
    public ProcessingJobsController(IProcessingJobService service)
    {
        _service=service;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result=await _service.GetByIdAsync(id);
        if(result==null) return NotFound();
        return Ok(result);
    }

    [HttpPost("{id:int}/retry")]
    public async Task<IActionResult> RetryJob(int id)
    {
        var result=await _service.RetryProcessingJob(id);
        if(result==null) return NotFound();
        return Ok(result);
    }
}