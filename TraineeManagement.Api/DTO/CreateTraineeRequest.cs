using System.ComponentModel.DataAnnotations;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Constants;
namespace TraineeManagement.Api.DTO;

public class CreateTraineeRequest
{
        [Required(ErrorMessage = ValidationMessages.FirstNameReq)]
        [MaxLength(50)]
        public string FirstName { get; set; }="";

        [Required(ErrorMessage = ValidationMessages.LastNameReq)]
        [MaxLength(50)]
        public string LastName { get; set; }="";

        [Required(ErrorMessage = ValidationMessages.EmailReq)]
        [EmailAddress]
        public string Email { get; set; }="";

        [Required(ErrorMessage = ValidationMessages.TechStackReq)]
        public string TechStack { get; set; }="";

        [Required(ErrorMessage = ValidationMessages.StatusReq)]
        public TraineeStatus Status { get; set; }=TraineeStatus.Active;
}