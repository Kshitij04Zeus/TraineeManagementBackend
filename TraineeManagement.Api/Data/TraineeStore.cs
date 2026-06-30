using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.Data
{
    public static class TraineeStore
    {
        public static List<Trainee> Trainees=new List<Trainee>
        {
            new Trainee
            {
                Id=1,
                FirstName="Jason",
                LastName="Bourne",
                Email="jbourne@gmail.com",
                TechStack="Python",
                Status=TraineeStatus.Active,
                CreatedDate=DateTime.Now,
                UpdatedDate=DateTime.Now
            }
        };
    }
}