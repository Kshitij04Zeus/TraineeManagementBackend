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

public class ReviewsController : ControllerBase {
  private readonly IReviewService _service;
  public ReviewsController(IReviewService service) {
    _service = service;
  }

  [HttpGet]
  public async Task<ActionResult> GetAll() {
      var response = await _service.GetAllReviews();
      return Ok(response);
  }

  [HttpGet("{id:int}")]
  public async Task<ActionResult> GetById(int id) {
      var response = await _service.GetByIdReview(id);
      if (response == null)
        return NotFound();
      return Ok(response);
  }

  [HttpPost]
  public async Task<ActionResult> Create(CreateReviewRequest request) {
      return Ok(await _service.CreateReview(request));
  }
}