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
        if (course == null) return null;
        
        var courseDto = course.Adapt<CourseDto>();
        
        // Include questions for this course
        var courseQuestions = await _unitOfWork.Questions.FindAsync(q => q.CourseId == id);
        courseDto.Questions = courseQuestions.Adapt<List<QuestionDto>>();
        
        return courseDto;
    }

    public async Task<CourseDto> CreateCourseAsync(CreateCourseDto dto)
    {
        var course = dto.Adapt<Course>();
        course.Id = Guid.NewGuid();
        course.CreatedAt = DateTime.UtcNow;
        course.CreatorId = _currentUserContext.UserId; // Automatically assign the Creator

        if (dto.Questions != null && dto.Questions.Any())
        {
            course.Questions = dto.Questions.Select((q, index) => new Question
            {
                Id = Guid.NewGuid(),
                Content = q,
                OrderIndex = index,
                CourseId = course.Id
            }).ToList();
        }

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

        // Partial updates: Only update provided fields
        if (dto.Title != null) course.Title = dto.Title;
        if (dto.Description != null) course.Description = dto.Description;
        if (dto.IsGeneral != null) course.IsGeneral = dto.IsGeneral.Value;
        if (dto.Specialty != null) course.Specialty = dto.Specialty;
        if (dto.YouTubeVideoUrl != null) course.YouTubeVideoUrl = dto.YouTubeVideoUrl;
        if (dto.ContentMaterial != null) course.ContentMaterial = dto.ContentMaterial;

        if (dto.Questions != null)
        {
            // Remove existing questions
            var existingQuestions = await _unitOfWork.Questions.FindAsync(q => q.CourseId == id);
            _unitOfWork.Questions.RemoveRange(existingQuestions);

            // Add new questions
            course.Questions = dto.Questions.Select((q, index) => new Question
            {
                Id = Guid.NewGuid(),
                Content = q,
                OrderIndex = index,
                CourseId = id
            }).ToList();
        }
        
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

    public async Task<IEnumerable<CourseDto>> GetMyCoursesAsync()
    {
        var courses = await _unitOfWork.Courses.FindAsync(c => c.CreatorId == _currentUserContext.UserId);
        return courses.Adapt<IEnumerable<CourseDto>>();
    }
}
