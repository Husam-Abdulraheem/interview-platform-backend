using InterviewPlatform.Application.DTOs;

namespace InterviewPlatform.Application.Interfaces;

public interface ICourseService
{
    Task<IEnumerable<CourseDto>> GetAllCoursesAsync();
    Task<CourseDto?> GetCourseByIdAsync(Guid id);
    Task<CourseDto> CreateCourseAsync(CreateCourseDto dto);
    Task UpdateCourseAsync(Guid id, UpdateCourseDto dto);
    Task DeleteCourseAsync(Guid id);
}
