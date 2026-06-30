using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TraineeManagement.Api.Constants;

namespace TraineeManagement.Api.Models;
public enum UserRole
{
    Admin,
    Mentor,
    Trainee
}
public class User
{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = ValidationMessages.UsernameReq)]
        [MaxLength(50)]
        public string Username { get; set; }="";

        [Required(ErrorMessage = ValidationMessages.EmailReq)]
        [EmailAddress]
        public string Email { get; set; }="";

        [Required(ErrorMessage = ValidationMessages.PasswordReq)]
        public string PasswordHash { get; set; }="";

        [Required(ErrorMessage = ValidationMessages.UserRoleReq)]
        public UserRole Role { get; set; }=UserRole.Trainee;

        public DateTime CreatedDate { get; set; }=DateTime.Now;

        public DateTime UpdatedDate { get; set; }= DateTime.Now;
}