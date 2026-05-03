namespace InterviewPlatform.Application.DTOs;

public class QuestionDto
{
    public Guid Id { get; set; }
    public Guid InterviewId { get; set; }
    public Guid? CourseId { get; set; }
    public string Content { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
}
