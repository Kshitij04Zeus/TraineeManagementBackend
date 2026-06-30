using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TraineeManagement.Api.Constants;
namespace TraineeManagement.Api.Models
{
    public enum TraineeStatus
    {
        Active,
        Inactive,
        Completed
    }
    public class Trainee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

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
        public DateTime CreatedDate { get; set; }=DateTime.Now;

        public DateTime UpdatedDate { get; set; }=DateTime.Now;
    }
}