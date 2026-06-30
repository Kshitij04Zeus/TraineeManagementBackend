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

public class MentorController : ControllerBase {
  private readonly IMentorService _service;

  public MentorController(IMentorService service) 
  {
    _service = service;
  }

  [HttpGet]
  public async Task<ActionResult> GetAll(int? pageNumber, int? pageSize, string? search, MentorStatus? status) 
  {
      var (result, count) = await _service.GetAllMentors(pageNumber, pageSize, search, status);
      if (result == null) return NotFound();
      return Ok(new PagedMentorResponse() 
      { 
          PageNumber = pageNumber ?? 1,
          PageSize = pageSize ?? 10,
          TotalRecords = count,
          Data = result 
      });
  }

  [HttpGet("{id:int}")]
  public async Task<ActionResult> GetById(int id) {
      var response = await _service.GetByIdMentor(id);
      if (response == null)
        return NotFound();
      return Ok(response);
  }

  [HttpPost]
  public async Task<ActionResult> Create(CreateMentorRequest request) {
    return Ok(await _service.AddNewMentor(request));
  }

  [HttpPut("{id:int}")]
  public async Task<ActionResult> UpdateMentor(int id, UpdateMentorRequest request) {
      var updated = await _service.UpdateMentor(id, request);
      if (updated == null)
        return NotFound();
      return Ok(updated);
  }

  [HttpDelete("{id:int}")]
  public async Task<ActionResult> DeleteMentor(int id) 
  {
      if (await _service.DeleteMentor(id))
        return NoContent();
      return NotFound();
  }
}