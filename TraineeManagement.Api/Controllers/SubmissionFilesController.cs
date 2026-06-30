using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.DTO;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Services;
using Microsoft.AspNetCore.Authorization;

namespace TraineeManagement.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/submission-files")]

public class SubmissionFilesController : ControllerBase {
  private readonly ISubmissionFileService _service;
  public SubmissionFilesController(ISubmissionFileService service) {
    _service = service;
  }

  [HttpGet("{id:int}/download")]
  public async Task<ActionResult> Download(int id) 
  {
      var file = await _service.DownloadAsync(id);
      return File(file.Stream,file.ContentType,file.FileName);
  }

  [HttpDelete("{id:int}")]
  public async Task<ActionResult> Create(int id) {
      await _service.DeleteAsync(id);
      return NoContent();
  }

  [HttpGet("{id:int}")]
  public async Task<ActionResult> GetMetadata(int id) {
    string correlationId=HttpContext.TraceIdentifier;
    var result=await _service.GetMetadataByIdAsync(id,correlationId);
    return Ok(result);
  }

}