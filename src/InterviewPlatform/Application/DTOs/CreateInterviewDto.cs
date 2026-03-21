namespace InterviewPlatform.Application.DTOs;

public class CreateInterviewDto
{
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int PassingScore { get; set; }
}
