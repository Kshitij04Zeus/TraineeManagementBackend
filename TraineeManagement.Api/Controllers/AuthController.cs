using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.DTO;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;


namespace TraineeManagement.Api.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]

public class AuthController:ControllerBase
{
    private readonly IAuthService _authservice;

    public AuthController(IAuthService authservice)
    {
        _authservice=authservice;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result=await _authservice.LoginAsync(request);
        if(result==null)
        {
            return Unauthorized("Invalid Username or Password");
        }
        return Ok(result);
    }
}