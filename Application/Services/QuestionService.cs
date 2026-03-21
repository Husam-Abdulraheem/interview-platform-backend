using InterviewPlatform.Application.DTOs;
using InterviewPlatform.Application.Exceptions;
using InterviewPlatform.Application.Interfaces;
using InterviewPlatform.Core.Entities;
using Mapster;

namespace InterviewPlatform.Application.Services;

public class QuestionService : IQuestionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserContext _currentUserContext;

    public QuestionService(IUnitOfWork unitOfWork, ICurrentUserContext currentUserContext)
    {
        _unitOfWork = unitOfWork;
        _currentUserContext = currentUserContext;
    }

    public async Task<IEnumerable<QuestionDto>> GetQuestionsByInterviewIdAsync(Guid interviewId)
    {
        var questions = await _unitOfWork.Questions.FindAsync(x => x.InterviewId == interviewId);
        return questions.OrderBy(q => q.OrderIndex).Adapt<IEnumerable<QuestionDto>>();
    }

    private async Task ValidateCourseOwnershipAsync(Guid interviewId, string action)
    {
        if (_currentUserContext.IsAdmin) return;
        var interview = await _unitOfWork.Interviews.GetByIdAsync(interviewId);
        if (interview == null) return;

        var course = await _unitOfWork.Courses.GetByIdAsync(interview.CourseId);
        if (course != null && course.CreatorId != _currentUserContext.UserId)
        {
            throw new ForbiddenException($"You do not have permission to {action} questions for this interview.");
        }
    }

    public async Task<QuestionDto> AddQuestionAsync(CreateQuestionDto dto)
    {
        var question = dto.Adapt<Question>();
        question.Id = Guid.NewGuid();

        await ValidateCourseOwnershipAsync(question.InterviewId, "add");

        await _unitOfWork.Questions.AddAsync(question);
        await _unitOfWork.CompleteAsync();

        return question.Adapt<QuestionDto>();
    }

    public async Task DeleteQuestionAsync(Guid id)
    {
        var question = await _unitOfWork.Questions.GetByIdAsync(id);
        if (question == null) throw new NotFoundException($"Question with id {id} not found");

        await ValidateCourseOwnershipAsync(question.InterviewId, "delete");

        _unitOfWork.Questions.Remove(question);
        await _unitOfWork.CompleteAsync();
    }
}
