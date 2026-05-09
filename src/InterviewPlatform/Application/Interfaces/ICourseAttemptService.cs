using InterviewPlatform.Application.DTOs;

namespace InterviewPlatform.Application.Interfaces;

public interface ICourseAttemptService
{
    Task<CourseAttemptDto> StartAttemptAsync(StartCourseAttemptDto dto);
    Task<QuestionAttemptDto> SubmitAnswerAsync(Guid attemptId, SubmitAnswerDto dto);
    Task<CourseAttemptDto> CompleteAttemptAsync(Guid attemptId);
    Task<IEnumerable<CourseAttemptDto>> GetMyAttemptsAsync();
    Task<CourseAttemptDto?> GetAttemptByIdAsync(Guid attemptId);
}
