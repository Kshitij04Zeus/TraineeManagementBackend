using System.ComponentModel.DataAnnotations;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.DTO;
public class UserResponse
{
    public int Id {get; set;}
    public string Username {get; set;}="";
    public UserRole Role {get; set;}=UserRole.Trainee;
}