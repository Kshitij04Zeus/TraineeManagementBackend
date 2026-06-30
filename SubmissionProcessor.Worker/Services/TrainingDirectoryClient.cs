using System.Net;
using System.Net.Http.Json;
using SubmissionProcessor.Worker.Contracts;
 
namespace SubmissionProcessor.Worker.Services;
 
public class TrainingDirectoryClient : ITrainingDirectoryClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TrainingDirectoryClient> _logger;
 
    public TrainingDirectoryClient(HttpClient httpClient,ILogger<TrainingDirectoryClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
 
    public async Task<DirectoryTraineeProfileResponse?> GetTraineeProfileAsync(int traineeId, string? correlationId = null,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get,$"api/directory/trainees/{traineeId}");
 
        if (!string.IsNullOrWhiteSpace(correlationId))
        {
            request.Headers.Add("X-Correlation-Id", correlationId);
        }
 
        var response = await _httpClient.SendAsync(request, cancellationToken);
 
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogWarning("Trainee profile not found in TrainingDirectory for TraineeId {TraineeId}",traineeId); 
            return null;
        }
 
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);

            _logger.LogError("TrainingDirectory trainee lookup failed for TraineeId {TraineeId}. StatusCode: {StatusCode}", traineeId, (int)response.StatusCode);

            if (response.StatusCode == HttpStatusCode.BadRequest || 
                response.StatusCode == HttpStatusCode.Unauthorized || 
                response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new InvalidOperationException($"Client error (Non-Transient). Status: {response.StatusCode}. Error: {error}");
            }
            throw new HttpRequestException($"Transient server error. Status={(int)response.StatusCode}. Response={error}", null, response.StatusCode);
        }

 
        return await response.Content.ReadFromJsonAsync<DirectoryTraineeProfileResponse>(cancellationToken: cancellationToken);
    }
}
 