using System.ComponentModel.DataAnnotations;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Constants;

namespace TraineeManagement.Api.DTO;
public class LoginRequest
{
    [Required(ErrorMessage = ValidationMessages.UsernameReq)]
    public string Username {get; set;}="";

    [Required(ErrorMessage = ValidationMessages.PasswordReq)]
    public string Password {get; set;}="";
}