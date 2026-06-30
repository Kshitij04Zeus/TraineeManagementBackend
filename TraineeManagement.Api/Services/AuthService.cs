using System.Diagnostics.Contracts;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.DTO;
using TraineeManagement.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace TraineeManagement.Api.Services;
public class AuthService:IAuthService
{
    private readonly AppDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthService> _logger;
    public AuthService(AppDbContext context, IJwtService jwtservice, ILogger<AuthService> logger)
    {
        _context=context;
        _jwtService=jwtservice;
        _logger=logger;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var hasher=new PasswordHasher<string>();        
        var user=await _context.Users.FirstOrDefaultAsync(t=>t.Username==request.Username);
        if(user==null) throw new UnauthorizedAccessException("Failed Authentication");

        var result=hasher.VerifyHashedPassword(user.Username,user.PasswordHash,request.Password);
        if(result==PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedAccessException("Incorrect Password");
        } 
        var token=_jwtService.GenerateToken(user);
         _logger.LogInformation("User {username} logged in successfully",request.Username);
        return new LoginResponse
        {
            Token=token,
            ExpiresIn=60,
            User=new UserResponse
            {
                Id=user.Id,
                Username=user.Username,
                Role=user.Role
            }
        };
    }
}