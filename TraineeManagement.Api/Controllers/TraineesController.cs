using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.DTO;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Services;
using Microsoft.AspNetCore.Authorization;
// using System.Threading.Tasks;

namespace TraineeManagement.Api.Controllers 
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TraineesController : ControllerBase 
    {
        private readonly ITraineeService _service;

        public TraineesController(ITraineeService service)
        { 
            _service = service; 
        }

        [HttpGet]
        public async Task<ActionResult> GetAll(int? pageNumber, int? pageSize, string? search, TraineeStatus? status)
        {
            var(result, count) = await _service.GetAllTrainees(pageNumber, pageSize, search, status);
            if (result == null) return NotFound();
            return Ok(new PagedResponse(){
                            PageNumber=pageNumber??1,
                            PageSize=pageSize??10,
                            TotalRecords=count,
                            Data=result
                        });
        }

        [HttpGet("{id:int}")] 
        public async Task<ActionResult> GetById(int id) {
        var response = await _service.GetById(id);
        if (response == null) return NotFound();
        return Ok(response);
        }

        [HttpPost] 
        public async Task<ActionResult> Create(CreateTraineeRequest request) 
        {
            return Ok(await _service.AddNewTrainee(request));
        }

        [HttpPut("{id:int}")] 
        public async Task<ActionResult> UpdateTrainee(int id, UpdateTraineeRequest request) 
        {
            var updated = await _service.UpdateTrainee(id, request);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id:int}")] 
        public async Task<ActionResult> DeleteTrainee(int id) 
        {
            if (await _service.DeleteTrainee(id)) return NoContent();
            return NotFound();
        }
    }
}  // namespace TraineeManagement.Api. Controllers