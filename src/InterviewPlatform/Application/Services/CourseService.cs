using InterviewPlatform.Application.DTOs;
using InterviewPlatform.Application.Exceptions;
using InterviewPlatform.Application.Interfaces;
using InterviewPlatform.Core.Entities;
using Mapster;

namespace InterviewPlatform.Application.Services;

public class CourseService : ICourseService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserContext _currentUserContext;

    public CourseService(IUnitOfWork unitOfWork, ICurrentUserContext currentUserContext)
    {
        _unitOfWork = unitOfWork;
        _currentUserContext = currentUserContext;
    }

    public async Task<IEnumerable<CourseDto>> GetAllCoursesAsync()
    {
        var courses = await _unitOfWork.Courses.GetAllAsync();
        return courses.Adapt<IEnumerable<CourseDto>>();
    }

    public async Task<CourseDto?> GetCourseByIdAsync(Guid id)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(id);
        return course?.Adapt<CourseDto>();
    }

    public async Task<CourseDto> CreateCourseAsync(CreateCourseDto dto)
    {
        var course = dto.Adapt<Course>();
        course.Id = Guid.NewGuid();
        course.CreatedAt = DateTime.UtcNow;
        course.CreatorId = _currentUserContext.UserId; // Automatically assign the Creator

        await _unitOfWork.Courses.AddAsync(course);
        await _unitOfWork.CompleteAsync();

        return course.Adapt<CourseDto>();
    }

    public async Task UpdateCourseAsync(Guid id, UpdateCourseDto dto)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(id);
        if (course == null) throw new NotFoundException($"Course with id {id} not found");

        if (!_currentUserContext.IsAdmin && course.CreatorId != _currentUserContext.UserId)
            throw new ForbiddenException("You do not have permission to update this course.");

        dto.Adapt(course);
        
        _unitOfWork.Courses.Update(course);
        await _unitOfWork.CompleteAsync();
    }

    public async Task DeleteCourseAsync(Guid id)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(id);
        if (course == null) throw new NotFoundException($"Course with id {id} not found");

        if (!_currentUserContext.IsAdmin && course.CreatorId != _currentUserContext.UserId)
            throw new ForbiddenException("You do not have permission to delete this course.");

        _unitOfWork.Courses.Remove(course);
        await _unitOfWork.CompleteAsync();
    }
}
