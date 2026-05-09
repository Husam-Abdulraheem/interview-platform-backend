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
        var courseDtos = courses.Adapt<List<CourseDto>>();

        var courseIds = courseDtos.Select(d => d.Id).ToList();
        var allQuestions = await _unitOfWork.Questions.FindAsync(q => q.CourseId != null && courseIds.Contains(q.CourseId.Value));
        
        var questionsByCourse = allQuestions.GroupBy(q => q.CourseId)
                                            .ToDictionary(g => g.Key!.Value, g => g.ToList());

        foreach (var dto in courseDtos)
        {
            if (questionsByCourse.TryGetValue(dto.Id, out var questions))
            {
                dto.Questions = questions.Adapt<List<QuestionDto>>();
            }
        }

        return courseDtos;
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
        // 1. Map basic properties, but ignore Questions to handle them manually
        var course = dto.Adapt<Course>();
        
        course.Id = Guid.NewGuid();
        course.CreatedAt = DateTime.UtcNow;
        course.CreatorId = _currentUserContext.UserId; // Always use the ID from the token for security

        // 2. Clear any accidental mapping of questions and rebuild them correctly
        course.Questions = new List<Question>();
        
        if (dto.Questions != null && dto.Questions.Any())
        {
            foreach (var (content, index) in dto.Questions.Select((v, i) => (v, i)))
            {
                course.Questions.Add(new Question
                {
                    Id = Guid.NewGuid(),
                    Content = content,
                    OrderIndex = index,
                    CourseId = course.Id
                });
            }
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

        // Partial updates
        if (dto.Title != null) course.Title = dto.Title;
        if (dto.Description != null) course.Description = dto.Description;
        if (dto.IsGeneral != null) course.IsGeneral = dto.IsGeneral.Value;
        if (dto.Specialty != null) course.Specialty = dto.Specialty;
        if (dto.YouTubeVideoUrl != null) course.YouTubeVideoUrl = dto.YouTubeVideoUrl;
        if (dto.ContentMaterial != null) course.ContentMaterial = dto.ContentMaterial;

        // Note: Questions are now managed via dedicated endpoints to avoid accidental deletion
        // and to allow updating individual questions without re-sending the whole list.
        
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
        var courseDtos = courses.Adapt<List<CourseDto>>();

        var courseIds = courseDtos.Select(d => d.Id).ToList();
        var allQuestions = await _unitOfWork.Questions.FindAsync(q => q.CourseId != null && courseIds.Contains(q.CourseId.Value));

        var questionsByCourse = allQuestions.GroupBy(q => q.CourseId)
                                            .ToDictionary(g => g.Key!.Value, g => g.ToList());

        foreach (var dto in courseDtos)
        {
            if (questionsByCourse.TryGetValue(dto.Id, out var questions))
            {
                dto.Questions = questions.Adapt<List<QuestionDto>>();
            }
        }

        return courseDtos;
    }

    public async Task AddQuestionsToCourseAsync(Guid courseId, List<string> questionContents)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(courseId);
        if (course == null) throw new NotFoundException($"Course with id {courseId} not found");

        if (!_currentUserContext.IsAdmin && course.CreatorId != _currentUserContext.UserId)
            throw new ForbiddenException("You do not have permission to add questions to this course.");

        // Get current max index to append properly
        var existingQuestions = await _unitOfWork.Questions.FindAsync(q => q.CourseId == courseId);
        int startIndex = existingQuestions.Any() ? existingQuestions.Max(q => q.OrderIndex) + 1 : 0;

        var newQuestions = questionContents.Select((content, index) => new Question
        {
            Id = Guid.NewGuid(),
            Content = content,
            OrderIndex = startIndex + index,
            CourseId = courseId // Ensure the course ID is explicitly set
        }).ToList();

        await _unitOfWork.Questions.AddRangeAsync(newQuestions);
        await _unitOfWork.CompleteAsync();
    }

    public async Task UpdateQuestionAsync(Guid questionId, UpdateQuestionDto dto)
    {
        var question = await _unitOfWork.Questions.GetByIdAsync(questionId);
        if (question == null) throw new NotFoundException($"Question with id {questionId} not found");

        var course = await _unitOfWork.Courses.GetByIdAsync((Guid)question.CourseId);
        if (course != null && !_currentUserContext.IsAdmin && course.CreatorId != _currentUserContext.UserId)
            throw new ForbiddenException("You do not have permission to update this question.");

        question.Content = dto.Content;
        question.OrderIndex = dto.OrderIndex;

        _unitOfWork.Questions.Update(question);
        await _unitOfWork.CompleteAsync();
    }

    public async Task DeleteQuestionAsync(Guid questionId)
    {
        var question = await _unitOfWork.Questions.GetByIdAsync(questionId);
        if (question == null) throw new NotFoundException($"Question with id {questionId} not found");

        var course = await _unitOfWork.Courses.GetByIdAsync((Guid)question.CourseId);
        if (course != null && !_currentUserContext.IsAdmin && course.CreatorId != _currentUserContext.UserId)
            throw new ForbiddenException("You do not have permission to delete this question.");

        _unitOfWork.Questions.Remove(question);
        await _unitOfWork.CompleteAsync();
    }
}
