namespace InterviewPlatform.Application.DTOs;

public class InterviewDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int PassingScore { get; set; }
}
