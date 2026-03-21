using InterviewPlatform.Application.DTOs;
using InterviewPlatform.Application.Exceptions;
using InterviewPlatform.Application.Interfaces;
using InterviewPlatform.Core.Entities;
using Mapster;

namespace InterviewPlatform.Application.Services;

public class InterviewService : IInterviewService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserContext _currentUserContext;

    public InterviewService(IUnitOfWork unitOfWork, ICurrentUserContext currentUserContext)
    {
        _unitOfWork = unitOfWork;
        _currentUserContext = currentUserContext;
    }

    public async Task<IEnumerable<InterviewDto>> GetInterviewsByCourseIdAsync(Guid courseId)
    {
        var interviews = await _unitOfWork.Interviews.FindAsync(x => x.CourseId == courseId);
        return interviews.Adapt<IEnumerable<InterviewDto>>();
    }

    public async Task<InterviewDto?> GetInterviewByIdAsync(Guid id)
    {
        var interview = await _unitOfWork.Interviews.GetByIdAsync(id);
        return interview?.Adapt<InterviewDto>();
    }

    private async Task ValidateCourseOwnershipAsync(Guid courseId, string action)
    {
        if (_currentUserContext.IsAdmin) return;
        var course = await _unitOfWork.Courses.GetByIdAsync(courseId);
        if (course != null && course.CreatorId != _currentUserContext.UserId)
        {
            throw new ForbiddenException($"You do not have permission to {action} interviews for this course.");
        }
    }

    public async Task<InterviewDto> CreateInterviewAsync(CreateInterviewDto dto)
    {
        var interview = dto.Adapt<Interview>();
        interview.Id = Guid.NewGuid();

        await ValidateCourseOwnershipAsync(interview.CourseId, "create");

        await _unitOfWork.Interviews.AddAsync(interview);
        await _unitOfWork.CompleteAsync();

        return interview.Adapt<InterviewDto>();
    }

    public async Task UpdateInterviewAsync(Guid id, UpdateInterviewDto dto)
    {
        var interview = await _unitOfWork.Interviews.GetByIdAsync(id);
        if (interview == null) throw new NotFoundException($"Interview with id {id} not found");

        await ValidateCourseOwnershipAsync(interview.CourseId, "update");

        dto.Adapt(interview);

        _unitOfWork.Interviews.Update(interview);
        await _unitOfWork.CompleteAsync();
    }

    public async Task DeleteInterviewAsync(Guid id)
    {
        var interview = await _unitOfWork.Interviews.GetByIdAsync(id);
        if (interview == null) throw new NotFoundException($"Interview with id {id} not found");

        await ValidateCourseOwnershipAsync(interview.CourseId, "delete");

        _unitOfWork.Interviews.Remove(interview);
        await _unitOfWork.CompleteAsync();
    }
}
