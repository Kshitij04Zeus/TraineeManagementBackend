using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace TraineeManagement.Api.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly HealthCheckService _service;
        public HealthController(HealthCheckService service)
        {
            _service=service;
        }
        [HttpGet("live")]
        public async Task<IActionResult> Live()
        {
            return Ok(
                new
                {
                    status="Application Is Alive",
                }
            );
        }

        [HttpGet("ready")]
        public async Task<IActionResult> Ready()
        {
            var report=await _service.CheckHealthAsync();
            var response=new 
            {
                status=report.Status.ToString(),
                checks=report.Entries.Select(x=>new
                {
                    service=x.Key,
                    status=x.Value.Status.ToString(),
                    description=x.Value.Description
                })
            };
            if(report.Status==HealthStatus.Healthy) return Ok(response);
            return StatusCode(503,response);
        }
    }
}