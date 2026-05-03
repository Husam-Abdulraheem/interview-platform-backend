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

    public async Task<IEnumerable<QuestionDto>> GetQuestionsByCourseIdAsync(Guid courseId)
    {
        // Validate that course exists
        var course = await _unitOfWork.Courses.GetByIdAsync(courseId);
        if (course == null)
            throw new NotFoundException($"Course with ID {courseId} not found.");

        var questions = await _unitOfWork.Questions.FindAsync(x => x.CourseId == courseId);
        return questions.OrderBy(q => q.OrderIndex).Adapt<IEnumerable<QuestionDto>>();
    }

    private async Task ValidateInterviewOwnershipAsync(Guid interviewId, string action)
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

    private async Task ValidateCourseOwnershipAsync(Guid courseId, string action)
    {
        if (_currentUserContext.IsAdmin) return;
        var course = await _unitOfWork.Courses.GetByIdAsync(courseId);
        if (course == null) return;

        if (course.CreatorId != _currentUserContext.UserId)
        {
            throw new ForbiddenException($"You do not have permission to {action} questions for this course.");
        }
    }

    public async Task<QuestionDto> AddQuestionAsync(CreateQuestionDto dto)
    {
        var question = dto.Adapt<Question>();
        question.Id = Guid.NewGuid();

        // Validate based on whether it's for course or interview
        if (dto.CourseId.HasValue)
        {
            await ValidateCourseOwnershipAsync(dto.CourseId.Value, "add");
        }
        else if (dto.InterviewId != Guid.Empty)
        {
            await ValidateInterviewOwnershipAsync(dto.InterviewId, "add");
        }

        await _unitOfWork.Questions.AddAsync(question);
        await _unitOfWork.CompleteAsync();

        return question.Adapt<QuestionDto>();
    }

    public async Task<QuestionDto> UpdateQuestionAsync(Guid id, UpdateQuestionDto dto)
    {
        var question = await _unitOfWork.Questions.GetByIdAsync(id);
        if (question == null) 
            throw new NotFoundException($"Question with id {id} not found");

        // Validate based on whether it's for course or interview
        if (question.CourseId.HasValue)
        {
            await ValidateCourseOwnershipAsync(question.CourseId.Value, "update");
        }
        else if (question.InterviewId != Guid.Empty)
        {
            await ValidateInterviewOwnershipAsync(question.InterviewId, "update");
        }

        question.Content = dto.Content;
        question.OrderIndex = dto.OrderIndex;

        _unitOfWork.Questions.Update(question);
        await _unitOfWork.CompleteAsync();

        return question.Adapt<QuestionDto>();
    }

    public async Task<QuestionDto?> GetQuestionByIdAsync(Guid id)
    {
        var question = await _unitOfWork.Questions.GetByIdAsync(id);
        return question?.Adapt<QuestionDto>();
    }

    public async Task DeleteQuestionAsync(Guid id)
    {
        var question = await _unitOfWork.Questions.GetByIdAsync(id);
        if (question == null) 
            throw new NotFoundException($"Question with id {id} not found");

        // Validate based on whether it's for course or interview
        if (question.CourseId.HasValue)
        {
            await ValidateCourseOwnershipAsync(question.CourseId.Value, "delete");
        }
        else if (question.InterviewId != Guid.Empty)
        {
            await ValidateInterviewOwnershipAsync(question.InterviewId, "delete");
        }

        _unitOfWork.Questions.Remove(question);
        await _unitOfWork.CompleteAsync();
    }
}
