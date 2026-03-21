using InterviewPlatform.Application.DTOs;

namespace InterviewPlatform.Application.Interfaces;

public interface IInterviewService
{
    Task<IEnumerable<InterviewDto>> GetInterviewsByCourseIdAsync(Guid courseId);
    Task<InterviewDto?> GetInterviewByIdAsync(Guid id);
    Task<InterviewDto> CreateInterviewAsync(CreateInterviewDto dto);
    Task UpdateInterviewAsync(Guid id, UpdateInterviewDto dto);
    Task DeleteInterviewAsync(Guid id);
}
