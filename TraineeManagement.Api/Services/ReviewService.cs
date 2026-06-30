using TraineeManagement.Api.DTO;
using TraineeManagement.Api.Models;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Data;

namespace TraineeManagement.Api.Services;

public class ReviewService : IReviewService {
  private readonly AppDbContext _context;
  private readonly ILogger<ReviewService> _logger;

  public ReviewService(AppDbContext context, ILogger<ReviewService> logger) {
      _context = context;
      _logger = logger;
  }

  public async Task<ReviewResponse> CreateReview(CreateReviewRequest request) {
    var submission = await _context.Submissions.FindAsync(request.SubmissionId);
    if (submission == null) {
      throw new KeyNotFoundException("Submission Not Found");
    }

    var mentor = await _context.Mentors.FindAsync(request.MentorId);
    if (mentor == null) {
      throw new KeyNotFoundException("Mentor Not Found");
    }

    var newreview = new Review { 
      SubmissionId = request.SubmissionId,
      MentorId = request.MentorId,
      Feedback = request.Feedback,
      Score = request.Score ?? 0,
      ReviewStatus = request.ReviewStatus,
      ReviewedDate = request.ReviewedDate 
    };
    await _context.Reviews.AddAsync(newreview);
    await _context.SaveChangesAsync();
    _logger.LogInformation("Review created successfully with Id: {id}", newreview.Id);
    return MapToResponse(newreview);
  }

  public async Task<List<ReviewResponse>> GetAllReviews() {
    var reviews = await _context.Reviews.Include(t => t.Submission)
                      .Include(t => t.Mentor)
                      .ToListAsync();

    return reviews.Select(MapToResponse).ToList();
  }

  public async Task<ReviewResponse?> GetByIdReview(int id) {
    var review = await _context.Reviews.Include(t => t.Submission)
                     .Include(t => t.Mentor)
                     .FirstOrDefaultAsync(t => t.Id == id);
    if (review == null)
    {
        _logger.LogWarning("Review not found with Id:{id}", id);
        throw new KeyNotFoundException($"Review not found with Id {id}");
    }
    return MapToResponse(review);
  }

  private static ReviewResponse MapToResponse(Review request) {
    return new ReviewResponse 
    {
      Id = request.Id,
      SubmissionId = request.SubmissionId,
      SubmissionUrl = request.Submission.SubmissionUrl,
      SubmittedDate = request.Submission.SubmittedDate,
      SubmissionStatus = request.Submission.Status,
      MentorId = request.MentorId,
      MentorName = $"{request.Mentor.FirstName} {request.Mentor.LastName}",
      Feedback = request.Feedback,
      Score = request.Score,
      ReviewStatus = request.ReviewStatus,
      ReviewedDate = request.ReviewedDate,
    };
  }
}