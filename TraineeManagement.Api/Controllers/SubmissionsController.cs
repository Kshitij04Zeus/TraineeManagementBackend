using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.DTO;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Services;
using Microsoft.AspNetCore.Authorization;

namespace TraineeManagement.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]

public class SubmissionsController : ControllerBase {
  private readonly ISubmissionService _service;
  private readonly ISubmissionFileService _submissionservice;
  public SubmissionsController(ISubmissionService service,ISubmissionFileService submissionservice) {
    _service = service;
    _submissionservice=submissionservice;
  }

  [HttpGet]
  public async Task<ActionResult> GetAll() 
  {
      var response = await _service.GetAllSubmissions();
      return Ok(response);
  }

  [HttpGet("{id:int}")]
  public async Task<ActionResult> GetById(int id) 
  {
      var response = await _service.GetByIdSubmission(id);
      if (response == null)
        return NotFound();
      return Ok(response);
  }

  [HttpPost]
  public async Task<ActionResult> Create(CreateSubmissionRequest request) {
      return Ok(await _service.CreateSubmission(request));
  }

  [DisableRequestSizeLimit] 
  [RequestFormLimits(MultipartBodyLengthLimit = 20971520)]
  [HttpPost("{submissionId:int}/files")]
  public async Task<ActionResult> Upload(int submissionId,int userId,IFormFile file) 
  {
      var correlationId=HttpContext.TraceIdentifier;
      var response=await _submissionservice.UploadAsync(submissionId,userId,file,correlationId);
      return Accepted(response);
  }
}