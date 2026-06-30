using TraineeManagement.Api.DTO;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.Services;
public interface IReviewService
{
    Task<ReviewResponse?> GetByIdReview(int id);
    Task<ReviewResponse> CreateReview(CreateReviewRequest request);
    Task<List<ReviewResponse>> GetAllReviews();
}