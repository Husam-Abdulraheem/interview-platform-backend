namespace InterviewPlatform.Application.DTOs;

public class CreateCourseDto
{
    public Guid CreatorId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsGeneral { get; set; }
    public string? Specialty { get; set; }
    public string YouTubeVideoUrl { get; set; } = string.Empty;
    public string ContentMaterial { get; set; } = string.Empty;
}
