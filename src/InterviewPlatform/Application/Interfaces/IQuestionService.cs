using InterviewPlatform.Application.DTOs;

namespace InterviewPlatform.Application.Interfaces;

public interface IQuestionService
{
    Task<IEnumerable<QuestionDto>> GetQuestionsByInterviewIdAsync(Guid interviewId);
    Task<QuestionDto> AddQuestionAsync(CreateQuestionDto dto);
    Task DeleteQuestionAsync(Guid id);
}
