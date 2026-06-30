using TraineeManagement.Api.DTO;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.Services;

public interface IMentorService
{
    Task<MentorResponse?> GetByIdMentor(int id);
    Task<(List<MentorResponse>?,int)> GetAllMentors(int? pageNumber, int? pageSize, string? search, MentorStatus? status);

    Task<MentorResponse> AddNewMentor(CreateMentorRequest request);
    Task<MentorResponse?> UpdateMentor(int id,UpdateMentorRequest request);
    Task<bool> DeleteMentor(int id);

}