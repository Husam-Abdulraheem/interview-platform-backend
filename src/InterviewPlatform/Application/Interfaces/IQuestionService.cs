using InterviewPlatform.Application.DTOs;

namespace InterviewPlatform.Application.Interfaces;

public interface IQuestionService
{
    Task<IEnumerable<QuestionDto>> GetQuestionsByInterviewIdAsync(Guid interviewId);
    Task<IEnumerable<QuestionDto>> GetQuestionsByCourseIdAsync(Guid courseId);
    Task<QuestionDto?> GetQuestionByIdAsync(Guid id);
    Task<QuestionDto> AddQuestionAsync(CreateQuestionDto dto);
    Task<QuestionDto> UpdateQuestionAsync(Guid id, UpdateQuestionDto dto);
    Task DeleteQuestionAsync(Guid id);
}
