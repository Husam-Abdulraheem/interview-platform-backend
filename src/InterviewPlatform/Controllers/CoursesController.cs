using InterviewPlatform.Application.DTOs;
using InterviewPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InterviewPlatform.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var courses = await _courseService.GetAllCoursesAsync();
        return Ok(ApiResponse<IEnumerable<CourseDto>>.SuccessResult(courses));
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        var course = await _courseService.GetCourseByIdAsync(id);
        if (course == null) return NotFound(ApiResponse<object>.Fail("Course not found", 404));
        return Ok(ApiResponse<CourseDto>.SuccessResult(course));
    }

    [HttpGet("my-courses")]
    [Authorize]
    public async Task<IActionResult> GetMyCourses()
    {
        var courses = await _courseService.GetMyCoursesAsync();
        return Ok(ApiResponse<IEnumerable<CourseDto>>.SuccessResult(courses));
    }

    [HttpPost]
    [Authorize(Roles = "Creator,Admin")]
    public async Task<IActionResult> Create([FromBody] CreateCourseDto dto)
    {
        var course = await _courseService.CreateCourseAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = course.Id }, ApiResponse<CourseDto>.SuccessResult(course, "Course created successfully", 201));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Creator,Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCourseDto dto)
    {
        await _courseService.UpdateCourseAsync(id, dto);
        return Ok(ApiResponse<object>.SuccessResult(null, "Course updated successfully"));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Creator,Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _courseService.DeleteCourseAsync(id);
        return Ok(ApiResponse<object>.SuccessResult(null, "Course deleted successfully"));
    }

    // --- Question Management Endpoints ---

    [HttpPost("{id}/questions")]
    [Authorize(Roles = "Creator,Admin")]
    public async Task<IActionResult> AddQuestions(Guid id, [FromBody] List<string> questions)
    {
        await _courseService.AddQuestionsToCourseAsync(id, questions);
        return Ok(ApiResponse<object>.SuccessResult(null, "Questions added successfully"));
    }

    [HttpPut("questions/{questionId}")]
    [Authorize(Roles = "Creator,Admin")]
    public async Task<IActionResult> UpdateQuestion(Guid questionId, [FromBody] UpdateQuestionDto dto)
    {
        await _courseService.UpdateQuestionAsync(questionId, dto);
        return Ok(ApiResponse<object>.SuccessResult(null, "Question updated successfully"));
    }

    [HttpDelete("questions/{questionId}")]
    [Authorize(Roles = "Creator,Admin")]
    public async Task<IActionResult> DeleteQuestion(Guid questionId)
    {
        await _courseService.DeleteQuestionAsync(questionId);
        return Ok(ApiResponse<object>.SuccessResult(null, "Question deleted successfully"));
    }
}
