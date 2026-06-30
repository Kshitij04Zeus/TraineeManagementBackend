using System.ComponentModel.DataAnnotations;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.DTO;

public class LoginResponse
{
    public string Token {get; set;}="";
    public int ExpiresIn {get; set;}
    public UserResponse User {get; set;}=new UserResponse();
}