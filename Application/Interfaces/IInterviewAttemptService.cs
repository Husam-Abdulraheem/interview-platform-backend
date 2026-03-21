using InterviewPlatform.Application.DTOs;

namespace InterviewPlatform.Application.Interfaces;

public interface IInterviewAttemptService
{
    Task<InterviewAttemptDto> StartAttemptAsync(Guid traineeId, Guid interviewId);
    Task<AnswerAttemptDto> SubmitAnswerAsync(SubmitAnswerDto dto);
    Task<InterviewAttemptDto> CompleteAttemptAsync(Guid attemptId);
    Task<InterviewAttemptDto?> GetAttemptDetailsAsync(Guid attemptId);
}
