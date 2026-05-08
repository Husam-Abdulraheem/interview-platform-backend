namespace InterviewPlatform.Application.DTOs;

public class UpdateCourseDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public bool? IsGeneral { get; set; }
    public string? Specialty { get; set; }
    public string? YouTubeVideoUrl { get; set; }
    public string? ContentMaterial { get; set; }
}
